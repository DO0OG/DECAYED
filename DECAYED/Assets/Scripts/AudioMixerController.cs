using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer AudioMixer;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    float masterValue;
    float BGMValue;
    float SFXValue;

    private void Awake()
    {
        SetSliderValueFromAudioMixer();

        MasterSlider.onValueChanged.AddListener(SetMasterVolume);
        BGMSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetSliderValueFromAudioMixer()
    {
        float masterVolume;
        if (AudioMixer.GetFloat("Master", out masterVolume))
        {
            MasterSlider.value = Mathf.Pow(10f, masterVolume / 20f);
        }

        float BGMVolume;
        if (AudioMixer.GetFloat("BGM", out BGMVolume))
        {
            BGMSlider.value = Mathf.Pow(10f, BGMVolume / 20f);
        }

        float SFXVolume;
        if (AudioMixer.GetFloat("SFX", out SFXVolume))
        {
            SFXSlider.value = Mathf.Pow(10f, SFXVolume / 20f);
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}