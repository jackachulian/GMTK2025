using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private PlayerPrefSetter[] prefSetters;
    [SerializeField] private GameObject menuObject;

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            Array.ForEach(prefSetters, pref => pref.Sync());  
        }

        menuObject.SetActive(enabled);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        SetGameColor();

    }

    public void SetMusicVolume(int v)
    {
        AudioManager.Instance.musicSource.volume = v / 10f;
    }

    public void PlaySound(string s)
    {
        AudioManager.Instance.PlaySfx(s);
    }

    public void SetGameColor()
    {
        // Get color adjustments on property
        GameObject.Find("Screen Effects").GetComponent<Volume>().profile.TryGet(
            out UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments
        );

        // make new hue parameter
        int pref = PlayerPrefs.GetInt("game-color");
        VolumeParameter<float> hue = new()
        {
            value = (pref == 0) ? UnityEngine.Random.Range(-4, 4) * 45f : (pref - 5) * 45
        };

        // assign parameter
        if (colorAdjustments != null)
            colorAdjustments.hueShift.SetValue(hue);
    }
}
