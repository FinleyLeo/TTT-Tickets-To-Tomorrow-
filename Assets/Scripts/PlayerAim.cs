using UnityEngine;
using System.Collections;

public class PlayerAim : MonoBehaviour
{
    public GameObject arm, gun, shootPoint;
    public GameObject bullet;

    public Animator ammoAnim;

    float cooldown, reloadDelay;
    public float ammo = 6;
    bool reloading;

    public ParticleSystem shell;
    PlayerController playerScript;
    CameraController cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GetComponent<PlayerController>();

        cam = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIScript.instance.paused && !TimeManager.instance.gameOver && !TimeManager.instance.inDialogue)
        {
            Aim();
        }
    }

    void Aim()
    {
        Vector3 aimDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        if (playerScript.horiz == 0 && !playerScript.isSliding)
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

        cooldown -= Time.unscaledDeltaTime;
        reloadDelay -= Time.unscaledDeltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            if (cooldown <= 0 && ammo > 0 && !reloading)
            {
                Shoot();
            }

            else if (ammo <= 0 && !reloading)
            {
                AudioManager.instance.PlaySFX("ShootEmpty");
                StartCoroutine(Reload());
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && ammo != 6 && !reloading)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        if (reloadDelay <= 0)
        {
            reloadDelay = 0.5f;
            reloading = true;
            TimeManager.instance.timeLeft -= 4f;

            if (ammoAnim != null)
            {
                ammoAnim.Play("AmmoReload");
                AudioManager.instance.PlaySFX("Reload");
            }

            yield return new WaitForSeconds(0.3f);

            reloading = false;
            ammo = 6;
        }
        
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

        cam.Shake(0.75f, 0.1f, 0.1f);
        shell.Play();
        ammo -= 1;
        cooldown = 0.15f;
        gun.GetComponent<Animator>().SetTrigger("Shoot");

        GameObject temp = Instantiate(bullet, shootPoint.transform.position, Quaternion.Euler(0, 0, shootPoint.transform.rotation.eulerAngles.z));
        temp.tag = "Player";

        AudioManager.instance.PlaySFXWithPitch("Shoot", Random.Range(0.8f, 1.2f));
    }
}