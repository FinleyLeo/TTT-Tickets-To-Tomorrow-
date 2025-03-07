using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAim : MonoBehaviour
{
    public GameObject arm, gun, shootPoint;

    public GameObject bullet;

    float orientation;
    float cooldown;

    public ParticleSystem shell;
    PlayerController playerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GetComponent<PlayerController>();
        orientation = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
    }

    void Aim()
    {
        Vector3 aimDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        if (playerScript.horiz == 0)
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
            {
                playerScript.facingLeft = true;
                transform.localScale = new Vector3(-orientation, transform.localScale.y, transform.localScale.z);
            }

            else
            {
                playerScript.facingLeft = false;
                transform.localScale = new Vector3(orientation, transform.localScale.y, transform.localScale.z);
            }
        }

        if (playerScript.facingLeft)
        {
            arm.transform.rotation = Quaternion.Euler(0, 0, angle + 185);
        }

        else
        {
            arm.transform.rotation = Quaternion.Euler(0, 0, angle - 5);
        }

        cooldown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && cooldown <= 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        cooldown = 0.2f;
        shell.Play();
        gun.GetComponent<Animator>().SetTrigger("Shoot");

        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, shootPoint.transform.rotation.eulerAngles.z));
        temp.tag = "Player";
    }
}
