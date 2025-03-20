using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player components
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    // References to other scripts
    LevelManager levelManager;

    // Controls the direction the player is facing and runs in
    public float horiz;
    public float jumpForce;
    public bool facingLeft = true, canFlip = true;
    float friction = 0.88f;
    float orientation;

    // Manages the speed of the player
    public float speed, groundSpeed, airSpeed, maxSpeed;

    // Raycasts
    float groundRayDistance = 1.4f, wallRayDistance = 0.6f;
    public LayerMask groundLayer;

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

    // Manages Crouching
    bool isCrouching;

    // Health/Damaging
    IEnumerator flashRoutine;
    MaterialPropertyBlock mpb;
    int health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim =  GetComponent<Animator>();
        mpb = new MaterialPropertyBlock();

        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();

        orientation = transform.localScale.x;
    }

    void Update()
    {
        Animations();
        Jump();
        Flip();
        Crouch();
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
            if (!isCrouching)
            {
                rb.AddForce(10f * horiz * speed * Vector2.right);

                // Limits the speed of the player
                if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
                {
                    rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
                }
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

    void Animations()
    {
        if (horiz != 0)
        {
            anim.SetBool("Running", true);
        }

        else
        {
            anim.SetBool("Running", false);
        }

        if (!isGrounded && rb.linearVelocity.y < -0.1)
        {
            anim.SetBool("Falling", true);
        }

        else
        {
            anim.SetBool("Falling", false);
        }

        if (wallSliding)
        {
            anim.SetBool("WallSliding", true);
        }

        else
        {
            anim.SetBool("WallSliding", false);
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
                anim.SetTrigger("Jump");
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
        yield return new WaitForSeconds(0.35f);
        wallJumped = false;
    }

    void Flip()
    {
        if (!wallSliding && canFlip)
        {
            if (horiz < 0)
            {
                facingLeft = true;
                transform.localScale = new Vector3(-orientation, transform.localScale.y, transform.localScale.z);
            }

            else if (horiz > 0)
            {
                facingLeft = false;
                transform.localScale = new Vector3(orientation, transform.localScale.y, transform.localScale.z);
            }
        }

        if (wallSliding)
        {
            sr.flipX = true;
        }

        else
        {
            sr.flipX = false;
        }
    }

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isGrounded)
            {
                isCrouching = true;
                anim.SetBool("Crouching", true);
            }
        }

        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || !isGrounded)
        {
            isCrouching = false;
            anim.SetBool("Crouching", false);
        }

        if (isCrouching)
        {
            friction = 0.6f;
        }

        else
        {
            friction = 0.88f;
        }
    }

    void SlideLogic()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (horiz != 0 && isGrounded && !isSliding)
            {
                isSliding = true;
                anim.SetBool("Sliding", true);
                slideDir = facingLeft ? 1 : -1;
                StartCoroutine(SlideCancel());
            }
        }
    }

    void Slide()
    {
        if (isSliding)
        {
            // Apply a continuous force while sliding
            rb.AddForce(new Vector2(-slideDir * slideSpeed, 0), ForceMode2D.Force);

            // Cap max slide speed to prevent acceleration
            if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
            }
        }
    }

    IEnumerator SlideCancel()
    {
        yield return new WaitForSeconds(0.4f);
        isSliding = false;
        anim.SetBool("Sliding", false);
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

        if (wallJumped || wallSliding)
        {
            speed = 0;
        }

        else
        {
            speed = 50;
        }
    }

    void WallJump()
    {
        if (wallSliding && !wallJumped)
        {
            wallJumped = true;
            wallSliding = false;
            anim.SetTrigger("Jump");

            wallJumpDir = facingLeft ? -1 : 1;
            rb.AddForce(new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y) * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(WallJumpDelay());           
            StartCoroutine(FlipDelay());

            facingLeft = !facingLeft;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    IEnumerator FlipDelay()
    {
        canFlip = false;
        yield return new WaitForSeconds(0.4f);
        canFlip = true;
    }

    bool WallCheck()
    {
        Vector2 rayDirection = facingLeft ? Vector2.right : Vector2.left;

        return Physics2D.CapsuleCast(transform.position, new Vector2(0.2f, 0.4f), CapsuleDirection2D.Vertical, 0, -rayDirection, wallRayDistance, groundLayer);
    }

    void GroundCheck()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, groundRayDistance, groundLayer))
        {
            isGrounded = true;
            anim.SetBool("Grounded", true);
            Debug.DrawRay(transform.position, Vector2.down * groundRayDistance, Color.green);
        }

        else
        {
            isGrounded = false;
            anim.SetBool("Grounded", false);
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

    public void TakeDamage()
    {
        FlashWhite();
    }

    IEnumerator Flash()
    {
        sr.material.SetInt("_Hit", 1);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 1);
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 1);
        yield return new WaitForSeconds(0.1f);
        sr.material.SetInt("_Hit", 0);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 0);
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 0);
    }

    public void FlashWhite()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(Flash());
        }

        flashRoutine = Flash();
        StartCoroutine(flashRoutine);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RightDoor") && levelManager.canLeave)
        {
            transform.position += Vector3.right * 3f;
            levelManager.currentCarriage++;
        }

        else if (collision.gameObject.CompareTag("LeftDoor"))
        {
            transform.position += Vector3.right * -3f;
            levelManager.currentCarriage--;
        }
    }
}