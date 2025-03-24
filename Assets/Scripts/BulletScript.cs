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

            Destroy(gameObject);
        }
    }
}
