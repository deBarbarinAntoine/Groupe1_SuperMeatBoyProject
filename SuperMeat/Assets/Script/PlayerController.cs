using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 1f;
    public float deceleration = 2f;
    public float maxSpeed = 10f;
    public float jumpForce = 10f;
    public bool isRunning;

    public LayerMask wallLayer;
    public LayerMask groundLayer;

    public float wallSlideSpeed = 2f;
    public float wallClingTime = 1f;

    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;

    private Collider2D _collider;
    private InputSystem_Actions _controls;
    private float _coyoteTimeCounter;
    private bool _isTouchingWall;
    private bool _isTouchingWallLeft;
    private bool _isTouchingWallRight;
    private bool _isWallClinging;
    private float _jumpBufferCounter;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private float _wallClingCounter;
    private bool _wasGroundedLastFrame;
    private float _wallAscendCounter;
    public float wallAscendTime = 0.2f; // Adjust duration to tweak how long they rise before clinging
    private float _moveInputTimer = 0f;

    private void Awake()
    {
        _controls = new InputSystem_Actions();
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => _moveInput = Vector2.zero;
        _controls.Player.Jump.performed += _ => BufferJump();
        _controls.Player.Sprint.performed += _ => isRunning = true;
        _controls.Player.Sprint.canceled += _ => isRunning = false;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        var isGrounded = IsGrounded();
        Move();
        CheckWallCling();
        UpdateCoyoteTime(isGrounded);
        UpdateJumpBuffer(isGrounded);

        _wasGroundedLastFrame = isGrounded;
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Move()
    {
        if (_isWallClinging) return;

        var targetSpeed = Mathf.Clamp(_moveInput.x * (isRunning ? maxSpeed * 1.5f : maxSpeed), -maxSpeed * 1.5f,
            maxSpeed * 1.5f);
        var speedDifference = targetSpeed - _rb.linearVelocity.x;
        var accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        var movement = Mathf.Lerp(_rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);

        _rb.linearVelocity = new Vector2(movement, _rb.linearVelocity.y);
    }

    private void BufferJump()
    {
        _jumpBufferCounter = jumpBufferTime;
        CheckJump();
    }

    private void CheckJump()
    {
        if (_jumpBufferCounter > 0)
        {
            if (_coyoteTimeCounter > 0 || IsAtWallTop())
                Jump(Vector2.up);
            else if (_isWallClinging) WallJump();
            _jumpBufferCounter = 0;
        }
    }

    private void Jump(Vector2 direction)
    {
        // Reset vertical velocity before applying jump force
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);

        // Check if character is running and apply double jump force
        float finalJumpForce = (isRunning) ? jumpForce * 2 : jumpForce;

        // Apply jump force with the calculated finalJumpForce
        _rb.linearVelocity += direction * finalJumpForce;

        // Reset coyote time and jump buffer counters
        _coyoteTimeCounter = 0;
        _jumpBufferCounter = 0;
    }

    private void WallJump()
    {
        float jumpDirection = _isTouchingWallRight ? -1 : 1;
        var jumpVector = new Vector2(jumpDirection, 2f).normalized;

        Jump(jumpVector);
        _isWallClinging = false;
        _wallAscendCounter = wallAscendTime; // Add ascent time before clinging
    }

    private void CheckWallCling()
    {
        _isTouchingWallLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.6f, wallLayer);
        _isTouchingWallRight = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, wallLayer);

        _isTouchingWall = _isTouchingWallLeft || _isTouchingWallRight;

        // Decrease ascent counter
        if (_wallAscendCounter > 0)
        {
            _wallAscendCounter -= Time.deltaTime;
            return; // Prevent clinging until ascent time is over
        }
        
        // Track how long _moveInput.x is non-zero
        if (_moveInput.x != 0)
        {
            _moveInputTimer += Time.deltaTime;
        }
        else
        {
            _moveInputTimer = 0f; // Reset timer when _moveInput.x is zero
        }

        // Cancel wall cling if moving away from the wall
        if (_moveInputTimer >= 0.5f && ((_isTouchingWallLeft && _moveInput.x > 0) || (_isTouchingWallRight && _moveInput.x < 0)))
        {
            _isWallClinging = false;
            return;
        }

        if (_isTouchingWall && !IsGrounded())
        {
            if (_moveInput.x != 0) _wallClingCounter = wallClingTime;

            if (_wallClingCounter > 0)
            {
                _isWallClinging = true;
                WallSlide();
            }
        }
        else
        {
            _isWallClinging = false;
        }

        if (_isWallClinging && _moveInput.x != 0) _wallClingCounter -= Time.deltaTime;
    }



    private bool IsAtWallTop()
    {
        var checkDistance = 1f; // Small distance to detect the wall edge
        bool isWallBelow = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, wallLayer);
        bool isGroundBelow = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, groundLayer);
        return isWallBelow && !isGroundBelow;
    }


    private void WallSlide()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x,
            Mathf.Clamp(_rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
    }

    private void UpdateCoyoteTime(bool isGrounded)
    {
        _coyoteTimeCounter = isGrounded ? coyoteTime : _coyoteTimeCounter - Time.deltaTime;
    }

    private void UpdateJumpBuffer(bool isGrounded)
    {
        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;

            if (isGrounded && !_wasGroundedLastFrame) CheckJump();
        }
    }

    private bool IsGrounded()
    {
        return _collider.IsTouchingLayers(groundLayer);
    }
}