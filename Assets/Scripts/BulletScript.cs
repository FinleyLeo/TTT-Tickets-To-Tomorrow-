using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        if (gameObject.CompareTag("Player"))
        {
            GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Player");
        }

        else if (gameObject.CompareTag("Enemy"))
        {
            GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Enemy");
        }
    }

    IEnumerator HitTime()
    {
        TimeManager.instance.normalTime = false;
        TimeManager.instance.delay = 0.05f;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;

        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(0.15f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("RightDoor") || collision.gameObject.CompareTag("LeftDoor"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy"))
        {
            collision.gameObject?.GetComponent<PlayerController>()?.TakeDamage();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Player"))
        {
            collision.GetComponent<EnemyScript>().FlashWhite();
            collision.GetComponent<Animator>().SetBool("Dead", true);
            collision.GetComponent<EnemyScript>().isDead = true;
            collision.GetComponent<CapsuleCollider2D>().enabled = false;

            StartCoroutine(HitTime());
        }
    }
}
