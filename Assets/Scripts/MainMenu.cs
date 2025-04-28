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

    public UIScript UI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    // Update is called once per frame
    void Update()
    {
        startdelay += Time.deltaTime;

        if (Input.anyKeyDown && startdelay >= 1.5f)
        {
            StartCoroutine(FlashText());
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
    }

    public void Continue()
    {
        if (TimeManager.instance.saveExists)
        {
            mountainAnim.SetBool("InMenu", false);
            playAnim.SetBool("Opened", false);

            TimeManager.instance.LoadValues();

            SceneSwitcher.instance.Transition("Loop1");
            StartCoroutine(UI.DisableMenu());
        }
    }

    public void NewGame()
    {
        mountainAnim.SetBool("InMenu", false);
        playAnim.SetBool("Opened", false);

        UIScript.instance.RestartValues();
        TimeManager.instance.deathTimeElapsed = 0;
        TimeManager.instance.saveExists = true;

        SceneSwitcher.instance.Transition("Loop1");
        StartCoroutine(UI.DisableMenu());
    }

    public void OpenOptions()
    {
        mountainAnim.SetInteger("Break", 2);
        mountainAnim.SetBool("InMenu", true);

        settingsAnim.SetBool("Opened", true);
    }

    public void QuitGame()
    {
        mountainAnim.SetInteger("Break", 3);
        mountainAnim.SetBool("InMenu", true);

        quitConfirmAnim.SetBool("Opened", true);
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
}
