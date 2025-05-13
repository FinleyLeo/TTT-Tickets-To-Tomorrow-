using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI startText;

    Animator anim;
    public Animator mountainAnim, settingsAnim, quitConfirmAnim, playAnim;
    public float startdelay;
    public TextMeshProUGUI continueButton;

    bool menuOpen = false;

    public UIScript UI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;

        AudioManager.instance.PlaySFX("IntroGuitar");
    }

    // Update is called once per frame
    void Update()
    {
        startdelay += Time.deltaTime;

        if (Input.anyKeyDown && startdelay >= 1f && !menuOpen)
        {
            menuOpen = true;
            StartCoroutine(FlashText());

            AudioManager.instance.PlaySFX("PressPlay");
        }

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            if (TimeManager.instance.saveExists)
            {
                continueButton.color = Color.white;
            }

            else
            {
                continueButton.color = Color.grey;
            }
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
        mountainAnim.SetInteger("Break", 1);
        mountainAnim.SetBool("InMenu", true);

        playAnim.SetBool("Opened", true);

        AudioManager.instance.PlaySFX("Shatter");
        AudioManager.instance.PlaySFX("Shoot");
    }

    public void Continue()
    {
        if (TimeManager.instance.saveExists)
        {
            mountainAnim.SetBool("InMenu", false);
            playAnim.SetBool("Opened", false);

            TimeManager.instance.LoadValues();

            if (TimeManager.instance.currentLoop == 5)
            {
                SceneSwitcher.instance.Transition("PreBoss");
            }

            else
            {
                SceneSwitcher.instance.Transition("Loop1");
            }

            StartCoroutine(UI.ToggleMenu());

            menuOpen = false;

            AudioManager.instance.PlayMusic("Game");
        }
    }

    public void NewGame()
    {
        mountainAnim.SetBool("InMenu", false);
        playAnim.SetBool("Opened", false);

        UIScript.instance.RestartValues();


        SceneSwitcher.instance.Transition("Tutorial");
        StartCoroutine(UI.ToggleMenu());

        menuOpen = false;

        AudioManager.instance.PlayMusic("Game");
    }

    public void OpenOptions()
    {
        mountainAnim.SetInteger("Break", 2);
        mountainAnim.SetBool("InMenu", true);

        settingsAnim.SetBool("Opened", true);

        AudioManager.instance.PlaySFX("Shatter");
        AudioManager.instance.PlaySFX("Shoot");
    }

    public void QuitGame()
    {
        mountainAnim.SetInteger("Break", 3);
        mountainAnim.SetBool("InMenu", true);

        quitConfirmAnim.SetBool("Opened", true);

        AudioManager.instance.PlaySFX("Shatter");
        AudioManager.instance.PlaySFX("Shoot");
    }

    public void ConfirmQuit()
    {
        mountainAnim.SetBool("InMenu", false);
        quitConfirmAnim.SetBool("Opened", false);

        TimeManager.instance.SaveValues();

        Application.Quit();
    }

    public void CancelQuit()
    {
        mountainAnim.SetBool("InMenu", false);
        quitConfirmAnim.SetBool("Opened", false);
    }

    public void ExitSettings()
    {
        mountainAnim.SetBool("InMenu", false);
        settingsAnim.SetBool("Opened", false);
    }

    public void BottleClink()
    {
        int ran = Random.Range(0, 2);
        int x = Random.Range(-2, 3);
        float pitch = 1;

        for (int i = 0; i < x; i++)
        {
            pitch *= 1.059463f;
        }

        switch (ran)
        {
            case 0:
                AudioManager.instance.PlaySFXWithPitch("Clink1", pitch);
                break;
            case 1:
                AudioManager.instance.PlaySFXWithPitch("Clink2", pitch);
                break;
            case 2:
                AudioManager.instance.PlaySFXWithPitch("Clink3", pitch);
                break;
        }
    }
}
