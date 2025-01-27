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

    private void Awake()
    {
        _controls = new PlayerControls();

        // Bind the controls
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _controls.Player.Jump.performed += _ => Jump();
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
    }

    private void Move()
    {
        float moveSpeed = isRunning ? speed * 1.5f : speed;
        _rb.linearVelocity = new Vector2(_moveInput.x * moveSpeed, _rb.linearVelocity.y);
    }

    private void Jump()
    {
        if (IsGrounded()) // Add a ground check method
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
    }

    private bool IsGrounded()
    {
        // Add logic for ground detection (e.g., raycast or collision check)
        return true;
    }
}