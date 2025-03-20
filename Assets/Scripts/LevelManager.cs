using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject[] small, medium, large;
    [SerializeField] GameObject[] enemies;
    [SerializeField] List<GameObject> carriages = new List<GameObject>();
    List<GameObject> actors = new List<GameObject>();
    List<GameObject> actors2 = new List<GameObject>();
    GameObject carriage;

    [SerializeField] Transform map;

    float roomSize;

    float xOffset;

    public int carriageAmount, currentCarriage;

    public int enemyAmount;

    public bool canLeave;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xOffset += 23.375f;

        carriage = GameObject.Find("Start Room");
        carriages.Add(carriage);
    }

    public void FindObjectwithTag(string _tag, GameObject temp, List<GameObject> actors)
    {
        actors.Clear();
        Transform parent = temp.transform;
        GetChildObject(parent, _tag, actors);
    }

    public void GetChildObject(Transform parent, string _tag, List<GameObject> actors)
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
                GetChildObject(child, _tag, actors);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyDetection();

        if (currentCarriage > carriageAmount - 1)
        {
            SpawnRoom();
        }
    }

    public void EnemyDetection()
    {
        FindObjectwithTag("Enemy", carriages[currentCarriage], actors2);

        enemyAmount = actors2.Count;

        if (enemyAmount > 0)
        {
            canLeave = false;
        }

        else
        {
            canLeave = true;
        }
    }

    void SpawnRoom()
    {
        roomSize = Random.Range(0f, 3f);

        if (roomSize < 1)
        {
            carriage = Instantiate(small[Random.Range(0, small.Length)], new Vector3(0 + xOffset - 0.125f, 8.445f, 0), Quaternion.identity, map);

            xOffset += 19.075f;
        }

        else if (roomSize > 1 && roomSize < 2.5f)
        {
            carriage = Instantiate(medium[Random.Range(0, medium.Length)], new Vector3(0 + xOffset, 1.08f, 0), Quaternion.identity, map);

            xOffset += 23.375f;
        }

        else if (roomSize > 2.5f)
        {
            carriage = Instantiate(large[Random.Range(0, large.Length)], new Vector3(0 + xOffset, 1.09f, 0), Quaternion.identity, map);

            xOffset += 29.375f;
        }

        FindObjectwithTag("SpawnPoint", carriage, actors);

        foreach (GameObject child in actors)
        {
            Instantiate(enemies[Random.Range(0, enemies.Length)], child.transform.position, Quaternion.identity, child.transform);
        }

        carriages.Add(carriage);
        carriageAmount++;
    }
}
