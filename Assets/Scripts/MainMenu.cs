using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI startText;

    Animator anim;
    public Animator mountainAnim;

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

        if (Input.GetKeyDown(KeyCode.X) && startdelay >= 2f)
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

        SceneSwitcher.instance.Transition("Loop1");
    }
    public void OpenOptions()
    {
        // Open the options menu
        print("Opening options...");
        mountainAnim.SetInteger("Break", 2);
        mountainAnim.SetBool("InMenu", true);
    }

    public void QuitGame()
    {
        // Quit the game
        print("Quitting game...");
        mountainAnim.SetInteger("Break", 3);
        mountainAnim.SetBool("InMenu", true);
    }

    public void ExitSettings()
    {

    }
}
