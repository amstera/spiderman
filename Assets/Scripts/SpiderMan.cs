using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiderMan : MonoBehaviour
{
    public float RunSpeed = 5;
    public float SprintingSpeed = 10;
    public float RotationSpeed = 15;
    public float Health = 100;

    public Animator Animator;
    public HealthBar HealthBar;
    public LineRenderer LineRenderer;
    public Building ClimbingObject;
    public GameObject Hand;

    public AudioSource WebAS;
    public AudioSource RunningAS;
    public AudioSource ClimbingAS;
    public AudioSource HurtAS;
    public AudioSource ThudAS;

    private Rigidbody _rigidbody;
    private SpringJoint _joint;
    private RaycastHit _hit;

    private bool _isMoving;
    private bool _isClimbing;
    private bool _isFalling;
    private bool _isSwinging;
    private bool _isRecovering;
    private bool _isGrounded = true;

    private float _inAirTimer;

    private Vector3 _moveDirection;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckForWater();
        HandleFalling();
        HandleMovement();
        HandleRotation();
        HandleShootingWeb();
        HandleReleasingWeb();
    }

    void LateUpdate()
    {
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

            if (!ClimbingAS.isPlaying)
            {
                ClimbingAS.pitch = Random.Range(0.95f, 1.05f);
                ClimbingAS.Play();
            }
        }

        if (!_isGrounded && collision.collider.CompareTag("Walkable"))
        {
            if (_isRecovering)
            {
                return;
            }

            _isFalling = false;
            _isClimbing = false;
            StopSwinging();

            if (_inAirTimer > 2.5f && Animator.GetInteger("State") == (int)SpiderManAnimationState.Falling)
            {
                _isRecovering = true;
                ThudAS.Play();
                Animator.SetInteger("State", (int)SpiderManAnimationState.HardLanding);
                Invoke("Grounded", 0.5f);
            }
            else
            {
                _isGrounded = true;
            }

            _inAirTimer = 0;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (_isGrounded && collision.collider.CompareTag("Walkable") && transform.position.y > 1)
        {
            _isGrounded = false;
        }
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        HealthBar.UpdateValue(Health);
        HurtAS.Play();
    }

    private void HandleMovement()
    {
        if ((!_isMoving || !_isGrounded) && RunningAS.isPlaying)
        {
            RunningAS.Stop();
        }

        if (!_isClimbing && ClimbingAS.isPlaying)
        {
            ClimbingAS.Stop();
        }

        if (_isFalling || _isRecovering)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        if (_isSwinging)
        {
            _rigidbody.velocity += (transform.forward + transform.up * (_hit.point.y > transform.position.y + 5 ? 1.75f : 0)) * 10f * Time.deltaTime;
            return;
        }

        if (!_isGrounded && _isClimbing)
        {
            if (ClimbingObject != null && transform.position.y > ClimbingObject.GetComponent<MeshRenderer>().bounds.max.y * ClimbingObject.HeightPercent)
            {
                ClimbingAS.Stop();
                _isClimbing = false;
                ClimbingObject = null;
                transform.position += Vector3.up * 2.25f + transform.forward / 1.5f;
                ThudAS.Play();
                Animator.SetInteger("State", (int)SpiderManAnimationState.HardLanding);
            }
        }

        if (_isClimbing)
        {
            _moveDirection = transform.up * Input.GetAxisRaw("Vertical");
            _moveDirection += transform.right * Input.GetAxisRaw("Horizontal");
            _moveDirection.Normalize();
            _moveDirection *= 2f;
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
        bool isSprinting = false;
        if (_isMoving)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !_isClimbing)
            {
                if (_isGrounded)
                {
                    Animator.SetInteger("State", (int)SpiderManAnimationState.Sprint);
                }
                _moveDirection *= SprintingSpeed;
                isSprinting = true;
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

        if (_isGrounded && _isMoving)
        {
            RunningAS.pitch = isSprinting ? 1.5f : 1;
            if (!RunningAS.isPlaying)
            {
                RunningAS.Play();
            }
        }

        if (_isClimbing && !_isGrounded)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                if (hit.collider.gameObject.name == ClimbingObject.name && hit.distance > 0.5f)
                {
                    _moveDirection += transform.forward;
                }
                else if (hit.distance > 2)
                {
                    StopClimbing();
                }
            }
            if (_isMoving)
            {
                if (!ClimbingAS.isPlaying)
                {
                    ClimbingAS.pitch = Random.Range(0.95f, 1.05f);
                    ClimbingAS.Play();
                }
                _moveDirection /= 5;
                Animator.SetInteger("State", (int)SpiderManAnimationState.Climbing);
            }
            else
            {
                ClimbingAS.Stop();
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

        if (_isFalling || _isRecovering)
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
        if (_isGrounded && (transform.position.z < -24.6f || transform.position.z > 126f || transform.position.x < -18 || transform.position.x > 107))
        {
            _isGrounded = false;
        }
        if (!_isGrounded && !_isClimbing && !_isSwinging)
        {
            _inAirTimer += Time.deltaTime * 2.5f;
            _rigidbody.AddForce(-Vector3.up * 125f * _inAirTimer);

            Physics.Raycast(transform.position, -transform.up, out RaycastHit hit);
            if (_inAirTimer > (_isFalling ? 3 : 1.5f) && hit.distance > 5 && hit.collider.CompareTag("Walkable"))
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
            StopClimbing();
        }
    }

    private void HandleShootingWeb()
    {
        if (_isClimbing)
        {
            return;
        }

        if (_isSwinging)
        {
            if (transform.position.y < _hit.point.y && transform.position.x < _hit.point.x)
            {
                Animator.SetInteger("State", (int)SpiderManAnimationState.SwingingBothArms);
            }
            else
            {
                Animator.SetInteger("State", (int)SpiderManAnimationState.Swinging);
            }
            return;
        }

        if (Input.GetMouseButton(0) && _joint == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 75))
            {
                float distanceFromPoint = Vector3.Distance(transform.position, hit.point);
                if (hit.point.y > 3 && hit.point.y > transform.position.y)
                {
                    _hit = hit;
                    _hit.point += Vector3.up * 1.5f;
                    Animator.SetInteger("State", (int)SpiderManAnimationState.Swinging);

                    Vector3 targetPostition = new Vector3(_hit.point.x, transform.position.y, _hit.point.z);
                    transform.LookAt(targetPostition);

                    _isSwinging = true;
                    _isGrounded = false;
                    _isFalling = false;
                    _isClimbing = false;

                    _joint = gameObject.AddComponent<SpringJoint>();
                    _joint.autoConfigureConnectedAnchor = false;
                    _joint.connectedAnchor = _hit.point;

                    _joint.maxDistance = distanceFromPoint * 0.7f;
                    _joint.minDistance = distanceFromPoint * 0.1f;
                    _joint.spring = 35f;
                    _joint.damper = 20f;

                    WebAS.pitch = Random.Range(0.85f, 1.15f);
                    WebAS.Play();

                    CancelInvoke("ReleaseWeb");
                    Invoke("ReleaseWeb", 2f);
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
            LineRenderer.SetPosition(0, Hand.transform.position + new Vector3(0, 0.15f, 0));
            LineRenderer.SetPosition(1, _hit.point);
        }
        else
        {
            LineRenderer.positionCount = 0;
        }
    }

    private void CheckForWater()
    {
        if (transform.position.y < -2)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void Grounded()
    {
        _isGrounded = true;
        _isRecovering = false;
    }

    private void StopClimbing()
    {
        ClimbingAS.Stop();
        _rigidbody.AddForce(-transform.forward * 1000);
        Animator.SetInteger("State", (int)SpiderManAnimationState.ClimbJump);
        _isClimbing = false;
        ClimbingObject = null;
        _isFalling = true;
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
    Swinging = 10,
    SwingingBothArms = 11
}