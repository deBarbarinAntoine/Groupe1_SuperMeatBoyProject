using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls _controls;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool isRunning;

    // Wall mechanics
    public LayerMask wallLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;
    public float wallSlideSpeed = 2f;
    private bool _isTouchingWall;
    private bool _isWallClinging;

    // Ground check variables
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    // Coyote time
    public float coyoteTime = 0.2f;
    private float _coyoteTimeCounter;

    // Jump buffer
    public float jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;

    private void Awake()
    {
        _controls = new PlayerControls();

        // Bind the controls
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _controls.Player.Jump.performed += _ => BufferJump();
        _controls.Player.Run.performed += _ => isRunning = true;
        _controls.Player.Run.canceled += _ => isRunning = false;
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
        CheckWallCling();
    }

    private void Update()
    {
        UpdateCoyoteTime();
        UpdateJumpBuffer();
        CheckJump();
    }

    private void Move()
    {
        if (_isWallClinging) return; // Disable normal movement while clinging to a wall

        float moveSpeed = isRunning ? speed * 1.5f : speed;
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }

    private void BufferJump()
    {
        _jumpBufferCounter = jumpBufferTime;
    }

    private void CheckJump()
    {
        if (_jumpBufferCounter > 0)
        {
            if (_coyoteTimeCounter > 0)
            {
                Jump(Vector2.up);
                _jumpBufferCounter = 0; // Clear the buffer
            }
            else if (_isWallClinging)
            {
                WallJump();
                _jumpBufferCounter = 0;
            }
        }
    }

    private void Jump(Vector2 direction)
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0); // Reset vertical velocity
        _rb.linearVelocity += direction * jumpForce;
        _coyoteTimeCounter = 0; // Reset coyote time
    }

    private void WallJump()
    {
        Vector2 jumpDirection = new Vector2(-Mathf.Sign(_moveInput.x), 1).normalized; // Away from the wall
        Jump(jumpDirection);
        _isWallClinging = false; // Stop clinging after jumping
    }

    private void CheckWallCling()
    {
        _isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * Mathf.Sign(_moveInput.x), wallCheckDistance, wallLayer);

        if (_isTouchingWall && !IsGrounded() && _moveInput.x != 0)
        {
            _isWallClinging = true;
            WallSlide();
        }
        else
        {
            _isWallClinging = false;
        }
    }

    private void WallSlide()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Clamp(_rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
    }

    private void UpdateCoyoteTime()
    {
        if (IsGrounded())
        {
            _coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void UpdateJumpBuffer()
    {
        if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground and wall checks
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * Mathf.Sign(_moveInput.x) * wallCheckDistance);
        }
    }
}
