using UnityEngine;
using System.Collections;

public class ForegroundMover : MonoBehaviour
{
    public float laxSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WooshSound());
        Destroy(gameObject, 6f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * laxSpeed);
    }

    IEnumerator WooshSound()
    {
        yield return new WaitForSeconds(0.5f);

        switch (Random.Range(0, 3))
        {
            case 0:
                AudioManager.instance.PlaySFX("Woosh1");
                break;
            case 1:
                AudioManager.instance.PlaySFX("Woosh2");
                break;
            case 2:
                AudioManager.instance.PlaySFX("Woosh3");
                break;
        }
    }
}
