using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool isRunning;

    // Layers
    public LayerMask wallLayer;
    public LayerMask groundLayer;

    // Wall mechanics
    public float wallSlideSpeed = 2f;

    // Coyote time
    public float coyoteTime = 0.2f;

    // Jump buffer
    public float jumpBufferTime = 0.2f;
    private Collider2D _collider;
    private InputSystem_Actions _controls;
    private float _coyoteTimeCounter;
    private bool _isTouchingWall;
    private bool _isWallClinging;
    private float _jumpBufferCounter;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _controls = new InputSystem_Actions();

        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

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
        Move();
        CheckWallCling();
        UpdateCoyoteTime();
        UpdateJumpBuffer();
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
        if (_isWallClinging) return; // Disable normal movement while clinging to a wall

        var moveSpeed = isRunning ? speed * 1.5f : speed;
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }

    private void BufferJump()
    {
        _jumpBufferCounter = jumpBufferTime;
        Debug.Log("Jump buffered!"); // Log when jump is buffered
        CheckJump();
    }

    private void CheckJump()
    {
        Debug.Log("Checking Jump... Buffer: " + _jumpBufferCounter + " | Coyote Time: " + _coyoteTimeCounter + " | Wall Cling: " + _isWallClinging);

        if (_jumpBufferCounter > 0)
        {
            if (_coyoteTimeCounter > 0) // Ground Jump
            {
                Debug.Log("Ground Jump Triggered!");
                Jump(Vector2.up);
                _jumpBufferCounter = 0;
            }
            else if (_isWallClinging) // Wall Jump
            {
                Debug.Log("Wall Jump Triggered!");
                WallJump();
                _jumpBufferCounter = 0;
            }
        }
    }

    private void Jump(Vector2 direction)
    {
        Debug.Log("Jump executed with direction: " + direction);
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0); // Force reset vertical velocity
        _rb.linearVelocity += direction * jumpForce;
        _coyoteTimeCounter = 0; // Reset coyote time
    }


    private void WallJump()
    {
        var jumpDirection = new Vector2(-Mathf.Sign(_moveInput.x), 1).normalized; // Away from the wall
        Debug.Log("Wall Jump executed in direction: " + jumpDirection);
        Jump(jumpDirection);
        _isWallClinging = false; // Stop clinging after jumping
    }

    private void CheckWallCling()
    {
        _isTouchingWall = _collider.IsTouchingLayers(wallLayer);

        if (_isTouchingWall && !IsGrounded() && _moveInput.x != 0)
        {
            if (!_isWallClinging) Debug.Log("Started Wall Cling!");
            _isWallClinging = true;
            WallSlide();
        }
        else
        {
            if (_isWallClinging) Debug.Log("Stopped Wall Cling!");
            _isWallClinging = false;
        }
    }

    private void WallSlide()
    {
        if (_rb.linearVelocity.y < -wallSlideSpeed) Debug.Log("Wall Sliding...");
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x,
            Mathf.Clamp(_rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
    }

    private void UpdateCoyoteTime()
    {
        bool grounded = IsGrounded();
        if (grounded)
        {
            if (_coyoteTimeCounter <= 0) Debug.Log("Coyote Time Reset!");
            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
        Debug.Log("Grounded: " + grounded + " | Coyote Time: " + _coyoteTimeCounter);
    }

    private void UpdateJumpBuffer()
    {
        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
            CheckJump(); // Ensure CheckJump is called while jump is buffered
        }
    }


    private bool IsGrounded()
    {
        return _collider.IsTouchingLayers(groundLayer);
    }
}