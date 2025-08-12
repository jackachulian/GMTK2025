using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private PlayerPrefSetter[] prefSetters;
    [SerializeField] private GameObject menuObject;
    [SerializeField] private TMP_Dropdown resDropdown;

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            Array.ForEach(prefSetters, pref => pref.Sync());  
        }
        resDropdown.value = PlayerPrefs.GetInt("resolution", 0);

        menuObject.SetActive(enabled);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        SetGameColor();

        // Setup resolution selector
        foreach (var res in Screen.resolutions)
        {
            TMP_Dropdown.OptionData data = new()
            {
                text = res.ToString()
            };
            resDropdown.options.Add(data);
        }
        CalculateLetterbox();
    }

    public void SetMusicVolume(int v)
    {
        AudioManager.Instance.musicSource.volume = v / 10f;
    }

    public void PlaySound(string s)
    {
        AudioManager.Instance.PlaySfx(s);
    }

    public void SetResolution(int selection)
    {
        // resolution dropdown does not use playerprefsetter, set it manually
        PlayerPrefs.SetInt("resolution", selection);
        var res = Screen.resolutions[selection];
        
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        StartCoroutine(SetResoltutionHelper());
    }

    // resolution is set at end of frame, not when function is called
    private IEnumerator SetResoltutionHelper()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        CalculateLetterbox();
        Canvas.ForceUpdateCanvases();
    }

    private void CalculateLetterbox()
    {
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {  
            Rect rect = Camera.main.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;
            
            Camera.main.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = Camera.main.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            Camera.main.rect = rect;
        }
    }

    public void SetScreenMode(int mode)
    {
        var fsm = mode switch
        {
            0 => FullScreenMode.FullScreenWindow,
            1 => FullScreenMode.ExclusiveFullScreen,
            2 => FullScreenMode.Windowed,
            _ => FullScreenMode.FullScreenWindow,
        };
        Screen.fullScreenMode = fsm;
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
