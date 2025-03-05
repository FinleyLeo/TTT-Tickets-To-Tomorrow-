using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAim : MonoBehaviour
{
    public GameObject arm;

    float orientation;

    PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();
        orientation = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
    }

    void Aim()
    {
        Vector3 aimDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        if (player.horiz == 0)
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x - 0.1f)
            {
                player.facingRight = true;
                transform.localScale = new Vector3(-orientation, transform.localScale.y, transform.localScale.z);
            }

            else
            {
                player.facingRight = false;
                transform.localScale = new Vector3(orientation, transform.localScale.y, transform.localScale.z);
            }
        }

        if (player.facingRight)
        {
            arm.transform.eulerAngles = new Vector3(0, 0, angle - 180);
        }

        else
        {
            arm.transform.eulerAngles = new Vector3(0, 0, angle);
        }
        
    }

    void Shoot()
    {

    }
}
