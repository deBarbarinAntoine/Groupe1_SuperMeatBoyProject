    using System;
    using UnityEngine;

    public class SawBladeBottom : MonoBehaviour
    {
        public float speed = 20f;
        public float detectionRadius = 1f; // Adjust based on saw size
        public LayerMask playerLayer; // Assign in Inspector
        public LayerMask wallLayer;
        public Sprite hitSprite; // Assign a different sprite in Inspector

        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        // private void OnDrawGizmos()
        // {
        //     // Set the Gizmo color (optional)
        //     Gizmos.color = Color.red;
        //
        //     // Draw a sphere at the GameObject's position with a radius of 1
        //     Gizmos.DrawSphere(transform.position, detectionRadius);
        // }
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.linearVelocity = -transform.up * speed; // Move down
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