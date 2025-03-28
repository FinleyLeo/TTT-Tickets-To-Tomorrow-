using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public static TimeManager instance;

    public bool normalTime;

    bool slowTime;

    public float delay;

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
        if (normalTime)
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
            }
        }

        else
        {
            delay -= Time.unscaledDeltaTime;

            if (delay <= 0)
            {
                normalTime = true;
            }
        }
    }
}
