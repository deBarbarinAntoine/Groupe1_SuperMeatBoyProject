using System;
using UnityEngine;

public class SawBladeLeft : MonoBehaviour
{
    public float speed = 10f;
    public float detectionRadius = 0.4f; // Adjust based on saw size
    public LayerMask playerLayer; // Assign in Inspector
    public LayerMask wallLayer;
    public Sprite hitSprite; // Assign a different sprite in Inspector

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        transform.up = Vector2.up; 
        _rb.linearVelocity = -transform.right * speed; // Move left
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // Check for player in range manually
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player != null)
        {
            // Change the sprite
            if (hitSprite != null)
            {
                _spriteRenderer.sprite = hitSprite;
            }
            // Handle collision (damage player, destroy saw, etc.)
        }
        Collider2D wall = Physics2D.OverlapCircle(transform.position, detectionRadius, wallLayer);
        if (wall != null)
        {
            Destroy(gameObject); // Destroy on hitting a wall
        }
    }
    
}