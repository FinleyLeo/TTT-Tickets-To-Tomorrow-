using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public LayerMask groundLayer;

    // Controls the direction the player is facing and runs in
    float horiz, friction = 0.95f;
    public float jumpForce;
    bool facingRight = true;

    // Manages the speed of the player
    public float speed, groundSpeed, airSpeed, maxSpeed;

    // Raycast distances
    float groundRayDistance = 1.1f, wallRayDistance = 0.6f;

    // Manages Jumping
    float coyoteTime = 0.15f, coyoteTimeCounter;
    bool isGrounded, jumpPressed;

    // Manages wall sliding & jumping
    float wallJumpDir;
    public float wallSlideSpeed;
    bool wallSliding, wallJumped;
    public Vector2 wallJumpPower;

    // Manages Sliding values
    public float slideSpeed;
    float slideDir;
    bool isSliding;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Flip();
        SlideLogic();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        BasicMovement();
        WallSlide();
        Slide();
    }

    void BasicMovement()
    {
        if (!isSliding)
        {
            horiz = Input.GetAxisRaw("Horizontal");
        }

        if (horiz != 0)
        {
            rb.AddForce(10f * horiz * speed * Vector2.right);

            // Limits the speed of the player
            if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
            }
        }

        // Applies friction to the player when not moving
        else
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * friction, rb.linearVelocity.y);
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(JumpGrace());
        }

        if (jumpPressed)
        {
            if (isGrounded || coyoteTimeCounter > 0)
            {
                jumpPressed = false;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (wallSliding)
            {
                WallJump();
            }
        }
    }

    IEnumerator JumpGrace()
    {
        jumpPressed = true;
        yield return new WaitForSeconds(0.1f);
        jumpPressed = false;
    }

    IEnumerator WallJumpDelay()
    {
        yield return new WaitForSeconds(0.3f);
        wallJumped = false;
    }

    void Flip()
    {
        if (facingRight && horiz < 0 || !facingRight && horiz > 0)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void SlideLogic()
    {
        if (Input.GetKeyDown(KeyCode.S) && horiz != 0 && isGrounded && !isSliding)
        {
            isSliding = true;
            slideDir = facingRight ? 1 : -1;
            StartCoroutine(SlideCancel());
        }
    }

    void Slide()
    {
        if (isSliding)
        {
            // Apply a continuous force while sliding
            rb.AddForce(new Vector2(slideDir * slideSpeed, 0), ForceMode2D.Force);

            // Cap max slide speed to prevent acceleration
            if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed + 2)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed + 2, rb.linearVelocity.y);
            }
        }
    }

    IEnumerator SlideCancel()
    {
        yield return new WaitForSeconds(0.3f);
        isSliding = false;
    }

    void WallSlide()
    {
        if (!isGrounded && WallCheck() && horiz != 0)
        {
            wallSliding = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            wallSliding = false;
        }
    }

    void WallJump()
    {
        if (wallSliding && !wallJumped)
        {
            wallJumpDir = facingRight ? -1 : 1;
            rb.AddForce(new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y) * jumpForce, ForceMode2D.Impulse);
            wallJumped = true;
            StartCoroutine(WallJumpDelay());
        }
    }

    bool WallCheck()
    {
        Vector2 rayDirection = facingRight ? Vector2.right : Vector2.left;

        return Physics2D.Raycast(transform.position, rayDirection, wallRayDistance, groundLayer);
    }

    void GroundCheck()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, groundRayDistance, groundLayer))
        {
            isGrounded = true;
            Debug.DrawRay(transform.position, Vector2.down * groundRayDistance, Color.green);
        }

        else
        {
            isGrounded = false;
            Debug.DrawRay(transform.position, Vector2.down * groundRayDistance, Color.white);
        }

        if (isGrounded)
        {
            maxSpeed = groundSpeed;
            coyoteTimeCounter = coyoteTime;
        }

        else
        {
            maxSpeed = airSpeed;
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
}
