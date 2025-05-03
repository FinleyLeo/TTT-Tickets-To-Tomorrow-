using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] Slider sfxSlider, musicSlider;
    [SerializeField] Slider p_sfxSlider, p_musicSlider;

    public Sound[] SFXSounds, musicSounds;
    public AudioSource SFXSource, musicSource;

    public AudioMixer musicMixer;
    public AudioMixer SFXMixer;

    public float defaultPitch = 1f;

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
        SetValues();
        PlayMusic("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        PitchCalc();
    }

    void PitchCalc()
    {
        if (TimeManager.instance.slowTime || UIScript.instance.paused)
        {
            if (defaultPitch > 0.4f)
            {
                defaultPitch = Mathf.Lerp(defaultPitch, 0.4f, Time.unscaledDeltaTime * 6);
            }

            else
            {
                defaultPitch = 0.4f;
            }
        }

        else
        {
            if (defaultPitch < 1f)
            {
                defaultPitch = Mathf.Lerp(defaultPitch, 1f, Time.unscaledDeltaTime * 6);
            }

            else
            {
                defaultPitch = 1f;
            }
        }

        musicSource.pitch = defaultPitch;
    }

    void SetValues()
    {
        if (PlayerPrefs.HasKey("sfxVol"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVol");
            p_sfxSlider.value = PlayerPrefs.GetFloat("sfxVol");

            SFXMixer.SetFloat("sfxVol", Mathf.Log10(PlayerPrefs.GetFloat("sfxVol")) * 20);
        }

        else
        {
            PlayerPrefs.SetFloat("sfxVol", 0.75f);
        }

        if (PlayerPrefs.HasKey("musicVol"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVol");
            p_musicSlider.value = PlayerPrefs.GetFloat("musicVol");

            musicMixer.SetFloat("musicVol", Mathf.Log10(PlayerPrefs.GetFloat("musicVol")) * 20);
        }

        else
        {
            PlayerPrefs.SetFloat("musicVol", 0.75f);
        }
    }

    public void PlaySFXWithPitch(string name, float pitch)
    {
        Sound s = Array.Find(SFXSounds, x => x.soundName == name);

        if (s == null)
        {
            print("sound not found");
        }

        else
        {
            SFXSource.pitch = pitch * defaultPitch;
            SFXSource.PlayOneShot(s.sound);
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(SFXSounds, x => x.soundName == name);
        if (s == null)
        {
            print("sound not found");
        }
        else
        {
            SFXSource.pitch = defaultPitch;
            SFXSource.PlayOneShot(s.sound);
        }
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.soundName == name);

        if (s == null)
        {
            print("music not found");
        }

        else
        {
            musicSource.clip = s.sound;
            musicSource.Play();
        }
    }

    public void SFXVolume(float volume)
    {
        sfxSlider.value = volume;
        p_sfxSlider.value = volume;

        PlayerPrefs.SetFloat("sfxVol", volume);
        SFXMixer.SetFloat("sfxVol", Mathf.Log10(volume) * 20);
    }

    public void MusicVolume(float volume)
    {
        musicSlider.value = volume;
        p_musicSlider.value = volume;

        PlayerPrefs.SetFloat("musicVol", volume);
        musicMixer.SetFloat("musicVol", Mathf.Log10(volume) * 20);
    }

    
}
