using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject[] small, medium, large;
    [SerializeField] GameObject[] enemies;

    [SerializeField] Transform map;

    float roomSize;

    float xOffset;

    public int carriageAmount;

    string spawnTag;
    public List<GameObject> actors = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        xOffset += 23.375f;

        for (int i = 0; i < carriageAmount; i++)
        {
            SpawnRoom();
        }
    }

    public void FindObjectwithTag(string _tag, GameObject temp)
    {
        actors.Clear();
        Transform parent = temp.transform;
        GetChildObject(parent, _tag);
    }

    public void GetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.tag == _tag)
            {
                actors.Add(child.gameObject);
            }

            if (child.childCount > 0)
            {
                GetChildObject(child, _tag);
            }
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
            GameObject temp = Instantiate(small[Random.Range(0, small.Length)], new Vector3(0 + xOffset - 0.125f, 8.445f, 0), Quaternion.identity, map);

            if (spawnTag != null)
            {
                FindObjectwithTag(spawnTag, temp);

                foreach (GameObject child in actors)
                {
                    Debug.Log("child name: " + child.name);
                }

            }

            xOffset += 19.075f;
        }

        else if (roomSize > 1 && roomSize < 2.5f)
        {
            GameObject temp = Instantiate(medium[Random.Range(0, medium.Length)], new Vector3(0 + xOffset, 1.08f, 0), Quaternion.identity, map);

            if (spawnTag != null)
            {
                FindObjectwithTag(spawnTag, temp);

                foreach (GameObject child in actors)
                {
                    Debug.Log("child name: " + child.name);
                }

            }

            xOffset += 23.375f;
        }

        else if (roomSize > 2.5f)
        {
            GameObject temp = Instantiate(large[Random.Range(0, large.Length)], new Vector3(0 + xOffset, 1.09f, 0), Quaternion.identity, map);

            if (spawnTag != null)
            {
                FindObjectwithTag(spawnTag, temp);

                foreach (GameObject child in actors)
                {
                    Debug.Log("child name: " + child.name);
                }

            }

            xOffset += 29.375f;
        }
    }
}
