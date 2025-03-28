using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript instance;

    public bool paused;

    public Animator pauseAnim;

    public Image pauseDarken;

    [Header("Slider stuff")]
    public GameObject musicCross;
    public GameObject sfxCross;
    public Image sfxBack, musicBack, sfxFill, musicFill;
    bool sfxMuted, musicMuted;

    bool vSyncActive;
    public GameObject vSyncCross;

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

    public void MusicToggle()
    {
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
        vSyncActive = !vSyncActive;

        vSyncCross.SetActive(vSyncActive);

        QualitySettings.vSyncCount = vSyncActive ? 1 : 0;

        print(QualitySettings.vSyncCount);
    }
}
