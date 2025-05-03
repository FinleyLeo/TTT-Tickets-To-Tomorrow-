using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;

    GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");

        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        if (gameObject.CompareTag("Player") || player.GetComponent<PlayerController>().invincible)
        {
            GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Player");
        }

        else if (gameObject.CompareTag("Enemy"))
        {
            GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Enemy");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("RightDoor") || collision.gameObject.CompareTag("LeftDoor"))
        {
            AudioManager.instance.PlaySFX("WallHit");

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;
            StartCoroutine(TimeManager.instance.HitStop(0.15f));
            Destroy(gameObject, 0.20f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Player"))
        {
            float multiplier;

            EnemyScript enemyScript = collision.gameObject.GetComponent<EnemyScript>();

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;

            TimeManager.instance.comboTime = 1f;

            if (TimeManager.instance.comboAmount < 3)
            {
                TimeManager.instance.comboAmount += 1;
            }

            if (TimeManager.instance.slowTime)
            {
                multiplier = 1f;
            }

            else
            {
                multiplier = 1.5f;
            }

            TimeManager.instance.timeLeft += (multiplier * enemyScript.health * TimeManager.instance.comboAmount);
            
            enemyScript.TakeDamage();

            Destroy(gameObject, 0.1f);
        }
    }
}
