using UnityEngine;
using System.Collections;

public class ForegroundMover : MonoBehaviour
{
    public float laxSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 6f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * laxSpeed);
    }
}
