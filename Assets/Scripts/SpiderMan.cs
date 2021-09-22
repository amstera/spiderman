using UnityEngine;

public class SpiderMan : MonoBehaviour
{
    public float RunSpeed = 5;
    public float SprintingSpeed = 10;
    public float RotationSpeed = 15;

    public Animator Animator;

    private Rigidbody _rigidbody;

    private bool _isMoving;
    private bool _isClimbing;
    private bool _isGrounded = true;

    private Vector3 _moveDirection;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_isGrounded && collision.collider.tag == "Climbable")
        {
            _isClimbing = true;
            _isGrounded = false;
        }
    }

    private void HandleMovement()
    {
        _moveDirection = Camera.main.transform.forward * Input.GetAxisRaw("Vertical");
        _moveDirection += Camera.main.transform.right * Input.GetAxisRaw("Horizontal");
        _moveDirection.Normalize();
        _moveDirection.y = 0;

        if (_isClimbing)
        {
            _moveDirection.x = -Input.GetAxisRaw("Horizontal");
            _moveDirection.y = Input.GetAxisRaw("Vertical");
            _moveDirection.z = 0;
            _moveDirection.Normalize();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleJumping();
        }

        _isMoving = Mathf.Abs(_moveDirection.magnitude) > 0;
        if (_isMoving)
        {
            if (Input.GetKey(KeyCode.LeftShift))
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

        Vector3 movementVelocity = _moveDirection;
        _rigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if (_isClimbing)
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

    private void HandleJumping()
    {
        if (_isGrounded)
        {
            Animator.SetInteger("State", _isMoving ? (int)SpiderManAnimationState.RunningJump : (int)SpiderManAnimationState.Jump);
            _isGrounded = false;
           Invoke("Land", 0.75f);
        }
        else if (_isClimbing)
        {
            //todo: push off wall and show jump animation then on landing, change isGrounded
            _isClimbing = false;
        }
    }

    private void Land()
    {
        _isGrounded = true;
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
    ClimbingIdle = 6
}