using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public bool normalTime;
    public bool slowTime;
    bool slowActive;
    bool isHitStopRunning = false;

    float slowCoolDown;
    public float timeLeft;
    public int timeLoss;

    public int secondsLeft, minutesLeft;

    int previousLoss, previousSec, previousMin;

    GameObject timeObj, minuteHand, secondHand;
    Animator timeAnim;
    Image timeSlider;

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
        SlowLogic();
        SlowTime();
        TimeCalc();
        WatchAnim();

        if (SceneManager.GetActiveScene().name != "Main Menu" && timeObj == null)
        {
            timeObj = GameObject.Find("Time");
            timeAnim = timeObj.GetComponent<Animator>();
            timeSlider = timeObj.transform.GetChild(0).GetComponent<Image>();

            secondHand = timeObj.transform.GetChild(5).gameObject;
            minuteHand = timeObj.transform.GetChild(4).gameObject;

            Debug.Log("Time object found: " + timeObj.name);
            Debug.Log("Time anim found: " + timeAnim.name);
        }

        slowCoolDown -= Time.unscaledDeltaTime;

        timeLeft -= Time.unscaledDeltaTime * timeLoss;
        timeLeft = Mathf.Clamp(timeLeft, 0, 720); // 15 minutes max
    }

    void TimeCalc()
    {
        secondsLeft = Mathf.RoundToInt(timeLeft % 60);
        minutesLeft = Mathf.FloorToInt(timeLeft / 60);

        // Plays sound when ticks
        if (previousSec != secondsLeft)
        {
            previousSec = secondsLeft;
            Debug.Log("Second hand Ticked");
        }

        if (previousMin != minutesLeft)
        {
            previousMin = minutesLeft;
            Debug.Log("Minute Hand Ticked");
        }
    }

    void WatchAnim()
    {
        if (timeSlider != null)
        {
            timeSlider.fillAmount = Mathf.Lerp(timeSlider.fillAmount, Mathf.Clamp01(timeLeft / 720), Time.unscaledDeltaTime * 10);
            timeSlider.color = Color.Lerp(Color.red, Color.white, timeLeft / 720);
        }

        if (timeAnim != null)
        {
            timeAnim.SetFloat("AnimSpeed", timeLoss);
        }

        // Will move the watch hands based on current time

        if (minuteHand != null && secondHand != null)
        {
            float secondAngle = secondsLeft * 6f;           // 360° / 60 seconds
            float minuteAngle = (minutesLeft % 12) * 30f;   // 360° / 12 minutes

            // Lerp current rotation to target rotation
            float currentSec = secondHand.transform.localEulerAngles.z;
            float currentMin = minuteHand.transform.localEulerAngles.z;

            float smoothSec = Mathf.LerpAngle(currentSec, secondAngle, Time.unscaledDeltaTime * 10f);
            float smoothMin = Mathf.LerpAngle(currentMin, minuteAngle, Time.unscaledDeltaTime * 5f);

            secondHand.transform.localEulerAngles = new Vector3(0, 0, smoothSec);
            minuteHand.transform.localEulerAngles = new Vector3(0, 0, smoothMin);
        }
    }

    void SlowLogic()
    {
        if (!UIScript.instance.paused && SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (Input.GetMouseButtonDown(1) && slowCoolDown <= 0)
            {
                slowActive = true;
                normalTime = false;
                slowTime = true;
                timeLoss = 3;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timeLoss = previousLoss;
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                previousLoss = timeLoss;
                timeLoss = 0;
            }
        }

        if (!Input.GetMouseButton(1) && slowTime && slowActive)
        {
            slowActive = false;
            StartCoroutine(SlowDelay());

            if (!UIScript.instance.paused)
            {
                timeLoss = 1;
            }

            previousLoss = 1;

            slowCoolDown = 0.5f;
        }
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
