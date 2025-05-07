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

    float fadeTime = 2f, deadDelay = 0.25f, shootSpeed, aimSpeed = 2f, offset, flipValue;

    public int health;

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
        shootSpeed = Random.Range(0.75f, 1.5f);
        health = 1;

        if (Random.Range(0, 100) > 99)
        {
            shootSpeed = 0.5f;
            sr.color = new Color(1, 0.4f, 0.4f, 1);
            arm.GetComponentsInChildren<SpriteRenderer>()[0].color = new Color(1, 0.4f, 0.4f, 1);
            arm.GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(1, 0.4f, 0.4f, 1);

            if (gameObject.name != "Dummy")
            {
                arm.GetComponentsInChildren<SpriteRenderer>()[2].color = new Color(1, 0.4f, 0.4f, 1);
            }

            health = 3;
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

                if (SceneManager.GetActiveScene().name != "Tutorial")
                {
                    arm.GetComponentsInChildren<SpriteRenderer>()[1].color = Color.Lerp(arm.GetComponentsInChildren<SpriteRenderer>()[1].color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);

                    if (gameObject.name != "Dummy")
                    {
                        arm.GetComponentsInChildren<SpriteRenderer>()[2].color = Color.Lerp(arm.GetComponentsInChildren<SpriteRenderer>()[2].color, new Color(1, 1, 1, 0), Time.deltaTime * fadeTime);
                    }
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

            StartCoroutine(Shoot());
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
        offset = Random.Range(-7.5f, 7.5f);
        gunAnim.SetTrigger("Shoot");
        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, -shootPoint.transform.rotation.eulerAngles.z));
        cam.Shake(0.25f, 0.1f, 0.1f);
        temp.tag = "Enemy";

        AudioManager.instance.PlaySFXWithPitch("Shoot", Random.Range(0.8f, 1.2f));

        yield return new WaitForSeconds(shootSpeed);

        if (!isDead && isActive)
        {
            StartCoroutine(Shoot());
        }
    }
}
