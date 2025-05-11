using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject[] small, medium, large, refill;
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
        if (SceneManager.GetActiveScene().name == "Loop1")
        {
            EnemyDetection();

            if (currentCarriage > carriageAmount - 1)
            {
                if (TimeManager.instance.carriagesPassed > 15)
                {
                    if (Random.Range(0, 101) < 20)
                    {
                        SpawnRefill();
                        TimeManager.instance.carriagesPassed = 0;
                    }

                    else
                    {
                        SpawnRoom();
                    }
                }

                else
                {
                    SpawnRoom();
                }
            }
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

    void SpawnRefill()
    {
        roomSize = Random.Range(0f, 3f);

        if (roomSize < 1)
        {
            carriage = Instantiate(refill[0], new Vector3(0 + xOffset - 0.125f, 8.445f, 0), Quaternion.identity, map);

            xOffset += 19.075f;
        }

        else if (roomSize > 1 && roomSize < 2.5f)
        {
            carriage = Instantiate(refill[1], new Vector3(0 + xOffset, 1.08f, 0), Quaternion.identity, map);

            xOffset += 23.375f;
        }

        else if (roomSize > 2.5f)
        {
            carriage = Instantiate(refill[2], new Vector3(0 + xOffset, 1.09f, 0), Quaternion.identity, map);

            xOffset += 29.375f;
        }

        carriages.Add(carriage);
        carriageAmount++;
    }

    public void ActivateEnemies()
    {
        if (SceneManager.GetActiveScene().name == "Loop1")
        {
            Helper.instance.FindObjectwithTag("Enemy", carriages[currentCarriage], actors2);

            foreach (GameObject enemy in actors2)
            {
                StartCoroutine(enemy.GetComponent<EnemyScript>().Awaken());
            }
        }        
    }

    public void DeactivateEnemies()
    {
        if (SceneManager.GetActiveScene().name == "Loop1")
        {
            Helper.instance.FindObjectwithTag("Enemy", carriages[currentCarriage], actors2);

            foreach (GameObject enemy in actors2)
            {
                enemy.GetComponent<EnemyScript>().isActive = false;
                enemy.GetComponent<EnemyScript>().isAwake = false;
            }
        }
    }

    public void RespawnEnemies()
    {
        Helper.instance.FindObjectwithTag("SpawnPoint", carriages[currentCarriage], actors);

        foreach (GameObject child in actors)
        {
            if (child.transform.childCount == 0)
            {
                Instantiate(enemies[Random.Range(0, enemies.Length)], child.transform.position, Quaternion.identity, child.transform);
            }

            else if (child.transform.GetChild(0).GetComponent<EnemyScript>().isDead && child.transform.childCount > 0)
            {
                Instantiate(enemies[Random.Range(0, enemies.Length)], child.transform.position, Quaternion.identity, child.transform);
            }
        }

        ActivateEnemies();
    }

    public void FollowLogic()
    {
        if (SceneManager.GetActiveScene().name == "Loop1")
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

        else
        {
            cam.Target.TrackingTarget = player;
        }
    }
}
