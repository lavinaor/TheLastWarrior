using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class SoundMixerMeneger : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider MasterVolumeslider;
    [SerializeField] private Slider MusicVolumeslider;
    [SerializeField] private Slider SoundFXVolumeslider;

    private void Awake()
    {
        MasterVolumeslider.value = PlayerPrefs.GetFloat("MasterVolume");
        MusicVolumeslider.value = PlayerPrefs.GetFloat("MusicVolume");
        SoundFXVolumeslider.value = PlayerPrefs.GetFloat("SoundFXVolume");
        SetMasterVolume(MasterVolumeslider.value);
        SetMusicVolume(MusicVolumeslider.value);
        SetSoundFXVolume(SoundFXVolumeslider.value);
    }

    public void SetMasterVolume(float level)
    {
        PlayerPrefs.SetFloat("MasterVolume", level);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        PlayerPrefs.SetFloat("MusicVolume", level);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSoundFXVolume(float level)
    {
        PlayerPrefs.SetFloat("SoundFXVolume", level);
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);
    }
}
