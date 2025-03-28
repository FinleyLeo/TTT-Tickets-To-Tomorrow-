using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI startText;

    Animator anim;
    public Animator mountainAnim, settingsAnim, quitConfirmAnim, playAnim;
    float startdelay;

    


    

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

        playAnim.SetBool("Opened", true);
    }

    public void Continue()
    {
        mountainAnim.SetBool("InMenu", false);
        playAnim.SetBool("Opened", false);

        StartCoroutine(DisableMenu());
    }

    public void NewGame()
    {
        mountainAnim.SetBool("InMenu", false);
        playAnim.SetBool("Opened", false);

        SceneSwitcher.instance.Transition("Loop1");
        StartCoroutine(DisableMenu());
    }

    public void OpenOptions()
    {
        // Open the options menu
        print("Opening options...");
        mountainAnim.SetInteger("Break", 2);
        mountainAnim.SetBool("InMenu", true);

        settingsAnim.SetBool("Opened", true);
    }

    public void QuitGame()
    {
        // Quit the game
        print("Quitting game...");
        mountainAnim.SetInteger("Break", 3);
        mountainAnim.SetBool("InMenu", true);

        quitConfirmAnim.SetBool("Opened", true);
    }

    public void ConfirmQuit()
    {
        mountainAnim.SetBool("InMenu", false);
        quitConfirmAnim.SetBool("Opened", false);

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

    IEnumerator DisableMenu()
    {
        yield return new WaitForSeconds(0.99f);
        gameObject.SetActive(false);
    }
}
