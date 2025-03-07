using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    SpriteRenderer sr;
    GameObject player;

    public GameObject bullet;
    public GameObject shootPoint;

    float flipValue;
    bool facingRight;

    public Animator gunAnim;
    public GameObject arm;
    public Sprite normalSprite;  // Default front-facing sprite
    public Sprite upSprite;      // Sprite when aiming up
    public Sprite downSprite;    // Sprite when aiming down

    void Start()
    {
        player = GameObject.Find("Player");
        sr = GetComponent<SpriteRenderer>();

        flipValue = transform.localScale.x;

        StartCoroutine(Shoot());
    }


    void Update()
    {
        Orientation();
        ArmAim();
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
            arm.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            arm.transform.rotation = Quaternion.Euler(0, 0, angle + 180);
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

    IEnumerator Shoot()
    {
        gunAnim.SetTrigger("Shoot");
        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, -shootPoint.transform.rotation.eulerAngles.z));
        temp.tag = "Enemy";

        yield return new WaitForSeconds(Random.Range(0.25f, 1f));

        StartCoroutine(Shoot());
    }
}
