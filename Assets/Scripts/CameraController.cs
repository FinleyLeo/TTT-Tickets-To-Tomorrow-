using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin noise;

    float shakeTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        noise.AmplitudeGain = 0.3f;
        noise.FrequencyGain = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0)
            {
                noise.AmplitudeGain = 0.3f;
                noise.FrequencyGain = 0.1f;
            }
        }
    }

    public void Shake(float intensity, float frequency, float time)
    {   
        noise.AmplitudeGain = intensity;
        noise.FrequencyGain = frequency;
        shakeTimer = time;
    }
}
