using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Level : MonoBehaviour
{
    public Vector2 levelSize = new Vector2(20f, 11.25f);

    private void Start()
    {        
        GameManager.currentLevel = this;
        AudioManager.Instance.PlayMusic("BGM1");

        // Get color adjustments on property
        ColorAdjustments colorAdjustments;
        GameObject.Find("Screen Effects").GetComponent<Volume>().profile.TryGet(out colorAdjustments);

        // make new hue parameter
        VolumeParameter<float> hue = new()
        {
            value = Random.Range(-4, 4) * 45f
        };

        // assign parameter
        if (colorAdjustments != null)
            colorAdjustments.hueShift.SetValue(hue);
    }
}