using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject[] small, medium, large;
    [SerializeField] GameObject[] enemies;

    [SerializeField] List<GameObject> carriages = new List<GameObject>();
    List<GameObject> actors = new List<GameObject>();
    List<GameObject> actors2 = new List<GameObject>();
    List<GameObject> camPoints = new List<GameObject>();
    GameObject carriage;

    [SerializeField] CinemachineCamera cam;
    [SerializeField] Transform map, player;

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
        Helper.instance.FindObjectwithTag("Enemy", carriages[currentCarriage], actors2);

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

        Helper.instance.FindObjectwithTag("SpawnPoint", carriage, actors);

        foreach (GameObject child in actors)
        {
            Instantiate(enemies[Random.Range(0, enemies.Length)], child.transform.position, Quaternion.identity, child.transform);
        }

        carriages.Add(carriage);
        carriageAmount++;
    }

    public void ActivateEnemies()
    {
        Helper.instance.FindObjectwithTag("Enemy", carriages[currentCarriage], actors2);

        foreach (GameObject enemy in actors2)
        {
            StartCoroutine(enemy.GetComponent<EnemyScript>().Awaken());
        }
    }

    public void DeactivateEnemies()
    {
        Helper.instance.FindObjectwithTag("Enemy", carriages[currentCarriage], actors2);

        foreach (GameObject enemy in actors2)
        {
            enemy.GetComponent<EnemyScript>().isActive = false;
            enemy.GetComponent<EnemyScript>().isAwake = false;
        }
    }

    public void FollowLogic()
    {
        if (carriages[currentCarriage].layer == 11 || carriages[currentCarriage].layer == 12)
        {
            Helper.instance.FindObjectwithTag("RoomPoint", carriages[currentCarriage], camPoints);

            cam.Target.TrackingTarget = camPoints[0].transform;
        }

        else
        {
            cam.Target.TrackingTarget = player;
        }
    }
}
