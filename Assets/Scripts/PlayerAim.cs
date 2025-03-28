using UnityEngine;
using System.Collections;

public class PlayerAim : MonoBehaviour
{
    public GameObject arm, gun, shootPoint;
    public GameObject bullet;

    Animator ammoAnim;

    float orientation;
    float cooldown;
    float ammo = 6;
    bool reloading;

    public ParticleSystem shell;
    PlayerController playerScript;
    CameraController cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GetComponent<PlayerController>();
        orientation = transform.localScale.x;

        cam = Camera.main.GetComponent<CameraController>();
        ammoAnim = GameObject.Find("Ammo")?.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIScript.instance.paused)
        {
            Aim();
        }
    }

    void Aim()
    {
        Vector3 aimDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        if (playerScript.horiz == 0)
        {
            bool shouldFaceLeft = Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x;

            if (shouldFaceLeft != playerScript.facingLeft)
            {
                playerScript.facingLeft = shouldFaceLeft;
                transform.rotation = Quaternion.Euler(0, shouldFaceLeft ? 180 : 0, 0);
            }
        }

        if (playerScript.facingLeft)
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(180, 0, -angle - 5), Time.deltaTime * 50);
            shootPoint.transform.rotation = Quaternion.Lerp(shootPoint.transform.rotation, Quaternion.Euler(0, 0, angle + 270), Time.deltaTime * 50);
        }

        else
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle - 5), Time.deltaTime * 50);
            shootPoint.transform.rotation = Quaternion.Lerp(shootPoint.transform.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * 50);
        }

        cooldown -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && cooldown <= 0 && ammo > 0 && !reloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && ammo != 6 && !reloading)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        reloading = true;

        if (ammoAnim != null)
        {
            ammoAnim.Play("AmmoReload");
        }

        yield return new WaitForSeconds(0.3f);
        reloading = false;
        ammo = 6;
    }

    void Shoot()
    {
        if (ammoAnim != null)
        {
            switch (ammo)
            {
                case 6:
                    ammoAnim.Play("Shot1", 0);
                    break;
                case 5:
                    ammoAnim.Play("Shot2", 0);
                    break;
                case 4:
                    ammoAnim.Play("Shot3", 0);
                    break;
                case 3:
                    ammoAnim.Play("Shot4", 0);
                    break;
                case 2:
                    ammoAnim.Play("Shot5", 0);
                    break;
                case 1:
                    ammoAnim.Play("Shot6", 0);
                    break;
            }
        }

        cam.Shake(1f, 0.1f, 0.1f);
        shell.Play();
        ammo -= 1;
        cooldown = 0.15f;
        gun.GetComponent<Animator>().SetTrigger("Shoot");

        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, shootPoint.transform.rotation.eulerAngles.z));
        temp.tag = "Player";
    }
}