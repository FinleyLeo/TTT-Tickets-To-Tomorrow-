using System.Collections;
using UnityEngine;

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

    float fadeTime = 2f, deadDelay = 0.25f, shootSpeed, aimSpeed, offset, flipValue;

    [SerializeField] int health = 1;

    bool facingRight;
    public bool isDead, isActive, isAwake;

    public Sprite normalSprite;  // Default front-facing sprite
    public Sprite upSprite;      // Sprite when aiming up
    public Sprite downSprite;    // Sprite when aiming down

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        mpb = new MaterialPropertyBlock();
        cam = Camera.main.GetComponent<CameraController>();

        anim.enabled = false;
        isActive = false;

        flipValue = transform.localScale.x;

        shootSpeed = Random.Range(0.75f, 1.75f);
        aimSpeed = Random.Range(2.5f, 3.5f);

        if (Random.Range(0, 100) > 90)
        {
            shootSpeed *= 2;
        }
    }


    void Update()
    {
        if (!isDead)
        {
            if (isActive)
            {
                Orientation();
                ArmAim();
            }
        }

        else
        {
            anim.enabled = true;
            deadDelay -= Time.deltaTime;

            if (deadDelay <= 0)
            {
                sr.color = Color.Lerp(sr.color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);
                arm.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.Lerp(arm.GetComponentsInChildren<SpriteRenderer>()[0].color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);
                arm.GetComponentsInChildren<SpriteRenderer>()[1].color = Color.Lerp(arm.GetComponentsInChildren<SpriteRenderer>()[1].color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);

                if (gameObject.name != "Dummy")
                {
                    arm.GetComponentsInChildren<SpriteRenderer>()[2].color = Color.Lerp(arm.GetComponentsInChildren<SpriteRenderer>()[2].color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);
                }

                StartCoroutine(Death());
            }
        }
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
        Vector3 aimDirection = player.transform.position - arm.transform.position;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        if (facingRight)
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle + offset), Time.deltaTime * aimSpeed);
        }
        else
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle + 180 + offset), Time.deltaTime * aimSpeed);

            //arm.transform.rotation = Quaternion.Euler(0, 0, angle + 180 + offset);
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
        mpb.SetInt("_Hit", 1);
        sr.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(0.1f);

        // Reset back to normal
        sr.GetPropertyBlock(mpb);
        mpb.SetInt("_Hit", 0);
        sr.SetPropertyBlock(mpb);
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

            StartCoroutine(Shoot());
        }
    }

    public void TakeDamage()
    {
        Camera.main.GetComponent<CameraController>().Shake(0.5f, 0.1f, 0.1f);
        FlashWhite();

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
        offset = Random.Range(-7.5f, 7.5f);
        gunAnim.SetTrigger("Shoot");
        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, -shootPoint.transform.rotation.eulerAngles.z));
        cam.Shake(0.25f, 0.1f, 0.1f);
        temp.tag = "Enemy";

        yield return new WaitForSeconds(shootSpeed);

        if (!isDead && isActive)
        {
            StartCoroutine(Shoot());
        }
    }
}
