using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject[] foregroundTypes, mountainTypes;
    public GameObject far, middle, close;
    public GameObject rails, foreground, mountains;

    public float laxSpeed, resetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("SpawnForeground", 0f);
        Invoke("SpawnMountains", 0f);
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

        if (close.transform.position.x < -resetPos + Camera.main.transform.position.x)
        {
            close.transform.position = new Vector3(1f + Camera.main.transform.position.x, close.transform.position.y);
        }

        if (middle.transform.position.x < -resetPos + Camera.main.transform.position.x)
        {
            middle.transform.position = new Vector3(1f + Camera.main.transform.position.x, middle.transform.position.y);
        }

        if (far.transform.position.x < -resetPos + Camera.main.transform.position.x)
        {
            far.transform.position = new Vector3(1f + Camera.main.transform.position.x, far.transform.position.y);
        }

        if (rails.transform.position.x < - resetPos + Camera.main.transform.position.x)
        {
            rails.transform.position = new Vector3(1f + Camera.main.transform.position.x, rails.transform.position.y);
        }
    }

    void SpawnForeground()
    {
        int index = Random.Range(0, foregroundTypes.Length);
        Instantiate(foregroundTypes[index], new Vector3(transform.position.x + 20f,  0, transform.position.z), Quaternion.identity, foreground.transform);

        Invoke("SpawnForeground", Random.Range(0.75f, 1.5f));
    }

    void SpawnMountains()
    {
        int index = Random.Range(0, mountainTypes.Length);
        Instantiate(mountainTypes[index], new Vector3(transform.position.x + 20f, transform.position.y - 0.2f, transform.position.z), Quaternion.identity, mountains.transform);

        Invoke("SpawnMountains", Random.Range(1.25f, 2f));
    }
}
