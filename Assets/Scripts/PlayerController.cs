using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Player components
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    PlayerAim aim;

    // References to other scripts
    LevelManager levelManager;

    // Controls the direction the player is facing and runs in
    public float horiz;
    public bool facingLeft = true, canFlip = true;
    public float defaultFriction = 0.8f;
    float friction;

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
    public bool invincible;
    Transform spawnPoint;
    public bool isDead;

    IEnumerator flashRoutine;
    MaterialPropertyBlock mpb;

    // Visuals
    public ParticleSystem Dust;
    Image ammoBase;
    Animator ammoAnim;
    public Sprite[] ammoSprites;
    ParticleSystem ammoShatter;

    public Volume volume;
    ChromaticAberration aberration;
    LensDistortion distortion;

    GameObject ammoObj;

    // Interactions
    bool canPickupWatch, canTurnValve;
    GameObject watch;
    Animator valveAnim;

    // Dialogue

    public Dialogue dialogue;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim =  GetComponent<Animator>();
        aim = GetComponent<PlayerAim>();
        mpb = new MaterialPropertyBlock();
        
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        dialogue = GameObject.Find("Dialogue box").GetComponent<Dialogue>();

        TimeManager.instance.timeLoss = 1;

        spawnPoint = GameObject.Find("Spawn Point").transform;
        spawnPoint.position = transform.position;

        Cursor.lockState = CursorLockMode.Confined;

        volume.profile.TryGet(out ChromaticAberration ab);
        {
            aberration = ab;
        }

        volume.profile.TryGet(out LensDistortion lensD);
        {
            distortion = lensD;
        }
    }

    void Update()
    {
        Animations();
        Jump();
        Flip();
        Crouch();
        SlideLogic();
        GroundCheck();
        RewindVisuals();
        Interacting();
        UIFade();

        if (ammoBase == null || ammoShatter == null)
        {
            ammoObj = GameObject.Find("Ammo");

            ammoBase = ammoObj.transform.GetChild(0).GetComponent<Image>();
            ammoShatter = ammoObj.GetComponentInChildren<ParticleSystem>();
            ammoAnim = ammoObj.GetComponent<Animator>();
        }
    }

    private void FixedUpdate()
    {
        BasicMovement();
        WallSlide();
        Slide();
    }

    void Interacting()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canPickupWatch && SceneManager.GetActiveScene().name == "Tutorial")
            {
                AudioManager.instance.PlaySFX("WatchPickup");
                TimeManager.instance.hasWatch = true;
                watch.SetActive(false);
            }

            if (canTurnValve)
            {
                AudioManager.instance.PlaySFX("ValveTurn");
                valveAnim.SetTrigger("Turn");


                if (SceneManager.GetActiveScene().name == "Tutorial")
                {
                    UIScript.instance.RestartValues();

                    TimeManager.instance.inDialogue = false;
                    TimeManager.instance.tutorialComplete = true;
                    TimeManager.instance.saveExists = true;
                    TimeManager.instance.hasWatch = true;

                    SceneSwitcher.instance.Transition("Loop1");
                }

                if (SceneManager.GetActiveScene().name == "PreBoss")
                {
                    // Win game visuals
                    print("Game finished");
                    TimeManager.instance.gameEnded = true;
                }

                if (TimeManager.instance.currentLoop == 4)
                {
                    TimeManager.instance.currentLoop++;

                    int temp = TimeManager.instance.currentLoop;

                    // not decided
                    UIScript.instance.RestartValues();

                    TimeManager.instance.inDialogue = false;
                    TimeManager.instance.tutorialComplete = true;
                    TimeManager.instance.saveExists = true;
                    TimeManager.instance.hasWatch = true;
                    TimeManager.instance.currentLoop = temp;

                    TimeManager.instance.SaveValues();

                    SceneSwitcher.instance.Transition("PreBoss");
                }

                else if (TimeManager.instance.currentLoop < 4)
                {
                    TimeManager.instance.currentLoop++;

                    int temp = TimeManager.instance.currentLoop;

                    // not decided
                    UIScript.instance.RestartValues();

                    TimeManager.instance.inDialogue = false;
                    TimeManager.instance.tutorialComplete = true;
                    TimeManager.instance.saveExists = true;
                    TimeManager.instance.hasWatch = true;
                    TimeManager.instance.currentLoop = temp;

                    TimeManager.instance.SaveValues();

                    SceneSwitcher.instance.Transition("Loop1");
                }
            }
        }
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
            if (wallSliding)
            {
                AudioManager.instance.StopSFX();
                wallSliding = false;
            }
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
            AudioManager.instance.StopSFX();
            AudioManager.instance.PlaySFX("PlayerJump");
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
            TimeManager.instance.health--;

            if (TimeManager.instance.health <= 0)
            {
                // Game Over
                isDead = true;
                StartCoroutine(Rewind());
            }

            Camera.main.GetComponent<CameraController>().Shake(1.5f, 0.1f, 0.2f);
            StartCoroutine(InvincibilityEffect(1.5f, 0.1f));
            FlashWhite();

            AudioManager.instance.PlaySFX("PlayerHit");

            switch (TimeManager.instance.health)
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
        TimeManager.instance.timeLoss = 15;
        Time.timeScale = 0;
        AudioManager.instance.musicSource.Stop();
        AudioManager.instance.loopingSFX.Stop();
        AudioManager.instance.PlaySFX("PlayerDeath");

        yield return new WaitForSecondsRealtime(0.6f);

        Shader.SetGlobalFloat("_isAffected", 1);
        ammoAnim.SetTrigger("Break");

        yield return new WaitForSecondsRealtime(0.4f);

        AudioManager.instance.PlaySFX("PlayerRewind");
        

        yield return new WaitForSecondsRealtime(1f);

        TimeManager.instance.isRewinding = false;

        yield return new WaitForSecondsRealtime(0.3f);

        Respawn();
    }

    void Respawn()
    {
        Time.timeScale = 1;
        TimeManager.instance.timeLoss = 1;
        Shader.SetGlobalFloat("_isAffected", 0);
        AudioManager.instance.musicSource.Play();
        AudioManager.instance.loopingSFX.Play();

        levelManager.RespawnEnemies();
        ammoAnim.SetTrigger("Shake");

        transform.position = spawnPoint.position;
        TimeManager.instance.health = 5;
        aim.ammo = 6;

        if (SceneManager.GetActiveScene().name == "Loop1")
        {
            aim.ammoAnim.Play("AmmoReload");
        }

        isDead = false;

        switch (TimeManager.instance.health)
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

    void RewindVisuals()
    {
        if (isDead)
        {
            if (TimeManager.instance.isRewinding && Time.timeScale < 0.05f)
            {
                if (aberration != null)
                {
                    aberration.intensity.value = Mathf.Lerp(aberration.intensity.value, 0.5f, Time.unscaledDeltaTime * 2f);
                }

                if (distortion != null)
                {
                    distortion.intensity.value = Mathf.Lerp(distortion.intensity.value, -0.5f, Time.unscaledDeltaTime * 2f);
                }
            }

            else
            {
                if (aberration != null)
                {
                    aberration.intensity.value = Mathf.Lerp(aberration.intensity.value, 0, Time.unscaledDeltaTime * 4f);
                }

                if (distortion != null)
                {
                    distortion.intensity.value = Mathf.Lerp(distortion.intensity.value, 0, Time.unscaledDeltaTime * 4f);
                }
            }
        }

        else
        {
            aberration.intensity.value = 0;
            distortion.intensity.value = 0;
        }
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

            TimeManager.instance.carriagesPassed++;
            TimeManager.instance.sinceRefill++;

            spawnPoint.position = transform.position;
            DialogueCheck();
        }

        else if (collision.gameObject.CompareTag("LeftDoor") && levelManager.canLeave)
        {
            transform.position += Vector3.right * -3f;
            levelManager.DeactivateEnemies();
            levelManager.currentCarriage--;
            levelManager.FollowLogic();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Watch"))
        {
            canPickupWatch = true;

            watch = collision.gameObject;
            collision.GetComponentInChildren<Animator>().SetBool("Active", true);
        }

        if (collision.gameObject.CompareTag("Refill"))
        {
            collision.GetComponent<CircleCollider2D>().enabled = false;
            Light2D light = collision.GetComponentInChildren<Light2D>();

            light.color = Color.cyan;
            light.intensity = 30;

            FlashWhite();

            TimeManager.instance.timeLeft += 120f;
            TimeManager.instance.health = 5;

            switch (TimeManager.instance.health)
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

        if (collision.gameObject.CompareTag("Space"))
        {
            AudioManager.instance.inSpace = true;
            TimeManager.instance.timeLoss = 0;
            Physics2D.gravity /= 2;
        }

        if (collision.gameObject.CompareTag("Valve"))
        {
            valveAnim = collision.GetComponent<Animator>();
            collision.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", true);
            canTurnValve = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Watch"))
        {
            canPickupWatch = false;

            collision.GetComponentInChildren<Animator>().SetBool("Active", false);
        }

        if (collision.gameObject.CompareTag("Valve"))
        {
            collision.transform.GetChild(0).GetComponent<Animator>().SetBool("Active", false);
            canTurnValve = false;
        }

        if (collision.gameObject.CompareTag("Space"))
        {
            AudioManager.instance.inSpace = false;
            TimeManager.instance.timeLoss = 1;
            Physics2D.gravity *= 2;
        }
    }

    public void PlayAnimSound(string sound)
    {
        AudioManager.instance.PlaySFX(sound);
    }

    public void Footsteps()
    {
        int ran = Random.Range(0, 3);

        switch (ran)
        {
            case 0:
                AudioManager.instance.PlaySFX("Step1");
                break;
            case 1:
                AudioManager.instance.PlaySFX("Step2");
                break;
            case 2:
                AudioManager.instance.PlaySFX("Step3");
                break;
            case 3:
                AudioManager.instance.PlaySFX("Step4");
                break;
        }
    }

    void DialogueCheck()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            switch (TimeManager.instance.carriagesPassed)
            {
                case 1:
                    dialogue.StartDialogue(5, 6);
                    break;
                case 2:
                    dialogue.StartDialogue(7, 9);
                    break;
                case 3:
                    dialogue.StartDialogue(10, 11);
                    break;
                case 4:
                    dialogue.StartDialogue(12, 13);
                    break;
                case 5:
                    dialogue.StartDialogue(14, 18);
                    break;
                case 6:
                    dialogue.StartDialogue(19, 20);
                    break;
                case 7:
                    dialogue.StartDialogue(21, 24);
                    break;
            }
        }
    }

    void UIFade()
    {
        if (SceneManager.GetActiveScene().name == "PreBoss")
        {
            if (AudioManager.instance.inSpace)
            {
                if (ammoObj != null)
                {
                    ammoObj.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(ammoObj.GetComponent<CanvasGroup>().alpha, 0, Time.unscaledDeltaTime * 4f);
                }

                if (TimeManager.instance.timeObj != null)
                {
                    TimeManager.instance.timeObj.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(TimeManager.instance.timeObj.GetComponent<CanvasGroup>().alpha, 0, Time.unscaledDeltaTime * 4f);
                }
            }

            else
            {
                if (ammoObj != null)
                {
                    ammoObj.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(ammoObj.GetComponent<CanvasGroup>().alpha, 1, Time.unscaledDeltaTime * 4f);
                }

                if (TimeManager.instance.timeObj != null)
                {
                    TimeManager.instance.timeObj.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(TimeManager.instance.timeObj.GetComponent<CanvasGroup>().alpha, 1, Time.unscaledDeltaTime * 4f);
                }
            }
        }
    }
}