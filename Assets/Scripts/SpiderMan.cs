using UnityEngine;

public class SpiderMan : MonoBehaviour
{
    public float RunSpeed = 5;
    public float SprintingSpeed = 10;
    public float RotationSpeed = 15;

    public Animator Animator;
    public LineRenderer LineRenderer;
    public Building ClimbingObject;
    public GameObject Hand;

    private Rigidbody _rigidbody;
    private SpringJoint _joint;
    private RaycastHit _hit;

    private bool _isMoving;
    private bool _isClimbing;
    private bool _isFalling;
    private bool _isSwinging;
    private bool _isGrounded = true;

    private float _inAirTimer;

    private Vector3 _moveDirection;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleFalling();
        HandleMovement();
        HandleRotation();
        HandleShootingWeb();
        HandleReleasingWeb();
        DrawWeb();
    }

    void OnCollisionEnter(Collision collision)
    {
        var colObject = collision.collider.gameObject;
        if (!_isFalling && collision.collider.CompareTag("Climbable"))
        {
            if (ClimbingObject == null)
            {
                ClimbingObject = colObject.GetComponent<Building>();
            }
            _isClimbing = true;
            _isGrounded = false;
            StopSwinging();
        }

        if (!_isGrounded && collision.collider.CompareTag("Walkable"))
        {
            _inAirTimer = 0;
            _isFalling = false;
            _isGrounded = true;
            _isClimbing = false;
            StopSwinging();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (_isGrounded && collision.collider.CompareTag("Walkable") && transform.position.y > 2)
        {
            _isGrounded = false;
        }
    }

    private void HandleMovement()
    {
        if (_isFalling)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        if (_isSwinging)
        {
            _rigidbody.velocity += (transform.forward + transform.up * 1.75f) * 10f * Time.deltaTime;
            return;
        }

        if (!_isGrounded && _isClimbing)
        {
            if (ClimbingObject != null && transform.position.y > ClimbingObject.GetComponent<MeshRenderer>().bounds.max.y * ClimbingObject.HeightPercent)
            {
                _isClimbing = false;
                ClimbingObject = null;
                transform.position += Vector3.up * 2f + transform.forward / 1.5f;
                Animator.SetInteger("State", (int)SpiderManAnimationState.HardLanding);
            }
        }

        if (_isClimbing)
        {
            _moveDirection = transform.up * Input.GetAxisRaw("Vertical");
            _moveDirection += transform.right * Input.GetAxisRaw("Horizontal");
            _moveDirection.Normalize();
            _moveDirection *= 1.5f;
        }
        else
        {
            _moveDirection = Camera.main.transform.forward * Input.GetAxisRaw("Vertical");
            _moveDirection += Camera.main.transform.right * Input.GetAxisRaw("Horizontal");
            _moveDirection.Normalize();
            _moveDirection.y = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleJumping();
        }

        _isMoving = Mathf.Abs(_moveDirection.magnitude) > 0;
        if (_isMoving)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !_isClimbing)
            {
                if (_isGrounded)
                {
                    Animator.SetInteger("State", (int)SpiderManAnimationState.Sprint);
                }
                _moveDirection *= SprintingSpeed;
            }
            else
            {
                if (_isGrounded)
                {
                    Animator.SetInteger("State", (int)SpiderManAnimationState.Run);
                }
                _moveDirection *= RunSpeed;
            }
        }
        else if (_isGrounded)
        {
            Animator.SetInteger("State", (int)SpiderManAnimationState.Idle);
        }

        if (_isClimbing && !_isGrounded)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                if (hit.collider.gameObject == ClimbingObject && hit.distance > 0.5f)
                {
                    _moveDirection += transform.forward;
                }
            }
            if (_isMoving)
            {
                _moveDirection /= 5;
                Animator.SetInteger("State", (int)SpiderManAnimationState.Climbing);
            }
            else
            {
                Animator.SetInteger("State", (int)SpiderManAnimationState.ClimbingIdle);
            }
        }

        _rigidbody.velocity = _moveDirection;
    }

    private void HandleRotation()
    {
        if (_isClimbing)
        {
            if (ClimbingObject != null)
            {
                Vector3 targetPostition = new Vector3(ClimbingObject.transform.position.x, transform.position.y, ClimbingObject.transform.position.z);
                transform.LookAt(targetPostition);
            }
            return;
        }

        if (_isFalling)
        {
            return;
        }

        var targetDirection = Camera.main.transform.forward * Input.GetAxisRaw("Vertical");
        targetDirection += Camera.main.transform.right * Input.GetAxisRaw("Horizontal");
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFalling()
    {
        if (!_isGrounded && !_isClimbing && !_isSwinging)
        {
            _inAirTimer += Time.deltaTime * 3f;
            _rigidbody.AddForce(-Vector3.up * 150f * _inAirTimer);

            Physics.Raycast(transform.position, -transform.up, out RaycastHit hit);
            if (!_isFalling && _inAirTimer > 1.5f && hit.distance > 5 && hit.collider.CompareTag("Walkable"))
            {
                Animator.SetInteger("State", (int)SpiderManAnimationState.Falling);
            }
        }
    }

    private void HandleJumping()
    {
        if (_isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * 500);
            Animator.SetInteger("State", _isMoving ? (int)SpiderManAnimationState.RunningJump : (int)SpiderManAnimationState.Jump);
            _isGrounded = false;
        }
        else if (_isClimbing)
        { 
            _rigidbody.AddForce(-transform.forward * 1500);
            Animator.SetInteger("State", (int)SpiderManAnimationState.ClimbJump);
            _isClimbing = false;
            ClimbingObject = null;
            _isFalling = true;
        }
    }

    private void HandleShootingWeb()
    {
        if (_isSwinging || _isClimbing)
        {
            return;
        }

        if (Input.GetMouseButton(0) && _joint == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 150))
            {
                float distanceFromPoint = Vector3.Distance(transform.position, hit.point);
                if (hit.point.y > 2 && hit.point.y > transform.position.y)
                {
                    _hit = hit;
                    _hit.point += Vector3.up;
                    Animator.SetInteger("State", (int)SpiderManAnimationState.Swinging);

                    Vector3 targetPostition = new Vector3(_hit.point.x, transform.position.y, _hit.point.z);
                    transform.LookAt(targetPostition);

                    _isSwinging = true;
                    _isGrounded = false;
                    _isFalling = false;

                    _joint = gameObject.AddComponent<SpringJoint>();
                    _joint.autoConfigureConnectedAnchor = false;
                    _joint.connectedAnchor = _hit.point;

                    _joint.maxDistance = distanceFromPoint * 0.85f;
                    _joint.minDistance = distanceFromPoint * 0.25f;
                    _joint.spring = 200f;
                    _joint.spring = 5;
                    _joint.damper = 0f;

                    Invoke("ReleaseWeb", 1.5f);
                }
            }
        }
    }

    private void HandleReleasingWeb()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseWeb();
        }
    }

    private void ReleaseWeb()
    {
        if (!_isSwinging)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
        }
        else
        {
            Animator.SetInteger("State", (int)SpiderManAnimationState.Falling);
        }
        StopSwinging();
    }

    private void StopSwinging()
    {
        _isSwinging = false;
        Destroy(_joint);
    }

    private void DrawWeb()
    {
        if (_isSwinging)
        {
            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(0, Hand.transform.position);
            LineRenderer.SetPosition(1, _hit.point);
        }
        else
        {
            LineRenderer.positionCount = 0;
        }
    }
}

public enum SpiderManAnimationState
{
    Idle = 0,
    Run = 1,
    Sprint = 2,
    Jump = 3,
    RunningJump = 4,
    Climbing = 5,
    ClimbingIdle = 6,
    ClimbJump = 7,
    HardLanding = 8,
    Falling = 9,
    Swinging = 10
}