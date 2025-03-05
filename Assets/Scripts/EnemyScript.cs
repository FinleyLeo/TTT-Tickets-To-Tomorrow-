using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    GameObject player;

    float flipValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        flipValue = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Orientation();
    }

    void Orientation()
    {
        if (player.transform.position.y - 1 > transform.position.y)
        {
            Debug.Log("Player Above Enemy");
        }

        else if (player.transform.position.y + 1 < transform.position.y)
        {
            Debug.Log("Player Below Enemy");
        }

        else
        {
            Debug.Log("Player at Enemy's Height");
        }

        // Is to the left or right of the enemy
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-flipValue, transform.localScale.y, transform.localScale.z);
        }

        else
        {
            transform.localScale = new Vector3(flipValue, transform.localScale.y, transform.localScale.z);
        }
    }
}
