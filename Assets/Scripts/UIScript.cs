using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;

    public bool paused;

    public Animator pauseAnim, quitChoice, menuAnim;

    public Image pauseDarken;
    bool menuActive = true;

    public GameObject mainMenu;

    [Header("Slider stuff")]
    public GameObject musicCross;
    public GameObject sfxCross;
    public Image sfxBack, musicBack, sfxFill, musicFill;
    bool sfxMuted, musicMuted;

    [Header("Toggles")]
    bool vSyncActive, fullScreen;
    bool quitToggle;
    public GameObject vSyncCross, fullScreenCross;

    Transform pauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            GameObject.Find("Main Menu").SetActive(true);
        }

        pauseMenu = pauseAnim.transform;
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

        if (Input.GetKeyDown(KeyCode.Escape))
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
        quitToggle = !quitToggle;

        SceneSwitcher.instance.Transition("Main Menu");
        StartCoroutine(DisableMenu());

        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        paused = false;
        menuAnim.SetTrigger("Reset");
        pauseAnim.SetBool("Paused", paused);
        quitChoice.SetBool("Opened", quitToggle);
    }

    public void Quit()
    {
        SceneSwitcher.instance.Transition("Main Menu");
        StartCoroutine(DisableMenu());

        Application.Quit();
    }

    public void MusicToggle()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            musicCross = pauseMenu.Find("Panel").Find("Sliders").Find("Music Volume").Find("Icon").GetChild(0).gameObject;
            musicBack = pauseMenu.Find("Panel").Find("Sliders").Find("Music Volume").Find("Slider").GetChild(0).GetComponent<Image>();
            musicFill = pauseMenu.Find("Panel").Find("Sliders").Find("Music Volume").Find("Slider").Find("Fill Area").GetChild(0).GetComponent<Image>();
        }

        else
        {
            musicCross = mainMenu.transform.Find("Settings").Find("Panel").Find("Sliders").Find("Music Volume").Find("Icon").GetChild(0).gameObject;
            musicBack = mainMenu.transform.Find("Settings").Find("Panel").Find("Sliders").Find("Music Volume").Find("Slider").GetChild(0).GetComponent<Image>();
            musicFill = mainMenu.transform.Find("Settings").Find("Panel").Find("Sliders").Find("Music Volume").Find("Slider").Find("Fill Area").GetChild(0).GetComponent<Image>();
        }

        musicMuted = !musicMuted;

        musicCross.SetActive(musicMuted);

        if (musicMuted)
        {
            musicBack.color = new Color(0.75f, 0.75f, 0.75f);
            musicFill.color = new Color(0.75f, 0.75f, 0.75f);

            musicFill.GetComponentInParent<Slider>().interactable = false;
        }

        else
        {
            musicBack.color = Color.white;
            musicFill.color = Color.white;

            musicFill.GetComponentInParent<Slider>().interactable = true;
        }
    }

    public void SFXToggle()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            sfxCross = pauseMenu.Find("Panel").Find("Sliders").Find("SFX Volume").Find("Icon").GetChild(0).gameObject;
            sfxBack = pauseMenu.Find("Panel").Find("Sliders").Find("SFX Volume").Find("Slider").GetChild(0).GetComponent<Image>();
            sfxFill = pauseMenu.Find("Panel").Find("Sliders").Find("SFX Volume").Find("Slider").Find("Fill Area").GetChild(0).GetComponent<Image>();
        }

        else
        {
            musicCross = mainMenu.transform.Find("Settings").Find("Panel").Find("Sliders").Find("SFX Volume").Find("Icon").GetChild(0).gameObject;
            musicBack = mainMenu.transform.Find("Settings").Find("Panel").Find("Sliders").Find("SFX Volume").Find("Slider").GetChild(0).GetComponent<Image>();
            musicFill = mainMenu.transform.Find("Settings").Find("Panel").Find("Sliders").Find("SFX Volume").Find("Slider").Find("Fill Area").GetChild(0).GetComponent<Image>();
        }

        sfxMuted = !sfxMuted;

        sfxCross.SetActive(sfxMuted);

        if (sfxMuted)
        {
            sfxBack.color = new Color(0.75f, 0.75f, 0.75f);
            sfxFill.color = new Color(0.75f, 0.75f, 0.75f);

            sfxFill.GetComponentInParent<Slider>().interactable = false;
        }

        else
        {
            sfxBack.color = Color.white;
            sfxFill.color = Color.white;

            sfxFill.GetComponentInParent<Slider>().interactable = true;
        }
    }

    public void VSyncToggle()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            vSyncCross = pauseMenu.Find("Panel").Find("VSync").Find("Toggle").GetChild(2).gameObject;
        }

        vSyncActive = !vSyncActive;

        vSyncCross.SetActive(vSyncActive);

        QualitySettings.vSyncCount = vSyncActive ? 1 : 0;

        print(QualitySettings.vSyncCount);
    }

    public void FullscreenToggle()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            fullScreenCross = pauseMenu.Find("Panel").Find("Full screen").Find("Toggle").GetChild(2).gameObject;
        }

        fullScreen = !fullScreen;

        fullScreenCross.SetActive(fullScreen);

        if (fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
    public IEnumerator DisableMenu()
    {
        menuActive = !menuActive;

        yield return new WaitForSecondsRealtime(0.99f);

        mainMenu.SetActive(menuActive);
    }


    public void SetResolution(int resolution)
    {
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

        Debug.Log(Screen.currentResolution);
    }
}
