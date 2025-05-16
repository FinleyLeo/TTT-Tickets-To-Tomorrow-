using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public bool normalTime;
    public bool slowTime;
    bool slowActive;

    public bool isRewinding;

    float slowCoolDown;
    public float timeLeft;
    public int timeLoss;

    public int secondsLeft, minutesLeft;

    int previousLoss, previousSec, previousMin;

    GameObject minuteHand, secondHand;
    public GameObject timeObj;
    Animator timeAnim;
    Image timeSlider;

    bool secondFacade;

    public float comboTime;
    public int comboAmount;

    public Material shockwaveMat, sepiaMat;
    public float deathTimeElapsed, sepiaLerp;
    public bool waveDone, sepiaActive;

    public bool gameOver;
    public Animator gameOverAnim;
    bool gameOverSoundPlayed = false;

    public int health;

    public bool saveExists;

    public bool inDialogue;
    public int carriagesPassed, sinceRefill, currentLoop;

    public bool gameEnded;
    public Material crack;
    public Image endScreen;
    float crackAmount = 10;
    bool animPlaying = false;

    // Tutorial vars

    public bool hasWatch;
    public bool tutorialComplete;

    #region singleton

    private void Awake()
    {
        if (instance == null)
        {
            saveExists = false;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crack.SetFloat("_Radius", 10);
        LoadValues();
    }

    // Update is called once per frame
    void Update()
    {
        ComboLogic();
        SlowTime();

        comboTime -= Time.deltaTime;
        timeLeft = Mathf.Clamp(timeLeft, 0, 720); // 12 minutes max

        // Stops slow motion
        if (!Input.GetMouseButton(1) && slowTime && slowActive)
        {
            slowActive = false;
            StartCoroutine(SlowDelay());

            AudioManager.instance.PlaySFX("SlowOut");

            if (!UIScript.instance.paused)
            {
                timeLoss = 1;
            }

            previousLoss = 1;

            slowCoolDown = 0.5f;
        }


        // Runs all code
        if (SceneManager.GetActiveScene().name != "Main Menu" && hasWatch && !AudioManager.instance.inSpace)
        {
            TimeCalc();
            SlowLogic();
            WatchAnim();

            slowCoolDown -= Time.unscaledDeltaTime;

            timeLeft -= Time.unscaledDeltaTime * timeLoss;

            if (timeObj == null && hasWatch)
            {
                timeObj = GameObject.Find("Time");

                for (int i = 0; i < timeObj.transform.childCount; i++)
                {
                    GameObject Go = timeObj.transform.GetChild(i).gameObject;

                    Go.SetActive(true);
                }

                timeAnim = timeObj.GetComponent<Animator>();
                timeSlider = timeObj.transform.GetChild(1).GetComponent<Image>();

                secondHand = timeObj.transform.GetChild(6).gameObject;
                minuteHand = timeObj.transform.GetChild(5).gameObject;
            }

            // Game over management
            if (timeLeft <= 0 && !gameOver && !waveDone)
            {
                timeLeft = 0;
                timeLoss = 0;
                slowTime = false;
                normalTime = false;
                gameOver = true;
            }

            if (gameOver)
            {
                if (!waveDone)
                {
                    if (!gameOverSoundPlayed)
                    {
                        gameOverSoundPlayed = true;
                        AudioManager.instance.PlaySFX("GameOver");
                    }

                    deathTimeElapsed += Time.unscaledDeltaTime;

                    shockwaveMat.SetFloat("_isActive", 1);
                    shockwaveMat.SetFloat("_UnscaledTime", deathTimeElapsed);

                    if (Time.timeScale > 0)
                    {
                        Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, 5 * Time.unscaledDeltaTime);
                        Time.fixedDeltaTime = 0.02f * Time.timeScale;
                    }

                    else
                    {
                        Time.timeScale = 0f;
                    }

                    sepiaActive = true;

                    if (sepiaMat.GetFloat("_Slider") < 1)
                    {
                        sepiaLerp = Mathf.Lerp(sepiaLerp, 1, Time.unscaledDeltaTime);
                        sepiaMat.SetFloat("_Slider", sepiaLerp);
                    }

                    else
                    {
                        sepiaMat.SetFloat("_Slider", 1);
                    }

                    if (deathTimeElapsed > 2.6f)
                    {
                        waveDone = true;
                    }
                }

                else
                {
                    // Game run ends
                    gameOverAnim.SetBool("GameOver", gameOver);
                }   
            }
        }

        if (gameEnded)
        {
            EndLerp();

            if (!animPlaying)
            {
                animPlaying = true;
                StartCoroutine(EndAnimation());
            }
        }
    }

    public void LoadValues()
    {
        if (PlayerPrefs.HasKey("Health"))
        {
            health = PlayerPrefs.GetInt("Health");
        }

        else
        {
            health = 5;
        }

        if (PlayerPrefs.HasKey("TimeLeft"))
        {
            timeLeft = PlayerPrefs.GetFloat("TimeLeft");
        }

        else
        {
            timeLeft = 720;
        }

        if (PlayerPrefs.HasKey("saveExists"))
        {
            saveExists = PlayerPrefs.GetInt("saveExists") == 1;
        }

        else
        {
            saveExists = false;
        }

        if (PlayerPrefs.HasKey("carriagesPassed"))
        {
            carriagesPassed = PlayerPrefs.GetInt("carriagesPassed");
        }

        else
        {
            carriagesPassed = 0;
        }

        if (PlayerPrefs.HasKey("sinceRefill"))
        {
            sinceRefill = PlayerPrefs.GetInt("sinceRefill");
        }

        else
        {
            sinceRefill = 0;
        }

        if (PlayerPrefs.HasKey("currentLoop"))
        {
            currentLoop = PlayerPrefs.GetInt("currentLoop");
        }

        else
        {
            currentLoop = 1;
        }

        if (PlayerPrefs.HasKey("hasWatch"))
        {
            hasWatch = PlayerPrefs.GetInt("hasWatch") == 1;
        }
        else
        {
            hasWatch = false;
        }
    }

    public void SaveValues()
    {
        PlayerPrefs.SetFloat("TimeLeft", timeLeft);
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetInt("saveExists", saveExists ? 1 : 0);
        PlayerPrefs.SetInt("carriagesPassed", carriagesPassed);
        PlayerPrefs.SetInt("sinceRefill", sinceRefill);
        PlayerPrefs.SetInt("currentLoop", currentLoop);
        PlayerPrefs.SetInt("hasWatch", hasWatch ? 1 : 0);
        PlayerPrefs.Save();
    }

    void TimeCalc()
    {
        secondsLeft = Mathf.FloorToInt(timeLeft % 60);
        minutesLeft = Mathf.FloorToInt(timeLeft / 60);

        // Plays sound when ticks

        if (previousSec != secondsLeft)
        {
            previousSec = secondsLeft;

            if (timeLoss < 6)
            {
                secondFacade = false;

                AudioManager.instance.PlaySFX("Tick1");
            }

            else
            {
                // Play seconds tick sound every half second
                if (!secondFacade)
                {
                    secondFacade = true;
                    StartCoroutine(SecFacade());
                }
            }
        }

        if (previousMin != minutesLeft)
        {
            previousMin = minutesLeft;

            AudioManager.instance.PlaySFX("Tick2");
        }
    }

    void OnSceneChange(Scene current, Scene next)
    {
        Shader.SetGlobalFloat("_isAffected", 0);
        sepiaMat.SetFloat("_Slider", 0);
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
        if (!UIScript.instance.paused && !isRewinding)
        {
            if (Input.GetMouseButtonDown(1) && slowCoolDown <= 0)
            {
                slowActive = true;
                normalTime = false;
                slowTime = true;
                timeLoss = 3;

                AudioManager.instance.PlaySFX("SlowIn");
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                timeLoss = previousLoss;
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isRewinding)
            {
                previousLoss = timeLoss;
                timeLoss = 0;
            }
        }
    }

    void SlowTime()
    {
        if (hasWatch)
        {
            if (!UIScript.instance.paused && !isRewinding && !gameOver && !inDialogue)
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
    }

    IEnumerator SlowDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        normalTime = true;
        slowTime = false;

        Debug.Log("Normal time");
    }

    void ComboLogic()
    {
        if (comboTime <= 0)
        {
            comboAmount = 0;
            comboTime = 0;
        }

        if (comboAmount > 3)
        {
            comboAmount = 3;
        }
    }

    public void ToggleSepia()
    {
        sepiaActive = !sepiaActive;

        if (!sepiaActive)
        {
            for (float i = 0; i < 1; i += 0.1f)
            {
                sepiaLerp = Mathf.Lerp(sepiaLerp, 1, i);
                sepiaMat.SetFloat("_Slider", sepiaLerp);
            }
        }

        else if (sepiaActive)
        {
            for (float i = 1; i > 0; i -= 0.1f)
            {
                sepiaLerp = Mathf.Lerp(sepiaLerp, 0, i);
                sepiaMat.SetFloat("_Slider", sepiaLerp);
            }
        }
    }

    IEnumerator SecFacade()
    {
        AudioManager.instance.PlaySFX("EchoTick");

        yield return new WaitForSecondsRealtime(0.5f);

        if (secondFacade)
        {
            StartCoroutine(SecFacade());
        }
    }

    IEnumerator EndAnimation()
    {
        crackAmount = 6;
        Camera.main.GetComponent<CameraController>().Shake(0.75f, 0.5f, 0.1f);
        AudioManager.instance.PlayUnaffectedSFX("EndCrack1");

        yield return new WaitForSeconds(1f);

        crackAmount = 3;
        Camera.main.GetComponent<CameraController>().Shake(0.75f, 0.5f, 0.3f);
        AudioManager.instance.PlayUnaffectedSFX("EndCrack2");

        yield return new WaitForSeconds(1.5f);

        crackAmount = 0;
        Camera.main.GetComponent<CameraController>().Shake(0.75f, 0.5f, 0.5f);
        AudioManager.instance.PlayUnaffectedSFX("EndCrack3");

        yield return new WaitForSeconds(0.25f);

        crackAmount = 10;
        endScreen.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(1f);

        endScreen.color = new Color(0, 0, 0, 1);
        AudioManager.instance.PlayUnaffectedSFX("EndShatter");

        yield return new WaitForSeconds(2f);

        endScreen.GetComponent<Animator>().SetBool("Active", true);
    }

    void EndLerp()
    {
        crack.SetFloat("_Radius", Mathf.Lerp(crack.GetFloat("_Radius"), crackAmount, Time.unscaledDeltaTime * 10));
    }

    private void OnApplicationQuit()
    {
        SaveValues();
    }
}
