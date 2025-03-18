using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    SpriteRenderer sr;
    Animator anim;
    GameObject player;
    MaterialPropertyBlock mpb;
    IEnumerator flashRoutine;

    public GameObject bullet;
    public GameObject shootPoint;

    public float flipValue, offset;
    float fadeTime = 1.5f, deadDelay = 0.5f, shootSpeed;
    bool facingRight;
    public bool isDead;

    public Animator gunAnim;
    public GameObject arm;
    public Sprite normalSprite;  // Default front-facing sprite
    public Sprite upSprite;      // Sprite when aiming up
    public Sprite downSprite;    // Sprite when aiming down

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        mpb = new MaterialPropertyBlock();

        anim.enabled = false;

        flipValue = transform.localScale.x;

        shootSpeed = Random.Range(0.75f, 1.5f);

        if (Random.Range(0, 100) > 90)
        {
            shootSpeed *= 2;
        }

        StartCoroutine(Shoot());
    }


    void Update()
    {
        if (!isDead)
        {
            Orientation();
            ArmAim();
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
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle + offset), Time.deltaTime * 3);
        }
        else
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle + 180 + offset), Time.deltaTime * 3);

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

    IEnumerator Death()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    IEnumerator Shoot()
    {
        offset = Random.Range(-7.5f, 7.5f);
        gunAnim.SetTrigger("Shoot");
        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, -shootPoint.transform.rotation.eulerAngles.z));
        temp.tag = "Enemy";

        yield return new WaitForSeconds(shootSpeed);

        if (!isDead)
        {
            StartCoroutine(Shoot());
        }
    }
}
