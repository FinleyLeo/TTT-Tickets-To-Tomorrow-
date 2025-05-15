using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin noise;
    public CinemachineCamera cam;

    float shakeTimer;

    public float defaultAmp = 0.2f;
    public float defaultFreq = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        noise.AmplitudeGain = defaultAmp;
        noise.FrequencyGain = defaultFreq;
    }

    // Update is called once per frame
    void Update()
    {
        shakeTimer -= Time.deltaTime;

        noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, defaultAmp, -shakeTimer);
        noise.FrequencyGain = Mathf.Lerp(noise.FrequencyGain, defaultFreq, -shakeTimer);

        if (TimeManager.instance.hasWatch)
        {
            SlowCam();
        }

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
            cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, 6.25f, Time.unscaledDeltaTime * 2f);
        }

        else
        {
            cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, 6.75f, Time.unscaledDeltaTime * 2f);
        }
    }
}
