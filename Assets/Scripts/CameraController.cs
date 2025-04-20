using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin noise;
    public CinemachineCamera camera;

    float shakeTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        noise.AmplitudeGain = 0.2f;
        noise.FrequencyGain = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        shakeTimer -= Time.deltaTime;

        noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, 0.2f, -shakeTimer);
        noise.FrequencyGain = Mathf.Lerp(noise.FrequencyGain, 0.1f, -shakeTimer);

        SlowCam();
    }

    public void Shake(float intensity, float frequency, float time)
    {
        if (noise.AmplitudeGain < 3f)
        {
            noise.AmplitudeGain += intensity;
        }

        noise.FrequencyGain = frequency;
        shakeTimer = time;
    }

    void SlowCam()
    {
        if (TimeManager.instance.slowTime)
        {
            camera.Lens.OrthographicSize = Mathf.Lerp(camera.Lens.OrthographicSize, 6.25f, Time.unscaledDeltaTime * 2f);
        }

        else
        {
            camera.Lens.OrthographicSize = Mathf.Lerp(camera.Lens.OrthographicSize, 6.75f, Time.unscaledDeltaTime * 2f);
        }
    }
}
