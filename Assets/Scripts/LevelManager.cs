using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject[] small, medium, large;

    [SerializeField] Transform map;

    float roomSize;

    float xOffset;

    public int carriageAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xOffset += 23.375f;

        for (int i = 0; i < carriageAmount; i++)
        {
            SpawnRoom();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnRoom()
    {
        roomSize = Random.Range(0f, 3f);

        if (roomSize < 1)
        {
            Instantiate(small[Random.Range(0, small.Length)], new Vector3(0 + xOffset - 0.125f, 8.445f, 0), Quaternion.identity, map);
            xOffset += 19.075f;
        }

        else if (roomSize > 1 && roomSize < 2.5f)
        {
            Instantiate(medium[Random.Range(0, medium.Length)], new Vector3(0 + xOffset, 1.08f, 0), Quaternion.identity, map);
            xOffset += 23.375f;
        }

        else if (roomSize > 2.5f)
        {
            Instantiate(large[Random.Range(0, large.Length)], new Vector3(0 + xOffset, 1.09f, 0), Quaternion.identity, map);
            xOffset += 29.375f;
        }
    }
}
