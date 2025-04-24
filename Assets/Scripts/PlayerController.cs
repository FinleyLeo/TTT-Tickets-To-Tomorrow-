using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public bool facingLeft = true, canFlip = true;
    public float defaultFriction = 0.8f;
    float friction;
    float orientation;

    // Manages the speed of the player
    public float speed, groundSpeed, airSpeed, maxSpeed;

    // Raycasts
    float groundRayDistance = 1.4f, wallRayDistance = 0.6f;
    public LayerMask groundLayer;

    // Manages Jumping
    public float jumpForce;
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
    public bool isSliding;

    // Manages Crouching
    bool isCrouching;
    bool onPlatform;

    // Health/Damaging
    [SerializeField] int health = 5;
    public bool invincible;
    Transform spawnPoint;

    IEnumerator flashRoutine;
    MaterialPropertyBlock mpb;

    // Visuals
    public ParticleSystem Dust;
    Image ammoBase;
    Animator ammoAnim;
    public Sprite[] ammoSprites;
    ParticleSystem ammoShatter;

    public bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim =  GetComponent<Animator>();
        mpb = new MaterialPropertyBlock();
        
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        ammoBase = GameObject.Find("Ammo").transform.GetChild(0).GetComponent<Image>();
        ammoAnim = GameObject.Find("Ammo").GetComponent<Animator>();
        ammoShatter = GameObject.Find("Ammo").GetComponentInChildren<ParticleSystem>();

        TimeManager.instance.timeLoss = 1;

        orientation = transform.localScale.x;
        spawnPoint = GameObject.Find("Spawn Point").transform;
        spawnPoint.position = transform.position;

        Cursor.lockState = CursorLockMode.Confined;
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
        horiz = Input.GetAxisRaw("Horizontal");

        if (horiz != 0 && !isSliding && !isCrouching)
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

    IEnumerator PlatformFall()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        yield return new WaitForSeconds(0.75f);
        Physics2D.IgnoreLayerCollision(8, 9, false);
    }

    void Flip()
    {
        if (!wallSliding && canFlip)
        {
            if (horiz < 0)
            {
                facingLeft = true;
                transform.rotation = Quaternion.Euler(0, 180, 0);
                //transform.localScale = new Vector3(-orientation, transform.localScale.y, transform.localScale.z);
            }

            else if (horiz > 0)
            {
                facingLeft = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                //transform.localScale = new Vector3(orientation, transform.localScale.y, transform.localScale.z);
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
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (isGrounded && !isSliding && horiz == 0)
            {
                isCrouching = true;
                anim.SetBool("Crouching", true);
            }

            if (onPlatform)
            {
                StartCoroutine(PlatformFall());
            }
        }

        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow) || !isGrounded)
        {
            isCrouching = false;
            anim.SetBool("Crouching", false);
        }

        if (isCrouching)
        {
            friction = defaultFriction * 0.75f;
        }

        else
        {
            friction = defaultFriction;
        }
    }

    void SlideLogic()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (horiz != 0 && isGrounded && !isSliding)
            {
                isSliding = true;
                canFlip = false;
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
            Dust.emissionRate = 40;
            // Apply a continuous force while sliding
            rb.AddForce(new Vector2(-slideDir * slideSpeed, 0), ForceMode2D.Force);

            // Cap max slide speed to prevent acceleration
            if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
            }
        }

        else
        {
            Dust.emissionRate = 0;
        }
    }

    IEnumerator SlideCancel()
    {
        yield return new WaitForSeconds(0.4f);
        isSliding = false;
        canFlip = true;
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
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
            rb.AddForce(new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y) * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(WallJumpDelay());           
            StartCoroutine(FlipDelay());

            facingLeft = !facingLeft;

            transform.rotation = Quaternion.Euler(0, facingLeft ? 180 : 0, 0);
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
        if (!invincible)
        {
            health--;

            if (health <= 0)
            {
                // Game Over
                isDead = true;
                StartCoroutine(Rewind());
            }

            Camera.main.GetComponent<CameraController>().Shake(1.5f, 0.1f, 0.2f);
            StartCoroutine(InvincibilityEffect(1.5f, 0.1f));
            FlashWhite();

            switch (health)
            {
                case 5:
                    ammoBase.sprite = ammoSprites[0];
                    break;
                case 4:
                    ammoBase.sprite = ammoSprites[1];
                    break;
                case 3:
                    ammoBase.sprite = ammoSprites[2];
                    break;
                case 2:
                    ammoBase.sprite = ammoSprites[3];
                    break;
                case 1:
                    ammoBase.sprite = ammoSprites[4];
                    break;
            }

            ammoAnim.SetTrigger("Shake");
            ammoShatter.Play();
        }
    }

    IEnumerator Rewind()
    {
        TimeManager.instance.isRewinding = true;
        Time.timeScale = 0;
        Shader.SetGlobalFloat("_isAffected", 1);
        ammoAnim.SetTrigger("Break");
        yield return new WaitForSecondsRealtime(1);
        

        for (float i = ammoShatter.time; i > 0; i -= 0.1f)
        {
            ammoShatter.Simulate(i, false, true);
            Debug.Log(ammoShatter.time);
        }

        yield return new WaitForSecondsRealtime(1.3f);
        Respawn();
    }

    void Respawn()
    {
        Time.timeScale = 1;
        TimeManager.instance.timeLeft -= 20f;
        TimeManager.instance.isRewinding = false;
        Shader.SetGlobalFloat("_isAffected", 0);
        ammoAnim.SetTrigger("Shake");

        transform.position = spawnPoint.position;
        health = 5;
        isDead = false;

        switch (health)
        {
            case 5:
                ammoBase.sprite = ammoSprites[0];
                break;
            case 4:
                ammoBase.sprite = ammoSprites[1];
                break;
            case 3:
                ammoBase.sprite = ammoSprites[2];
                break;
            case 2:
                ammoBase.sprite = ammoSprites[3];
                break;
            case 1:
                ammoBase.sprite = ammoSprites[4];
                break;
        }
    }

    IEnumerator InvincibilityEffect(float duration, float flashSpeed)
    {
        invincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        SpriteRenderer armSr = transform.GetChild(1).GetComponent<SpriteRenderer>();
        SpriteRenderer gunSr = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;
        bool isVisible = true;

        while (elapsedTime < duration)
        {
            isVisible = !isVisible;
            sr.enabled = isVisible;
            armSr.enabled = isVisible;
            gunSr.enabled = isVisible;
            elapsedTime += flashSpeed;
            yield return new WaitForSeconds(flashSpeed);
        }

        sr.enabled = true;
        armSr.enabled = true;
        gunSr.enabled = true;
        invincible = false;
    }

    IEnumerator Flash()
    {
        sr.material.SetInt("_Hit", 1);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 1);
        transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 1);
        yield return new WaitForSeconds(0.1f);
        sr.material.SetInt("_Hit", 0);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 0);
        transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().material.SetInt("_Hit", 0);
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

    public void DustEffect()
    {
        Dust.Play();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            onPlatform = true;
        }

        else
        {
            onPlatform = false;
        }  
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RightDoor") && levelManager.canLeave)
        {
            transform.position += Vector3.right * 3f;
            levelManager.currentCarriage++;
            levelManager.FollowLogic();
            levelManager.ActivateEnemies();

            spawnPoint.position = transform.position;
        }

        else if (collision.gameObject.CompareTag("LeftDoor") && levelManager.canLeave)
        {
            transform.position += Vector3.right * -3f;
            levelManager.DeactivateEnemies();
            levelManager.currentCarriage--;
            levelManager.FollowLogic();
        }
    }
}