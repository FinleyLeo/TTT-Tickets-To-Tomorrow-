using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI startText;

    Animator anim;
    public Animator mountainAnim, settings, quitConfirm;
    float startdelay;

    [Header("Slider stuff")]
    public GameObject musicCross;
    public GameObject sfxCross;
    public Image sfxBack, musicBack, sfxFill, musicFill;
    bool sfxMuted, musicMuted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        startdelay += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.X) && startdelay >= 1.5f)
        {
            StartCoroutine(FlashText());
        }
    }

    IEnumerator FlashText()
    {
        startText.color = Color.white;
        yield return new WaitForSeconds(0.075f);
        startText.color = Color.black;
        yield return new WaitForSeconds(0.075f);
        startText.color = Color.white;
        yield return new WaitForSeconds(0.075f);
        startText.color = Color.black;
        yield return new WaitForSeconds(0.075f);
        startText.color = Color.white;
        yield return new WaitForSeconds(0.075f);
        startText.color = Color.black;
        yield return new WaitForSeconds(0.075f);
        startText.color = Color.white;
        yield return new WaitForSeconds(0.075f);
        startText.color = Color.black;
        yield return new WaitForSeconds(0.2f);

        anim.SetTrigger("Trans");
    }

    public void PlayGame()
    {
        // Load the game scene
        print("Loading game...");
        mountainAnim.SetInteger("Break", 1);
        mountainAnim.SetBool("InMenu", true);

        _SceneSwitcher.instance.Transition("Loop1");
    }
    public void OpenOptions()
    {
        // Open the options menu
        print("Opening options...");
        mountainAnim.SetInteger("Break", 2);
        mountainAnim.SetBool("InMenu", true);

        settings.SetBool("Opened", true);
    }

    public void QuitGame()
    {
        // Quit the game
        print("Quitting game...");
        mountainAnim.SetInteger("Break", 3);
        mountainAnim.SetBool("InMenu", true);

        quitConfirm.SetBool("Opened", true);
    }

    public void ConfirmQuit()
    {
        mountainAnim.SetBool("InMenu", false);
        quitConfirm.SetBool("Opened", false);

        Application.Quit();
    }

    public void CancelQuit()
    {
        mountainAnim.SetBool("InMenu", false);
        quitConfirm.SetBool("Opened", false);
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

    public void ExitSettings()
    {
        mountainAnim.SetBool("InMenu", false);
        settings.SetBool("Opened", false);
    }
}
