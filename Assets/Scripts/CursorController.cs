using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    [SerializeField] GameObject crosshair;

    Vector3 chPos;

    bool isPaused;

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crosshair.SetActive(false);
        Cursor.visible = true;
    }
    

    void OnSceneChange(Scene current, Scene next)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            crosshair.SetActive(false);
            Cursor.visible = true;
        }

        else
        {
            crosshair.SetActive(true);
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (!isPaused && UIScript.instance.paused)
            {
                crosshair.SetActive(false);
                Cursor.visible = true;
                isPaused = true;

                Debug.Log("Custom Cursor");
            }

            else
            {
                if (!UIScript.instance.paused)
                {
                    crosshair.SetActive(true);
                    Cursor.visible = false;
                    isPaused = false;

                    Debug.Log("Default Cursor");
                }
            }
        }

        chPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        crosshair.transform.position = chPos;
    }
}
