using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{

    public static TimeManager instance;

    public bool normalTime;
    bool slowTime;
    bool isHitStopRunning = false;

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
        if (!normalTime)
        {
            if (!slowTime)
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

            else
            {
                // slow motion effect
            }
        }
    }

    public IEnumerator HitStop(float duration)
    {
        if (isHitStopRunning)
        {
            yield break;
        }

        isHitStopRunning = true;

        Time.timeScale = 0f;
        Debug.Log("HitStop: Time.timeScale set to 0");

        yield return new WaitForSecondsRealtime(duration);

        if (normalTime)
        {
            Time.timeScale = 1f;
            Debug.Log("HitStop: Time.timeScale reset to 1");
        }
        else
        {
            Time.timeScale = 0.25f;
            Debug.Log("HitStop: Time.timeScale set to 0.25");
        }

        isHitStopRunning = false;
    }
}
