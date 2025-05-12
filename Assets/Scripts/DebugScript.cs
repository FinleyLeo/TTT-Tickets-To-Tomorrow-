using UnityEngine;
using TMPro;

public class DebugScript : MonoBehaviour
{
    public static DebugScript instance;

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

    [SerializeField] GameObject console;
    [SerializeField] TextMeshProUGUI fpsText, carriageText;

    bool isActive;

    float fps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        console.SetActive(isActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            isActive = !isActive;
            console.SetActive(isActive);
        }

        SetFPS();
        CarriagesPassed();
    }

    void SetFPS()
    {
        fps = 1 / Time.unscaledDeltaTime;

        fpsText.text = "FPS: " + Mathf.Round(fps);
    }

    void CarriagesPassed()
    {
        carriageText.text = "Carriages Passed: " + TimeManager.instance.carriagesPassed;
    }
}
