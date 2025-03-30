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
            collision.gameObject.GetComponent<EnemyScript>().TakeDamage();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;
            StartCoroutine(TimeManager.instance.HitStop(0.05f));
            Destroy(gameObject, 0.1f);
        }
    }
}
