using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public bool normalTime;
    bool slowTime, slowActive;
    bool isHitStopRunning = false;

    float slowCoolDown;
    public float timeLeft;

    public int timeLoss;
    public int previousLoss;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIScript.instance.paused && SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (Input.GetMouseButtonDown(1) && slowCoolDown <= 0)
            {
                slowActive = true;
                normalTime = false;
                slowTime = true;

                timeLoss = 2;
                GameObject.Find("Time").GetComponent<Animator>().SetFloat("AnimSpeed", timeLoss);
                Debug.Log("Slowed time");
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timeLoss = previousLoss;
                GameObject.Find("Time").GetComponent<Animator>().SetFloat("AnimSpeed", timeLoss);
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                previousLoss = timeLoss;
                timeLoss = 0;
                GameObject.Find("Time").GetComponent<Animator>().SetFloat("AnimSpeed", timeLoss);
            }
        }

        if (!Input.GetMouseButton(1) && slowTime && slowActive)
        {
            slowActive = false;
            StartCoroutine(SlowDelay());

            if (!UIScript.instance.paused)
            {
                timeLoss = 1;
                GameObject.Find("Time").GetComponent<Animator>().SetFloat("AnimSpeed", timeLoss);
            }

            previousLoss = 1;

            slowCoolDown = 0.5f;
        }

        SlowTime();

        slowCoolDown -= Time.unscaledDeltaTime;

        timeLeft -= Time.unscaledDeltaTime * timeLoss;
    }

    void SlowTime()
    {
        if (!UIScript.instance.paused)
        {
            if (slowTime)
            {
                if (Time.timeScale > 0.41)
                {
                    Time.timeScale = Mathf.Lerp(Time.timeScale, 0.4f, 10 * Time.unscaledDeltaTime);
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }

                else
                {
                    Time.timeScale = 0.4f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }
            }

            else
            {
                if (Time.timeScale < 0.8)
                {
                    Time.timeScale = Mathf.Lerp(Time.timeScale, 1, 10 * Time.unscaledDeltaTime);
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }

                else
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = 0.02f;
                    normalTime = true;
                }
            }
        }
    }

    IEnumerator SlowDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        normalTime = true;
        slowTime = false;

        Debug.Log("Normal time");
    }

    public IEnumerator HitStop(float duration)
    {
        if (isHitStopRunning)
        {
            yield break;
        }

        isHitStopRunning = true;
        slowTime = false;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        if (normalTime)
        {
            Time.timeScale = 1f;
        }

        else
        {
            slowTime = true;
            Time.timeScale = 0.4f;
        }
        
        isHitStopRunning = false;
    }
}
