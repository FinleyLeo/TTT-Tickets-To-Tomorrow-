using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject[] foregroundTypes, mountainTypes;
    public GameObject far, middle, close;
    public GameObject rails;

    public float laxSpeed, resetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("SpawnForeground", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        ParallaxBack();
    }

    void ParallaxBack()
    {
        close.transform.Translate(Vector3.left * Time.deltaTime * (laxSpeed * 0.5f));
        middle.transform.Translate(Vector3.left * Time.deltaTime * (laxSpeed * 0.25f));
        far.transform.Translate(Vector3.left * Time.deltaTime * (laxSpeed * 0.1f));
        rails.transform.Translate(Vector3.left * Time.deltaTime * laxSpeed * 1.5f);

        if (close.transform.position.x < -resetPos)
        {
            close.transform.position = new Vector3(1f, close.transform.position.y);
        }

        if (middle.transform.position.x < -resetPos)
        {
            middle.transform.position = new Vector3(1f, middle.transform.position.y);
        }

        if (far.transform.position.x < -resetPos)
        {
            far.transform.position = new Vector3(1f, far.transform.position.y);
        }

        if (rails.transform.position.x < - resetPos)
        {
            rails.transform.position = new Vector3(1f, rails.transform.position.y);
        }
    }

    void SpawnForeground()
    {
        int index = Random.Range(0, foregroundTypes.Length);
        Instantiate(foregroundTypes[index], new Vector3(transform.position.x + 20f, transform.position.y - 1f, transform.position.z), Quaternion.identity);

        Invoke("SpawnForeground", Random.Range(0.5f, 1.5f));
    }
}
