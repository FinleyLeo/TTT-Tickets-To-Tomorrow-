using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour
{
    SpriteRenderer sr;
    Animator anim;
    GameObject player;
    MaterialPropertyBlock mpb;
    CameraController cam;

    IEnumerator flashRoutine;

    public GameObject bullet;
    public GameObject shootPoint;
    public GameObject arm;
    public Animator gunAnim;

    float fadeTime = 2f, deadDelay = 0.25f, shootSpeed, aimSpeed = 3f, offset, flipValue;
    public float handicapMulti = 1;

    public int health;

    bool facingRight;
    public bool isDead, isActive, isAwake;
    bool playerSeen, isShooting;

    public Sprite normalSprite;  // Default front-facing sprite
    public Sprite upSprite;      // Sprite when aiming up
    public Sprite downSprite;    // Sprite when aiming down

    public LayerMask layers;

    Vector3 aimDirection;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        mpb = new MaterialPropertyBlock();
        cam = Camera.main.GetComponent<CameraController>();

        anim.enabled = false;

        flipValue = transform.localScale.x;
        shootSpeed = Random.Range(1f, 1.75f) - TimeManager.instance.currentLoop / 3;

        if (gameObject.name != "Dummy")
        {
            if (Random.Range(0f, 100f) > 99)
            {
                shootSpeed = 0.3f;

                sr.color = new Color(1, 0.4f, 0.4f, 1);

                for (int i = 0; i < arm.transform.childCount; i++)
                {
                    Transform child = arm.transform.GetChild(i);
                    child.GetComponent<SpriteRenderer>().color = new Color(1, 0.4f, 0.4f, 1);
                }

                health *= 3;
            }
        }

        shootSpeed = Mathf.Clamp(shootSpeed, 0.1f, 2f);
    }


    void Update()
    {
        if (!isDead)
        {
            if (isActive)
            {
                PlayerSeen();

                if (playerSeen)
                {
                    Orientation();
                    ArmAim();
                }
            }
        }

        else
        {
            anim.enabled = true;
            deadDelay -= Time.deltaTime;

            if (deadDelay <= 0)
            {
                sr.color = Color.Lerp(sr.color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);
                
                for (int i = 0; i < arm.transform.childCount; i++)
                {
                    Transform child = arm.transform.GetChild(i);
                    child.GetComponent<SpriteRenderer>().color = Color.Lerp(child.GetComponent<SpriteRenderer>().color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);
                }

                StartCoroutine(Death());
            }
        }

        handicapMulti = 1 - player.GetComponent<Rigidbody2D>().linearVelocity.magnitude / 30;
        handicapMulti = Mathf.Clamp(handicapMulti, 0.4f, 1);
    }

    void Orientation()
    {
        // Is to the left or right of the enemy
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-flipValue, transform.localScale.y, transform.localScale.z);
            facingRight = true;
        }

        else
        {
            transform.localScale = new Vector3(flipValue, transform.localScale.y, transform.localScale.z);
            facingRight = false;
        }
    }

    void ArmAim()
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        if (facingRight)
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle + offset), Time.deltaTime * aimSpeed * handicapMulti * TimeManager.instance.currentLoop / 2);
        }
        else
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle + 180 + offset), Time.deltaTime * aimSpeed * handicapMulti * TimeManager.instance.currentLoop / 2);
        }

        float armAngle = arm.transform.eulerAngles.z;

        // Normalize the angle (-180 to 180 range)
        if (armAngle > 180)
        {
            armAngle -= 360;
        }

        // Adjust logic when enemy is flipped
        if (!facingRight)
        {
            armAngle = -armAngle;
        }

        // Change enemy sprite based on arm's angle
        if (armAngle > 40)
        {
            sr.sprite = upSprite;
        }
        else if (armAngle < -40)
        {
            sr.sprite = downSprite;
        }
        else
        {
            sr.sprite = normalSprite;
        }
    }

    IEnumerator FlashEffect()
    {
        // Set flash effect to full white
        sr.GetPropertyBlock(mpb);

        for (int i = 0; i < arm.transform.childCount; i++)
        {
            Transform child = arm.transform.GetChild(i);

            child.GetComponent<SpriteRenderer>().GetPropertyBlock(mpb);
        }

        mpb.SetInt("_Hit", 1);

        sr.SetPropertyBlock(mpb);

        for (int i = 0; i < arm.transform.childCount; i++)
        {
            Transform child = arm.transform.GetChild(i);

            child.GetComponent<SpriteRenderer>().SetPropertyBlock(mpb);
        }

        yield return new WaitForSeconds(0.1f);

        // Reset back to normal
        sr.GetPropertyBlock(mpb);

        for (int i = 0; i < arm.transform.childCount; i++)
        {
            Transform child = arm.transform.GetChild(i);

            child.GetComponent<SpriteRenderer>().GetPropertyBlock(mpb);
        }

        mpb.SetInt("_Hit", 0);

        sr.SetPropertyBlock(mpb);

        for (int i = 0; i < arm.transform.childCount; i++)
        {
            Transform child = arm.transform.GetChild(i);

            child.GetComponent<SpriteRenderer>().SetPropertyBlock(mpb);
        }
    }

    public void FlashWhite()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = FlashEffect();
        StartCoroutine(flashRoutine);
    }

    public IEnumerator Awaken()
    {
        if (!isAwake)
        {
            isAwake = true;
            yield return new WaitForSeconds(0.5f);
            isActive = true;
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void TakeDamage()
    {
        Camera.main.GetComponent<CameraController>().Shake(0.5f, 0.1f, 0.1f);
        FlashWhite();

        AudioManager.instance.PlaySFXWithPitch("EnemyHit", 1 + TimeManager.instance.comboAmount / 2);

        health--;

        if (health <= 0)
        {
            isDead = true;
            GetComponent<CapsuleCollider2D>().enabled = false;
            anim.SetBool("Dead", true);
        }
    }

    IEnumerator Death()
    {
        tag = "Untagged";
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    public IEnumerator Shoot()
    {
        isShooting = true;
        offset = Random.Range(-7.5f, 7.5f);

        yield return new WaitForSeconds(0.5f);

        if (!isDead)
        {
            gunAnim.SetTrigger("Shoot");
            GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, -shootPoint.transform.rotation.eulerAngles.z));
            cam.Shake(0.25f, 0.1f, 0.1f);
            temp.tag = "Enemy";

            AudioManager.instance.PlaySFXWithPitch("Shoot", Random.Range(0.8f, 1.2f));

            yield return new WaitForSeconds(shootSpeed);

            if (!isDead && isActive && playerSeen)
            {
                StartCoroutine(Shoot());
            }

            else
            {
                isShooting = false;
            }
        }
    }

    void PlayerSeen()
    {
        int[] angles = { 0, -1, 1 }; // Middle ray first, then left, then right

        Vector2 dir = (player.transform.position - transform.position).normalized;
        float rayDistance = Vector2.Distance(transform.position, player.transform.position);

        RaycastHit2D hit;

        for (int i = 0; i < angles.Length; i++)
        {
            Vector2 rayDir = RotateVector(dir, i * 6);
            hit = Physics2D.Raycast(transform.position, rayDir, rayDistance, layers);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawRay(transform.position, rayDir * rayDistance, Color.green);
                    playerSeen = true;

                    aimDirection = rayDir;

                    if (!isShooting)
                    {
                        StartCoroutine(Shoot());
                    }

                    break;
                }

                else
                {
                    Debug.DrawRay(transform.position, rayDir * rayDistance, Color.red);
                    StopCoroutine(Shoot());
                    playerSeen = false;
                }
            }

            else
            {
                Debug.DrawRay(transform.position, rayDir * rayDistance, Color.white);
                StopCoroutine(Shoot());
                playerSeen = false;
            }
        }
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}
