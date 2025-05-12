using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;

    public bool paused;

    [SerializeField] Animator pauseAnim, quitChoice, menuAnim;
    [SerializeField] Image pauseDarken;

    // Menus
    [SerializeField] GameObject mainMenu;


    // Sliders
    [SerializeField] GameObject[] sliders;

    // Resolutions
    [SerializeField] TMP_Dropdown resolutions, pResolutions;

    // Toggles
    [SerializeField] GameObject[] crosses;
    bool sfxMuted, musicMuted;
    bool vSyncActive, fullScreen;
    bool quitToggle;
    bool menuActive = true;

    Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            GameObject.Find("Main Menu").SetActive(true);
        }

        SetValues();

        canvas = GetComponent<Canvas>();
    }

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

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    void OnSceneChange(Scene current, Scene next)
    {
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    void SetValues()
    {
        if (PlayerPrefs.HasKey("musicMuted"))
        {
            if (PlayerPrefs.GetInt("musicMuted") == 1)
            {
                musicMuted = false;
                MusicToggle();
            }

            else
            {
                musicMuted = true;
                MusicToggle();
            }
        }

        if (PlayerPrefs.HasKey("sfxMuted"))
        {
            if (PlayerPrefs.GetInt("sfxMuted") == 1)
            {
                sfxMuted = false;
                SFXToggle();
            }

            else
            {
                sfxMuted = true;
                SFXToggle();
            }
        }

        if (PlayerPrefs.HasKey("vSyncActive"))
        {
            if (PlayerPrefs.GetInt("vSyncActive") == 1)
            {
                vSyncActive = false;
                VSyncToggle();
            }

            else
            {
                vSyncActive = true;
                VSyncToggle();
            }
        }

        if (PlayerPrefs.HasKey("fsActive"))
        {
            if (PlayerPrefs.GetInt("fsActive") == 1)
            {
                fullScreen = false;
                FullscreenToggle();
            }

            else
            {
                fullScreen = true;
                FullscreenToggle();
            }
        }

        if (PlayerPrefs.HasKey("resolution"))
        {
            SetResolution(PlayerPrefs.GetInt("resolution"));
        }

        else
        {
            SetResolution(2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            PauseCheck();
        }
    }

    void PauseCheck()
    {
        if (!TimeManager.instance.gameOver && !TimeManager.instance.inDialogue)
        {
            if (paused)
            {
                if (Time.timeScale > 0.05)
                {
                    Time.timeScale = Mathf.Lerp(Time.timeScale, 0, 1 - Mathf.Exp(-10 * Time.unscaledDeltaTime));
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                }

                else
                {
                    Time.timeScale = 0;
                }

                if (pauseDarken.color.a < 0.65f)
                {
                    pauseDarken.color = Color.Lerp(pauseDarken.color, new Color(0, 0, 0, 0.66f), Time.unscaledDeltaTime * 10);
                }

                else
                {
                    pauseDarken.color = new Color(0, 0, 0, 0.66f);
                }
            }

            else
            {
                if (pauseDarken.color.a > 0.05f)
                {
                    pauseDarken.color = Color.Lerp(pauseDarken.color, new Color(0, 0, 0, 0f), Time.unscaledDeltaTime * 10);
                }

                else
                {
                    pauseDarken.color = new Color(0, 0, 0, 0);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !TimeManager.instance.isRewinding)
            {
                paused = !paused;
                TimeManager.instance.normalTime = !paused;
                pauseAnim.SetBool("Paused", paused);

                if (!paused)
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                    Cursor.lockState = CursorLockMode.Confined;
                }

                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }

    public void ExitPause()
    {
        paused = false;
        TimeManager.instance.normalTime = !paused;
        pauseAnim.SetBool("Paused", paused);
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void PauseQuit()
    {
        quitToggle = !quitToggle;

        quitChoice.SetBool("Opened", quitToggle);
        pauseAnim.SetBool("Paused", !quitToggle);
    }

    public void ToMenu()
    {
        quitToggle = false;

        SceneSwitcher.instance.Transition("Main Menu");
        StartCoroutine(ToggleMenu());

        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        paused = false;
        menuAnim.SetTrigger("Reset");
        pauseAnim.SetBool("Paused", paused);
        quitChoice.SetBool("Opened", quitToggle);

        AudioManager.instance.PlayMusic("Menu");
        TimeManager.instance.SaveValues();
    }

    public void Quit()
    {
        SceneSwitcher.instance.Transition("Main Menu");
        StartCoroutine(ToggleMenu());

        TimeManager.instance.SaveValues();

        Application.Quit();
    }

    public void MusicToggle()
    {
        musicMuted = !musicMuted;

        crosses[3].SetActive(musicMuted);
        crosses[1].SetActive(musicMuted);
        

        if (musicMuted)
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                sliders[3].transform.GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
                sliders[3].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);

                sliders[3].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = false;
            }

            sliders[1].transform.GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
            sliders[1].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);

            sliders[1].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = false;
        }

        else
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                sliders[3].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                sliders[3].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;

                sliders[3].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = true;
            }

            sliders[1].transform.GetChild(0).GetComponent<Image>().color = Color.white;
            sliders[1].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;

            sliders[1].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = true;
        }

        PlayerPrefs.SetInt("musicMuted", musicMuted ? 1 : 0);
        AudioManager.instance.musicSource.mute = musicMuted;
    }

    public void SFXToggle()
    {
        sfxMuted = !sfxMuted;

        crosses[2].SetActive(sfxMuted);
        crosses[0].SetActive(sfxMuted);

        if (sfxMuted)
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                sliders[2].transform.GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
                sliders[2].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);

                sliders[2].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = false;
            }

            sliders[0].transform.GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
            sliders[0].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);

            sliders[0].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = false;
        }

        else
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                sliders[2].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                sliders[2].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;

                sliders[2].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = true;
            }

            sliders[0].transform.GetChild(0).GetComponent<Image>().color = Color.white;
            sliders[0].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;

            sliders[0].transform.GetChild(1).GetChild(0).GetComponentInParent<Slider>().interactable = true;
        }

        PlayerPrefs.SetInt("sfxMuted", sfxMuted ? 1 : 0);
        AudioManager.instance.SFXSource.mute = sfxMuted;
    }

    public void VSyncToggle()
    {
        vSyncActive = !vSyncActive;

        crosses[6].SetActive(vSyncActive);
        crosses[4].SetActive(vSyncActive);

        QualitySettings.vSyncCount = vSyncActive ? 1 : 0;

        PlayerPrefs.SetInt("vSyncActive", vSyncActive ? 1 : 0);
    }

    public void FullscreenToggle()
    {
        fullScreen = !fullScreen;

        crosses[7].SetActive(fullScreen);
        crosses[5].SetActive(fullScreen);

        if (fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        PlayerPrefs.SetInt("fsActive", fullScreen ? 1 : 0);
    }
    public IEnumerator ToggleMenu()
    {
        menuActive = !menuActive;

        yield return new WaitForSecondsRealtime(1f);

        mainMenu.SetActive(menuActive);
    }

    public void SetResolution(int resolution)
    {
        resolutions.value = resolution;
        pResolutions.value = resolution;

        switch (resolution)
        {
            case 0:
                Screen.SetResolution(640, 480, fullScreen);
                break;
            case 1:
                Screen.SetResolution(1280, 720, fullScreen);
                break;
            case 2:
                Screen.SetResolution(1920, 1080, fullScreen);
                break;
            case 3:
                Screen.SetResolution(2650, 1440, fullScreen);
                break;
            case 4:
                Screen.SetResolution(2048, 1080, fullScreen);
                break;
            case 5:
                Screen.SetResolution(3840, 2160, fullScreen);
                break;
            case 6:
                Screen.SetResolution(7680, 4320, fullScreen);
                break;
        }

        PlayerPrefs.SetInt("resolution", resolution);
    }

    public void RestartGame()
    {
        RestartValues();
        TimeManager.instance.saveExists = true;
        TimeManager.instance.hasWatch = true;

        TimeManager.instance.SaveValues();

        SceneSwitcher.instance.Transition("Loop1");

        AudioManager.instance.PlayMusic("Game");
    }

    public void RestartValues()
    {
        // Reset all data relating to progression
        TimeManager.instance.timeLeft = 720;
        TimeManager.instance.gameOver = false;
        TimeManager.instance.waveDone = false;
        TimeManager.instance.health = 5;
        TimeManager.instance.deathTimeElapsed = 0;
        TimeManager.instance.carriagesPassed = 0;
        TimeManager.instance.sinceRefill = 0;
        TimeManager.instance.currentLoop = 1;
        TimeManager.instance.saveExists = false;
        TimeManager.instance.hasWatch = false;

        TimeManager.instance.SaveValues();
    }

    public void CloseEndMenu()
    {
        RestartValues();

        TimeManager.instance.gameOverAnim.SetBool("GameOver", TimeManager.instance.gameOver);

        TimeManager.instance.ToggleSepia();

        AudioManager.instance.PlaySFXWithPitch("ButtonClick", 1f);
    }

    public void ButtonHover()
    {
        AudioManager.instance.PlaySFXWithPitch("ButtonHover", 1f);
    }

    public void CrossOut()
    {
        AudioManager.instance.PlaySFX("CrossOut");
    }
}
