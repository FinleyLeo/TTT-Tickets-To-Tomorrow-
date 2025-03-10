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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GetComponent<PlayerController>();
        orientation = transform.localScale.x;

        ammoAnim = GameObject.Find("Ammo").GetComponent<Animator>();
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
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle + 185), Time.deltaTime * 50);
        }

        else
        {
            arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(0, 0, angle - 5), Time.deltaTime * 50);
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
        shell.emission.SetBurst(0, new ParticleSystem.Burst(0.1f, ammo));
        shell.Play();
        reloading = true;
        ammoAnim.Play("AmmoReload");
        yield return new WaitForSeconds(0.75f);
        reloading = false;
        ammo = 6;
    }

    void Shoot()
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

        ammo -= 1;
        cooldown = 0.15f;
        gun.GetComponent<Animator>().SetTrigger("Shoot");

        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, shootPoint.transform.rotation.eulerAngles.z));
        temp.tag = "Player";
    }
}
