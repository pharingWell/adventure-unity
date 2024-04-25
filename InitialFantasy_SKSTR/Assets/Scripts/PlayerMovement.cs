using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float playerMoveSpeed = 5f;

    private float horizontal = 0.0f;
    private float vertical = 0.0f;
    private bool isFacingRight = true;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal") * playerMoveSpeed;
        vertical = Input.GetAxis("Vertical") * playerMoveSpeed;
        animator.SetFloat("PlayerXSpeed", Mathf.Abs(horizontal));
        animator.SetFloat("PlayerYSpeed", vertical);

        if ((isFacingRight && horizontal < 0.0f) || (!isFacingRight && horizontal > 0.0f)) {
            isFacingRight = !isFacingRight;
        }
        
        
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = rb.position + new Vector2(horizontal, vertical) * Time.fixedDeltaTime;

        // Check if the player's next position would collide with the water layer
        if (!Physics2D.OverlapCircle(nextPosition, 0.1f, waterLayer))
        {
            // If not colliding with water, move the player
            rb.velocity = new Vector2(horizontal, vertical);
        }
        else
        {
            // If colliding with water, stop the player's movement
            rb.velocity = Vector2.zero;
        }

        // Flip the player sprite if needed
        sr.flipX = !isFacingRight;
    }
}