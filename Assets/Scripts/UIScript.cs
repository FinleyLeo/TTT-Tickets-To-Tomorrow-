using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;

    public bool paused;

    public Animator pauseAnim, quitChoice, menuAnim;
    public Image pauseDarken;

    bool menuActive = true;

    // Menus
    public GameObject mainMenu;

    bool sfxMuted, musicMuted;

    [SerializeField] GameObject[] crosses;
    [SerializeField] Image[] sliderFills;
    [SerializeField] Image[] sliderBacks;
    [SerializeField] GameObject[] sliders;

    [SerializeField] TMP_Dropdown resolutions, pResolutions;

    // Toggles
    bool vSyncActive, fullScreen;
    bool quitToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            GameObject.Find("Main Menu").SetActive(true);
        }
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
        musicMuted = !musicMuted;

        crosses[3].SetActive(musicMuted);
        crosses[1].SetActive(musicMuted);
        

        if (musicMuted)
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                sliders[3].transform.GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
                sliders[3].transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);

                sliderFills[3].GetComponentInParent<Slider>().interactable = false;
            }


            sliderBacks[1].color = new Color(0.75f, 0.75f, 0.75f);
            sliderFills[1].color = new Color(0.75f, 0.75f, 0.75f);

            sliderFills[1].GetComponentInParent<Slider>().interactable = false;
        }

        else
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                sliderBacks[3].color = Color.white;
                sliderFills[3].color = Color.white;

                sliderFills[3].GetComponentInParent<Slider>().interactable = true;
            }


            sliderBacks[1].color = Color.white;
            sliderFills[1].color = Color.white;

            sliderFills[1].GetComponentInParent<Slider>().interactable = true;
        }
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
                sliderBacks[2].color = new Color(0.75f, 0.75f, 0.75f);
                sliderFills[2].color = new Color(0.75f, 0.75f, 0.75f);

                sliderFills[2].GetComponentInParent<Slider>().interactable = false;
            }

            sliderBacks[0].color = new Color(0.75f, 0.75f, 0.75f);
            sliderFills[0].color = new Color(0.75f, 0.75f, 0.75f);

            sliderBacks[0].GetComponentInParent<Slider>().interactable = false;
        }

        else
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                sliderBacks[2].color = Color.white;
                sliderFills[2].color = Color.white;

                sliderFills[2].GetComponentInParent<Slider>().interactable = true;
            }


            sliderBacks[0].color = Color.white;
            sliderFills[0].color = Color.white;

            sliderFills[0].GetComponentInParent<Slider>().interactable = true;
        }
    }

    public void VSyncToggle()
    {
        vSyncActive = !vSyncActive;

        crosses[6].SetActive(vSyncActive);
        crosses[4].SetActive(vSyncActive);

        QualitySettings.vSyncCount = vSyncActive ? 1 : 0;

        print(QualitySettings.vSyncCount);
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
    }
    public IEnumerator DisableMenu()
    {
        menuActive = !menuActive;

        yield return new WaitForSecondsRealtime(0.99f);

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
    }
}
