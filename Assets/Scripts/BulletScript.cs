using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D rb;

    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // collision.contacts[0].normal will be used for effects being displayed when hitting a wall
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player hit");
            Destroy(gameObject);
            //Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
