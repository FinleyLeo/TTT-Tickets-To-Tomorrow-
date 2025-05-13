using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    Animator anim;
    public TextMeshProUGUI textComponent;
    public GameObject tutOptions;
    bool choiceGiven;

    [TextArea(1, 3)]
    public string[] lines; // REMINDER: sentences over 3 lines long dont fit in dialogue box
    public float typingSpeed;
    int index, endIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        textComponent.text = string.Empty;
        anim.SetBool("Open", false);

        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            Time.timeScale = 0;
            StartDialogue(0, 3);
        }

        else
        {
            switch (TimeManager.instance.currentLoop)
            {
                case 1:
                    StartDialogue(25, 25);
                    break;
                case 2:
                    StartDialogue(26, 27);
                    break;
                case 3:
                    StartDialogue(28, 28);
                    break;
                case 4:
                    StartDialogue(29, 29);
                    break;
                case 5:
                    AudioManager.instance.musicSource.Stop();
                    StartDialogue(30, 30);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && TimeManager.instance.inDialogue)
        {
            if (!choiceGiven)
            {
                if (textComponent.text == lines[index])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    textComponent.text = lines[index];
                }
            }

            else
            {
                tutOptions.SetActive(true);
            }
        }
    }

    public void StartDialogue(int startIndex, int end)
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;

        anim.SetBool("Open", true);
        index = startIndex;
        endIndex = end;
        TimeManager.instance.inDialogue = true;

        StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        textComponent.text = string.Empty;
        anim.SetBool("Open", false);
        Time.timeScale = 1;
        TimeManager.instance.inDialogue = false;
        // Optionally, you can add logic here to end the dialogue or trigger another event.

        Debug.Log("End of dialogue");
    }

    IEnumerator TypeLine()
    {
        yield return new WaitForSecondsRealtime(0.25f);

        foreach (char letter in lines[index].ToCharArray())
        {
            textComponent.text += letter;
            AudioManager.instance.PlaySFX("Dialogue");
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        if (index == 3)
        {
            tutOptions.SetActive(true);
        }
    }

    void NextLine()
    {
        if (index < endIndex)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());

            if (index == 3)
            {
                choiceGiven = true;
            }
        }

        else
        {
            EndDialogue();
        }
    }

    public void SkipTut()
    {
        choiceGiven = false;
        tutOptions.SetActive(false);
        anim.SetBool("Open", false);
        Time.timeScale = 1;

        UIScript.instance.RestartValues();

        TimeManager.instance.inDialogue = false;
        TimeManager.instance.tutorialComplete = true;
        TimeManager.instance.saveExists = true;
        TimeManager.instance.hasWatch = true;

        AudioManager.instance.PlaySFX("ButtonClick");

        SceneSwitcher.instance.Transition("Loop1");
    }

    public void StartTut()
    {
        choiceGiven = false;
        tutOptions.SetActive(false);
        AudioManager.instance.PlaySFX("ButtonClick");

        StartDialogue(4, 4);
    }
}
