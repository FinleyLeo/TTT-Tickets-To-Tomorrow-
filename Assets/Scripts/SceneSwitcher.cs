using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher instance;

    public Animator transition;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transition.SetBool("Fading", true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Transition(string scene)
    {
        StartCoroutine(Transitioning(scene));
    }

    IEnumerator Transitioning(string scene)
    {
        transition.SetBool("Fading", false);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
        transition.SetBool("Fading", true);
    }

}