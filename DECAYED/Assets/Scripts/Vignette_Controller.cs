using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class Vignette_Controller : MonoBehaviour
{
    public VolumeProfile volumeProfile;
    Vignette vnt;
    public Color originalColor;

    public Player_Health PH;

    public Color lerpedColor;
    public Color targetColor = new Color(1f, 0f, 0f); // Red color

    public float intensityOrigin;
    public float targetIntensity = 0.55f;
    public float lerpedIntensity;

    // Start is called before the first frame update
    void Start()
    {
        PH = FindObjectOfType<Player_Health>();
        volumeProfile.TryGet(out vnt);
        vnt.color.value = Color.black;
        originalColor = vnt.color.value;
        vnt.intensity.value = 0.425f;
        intensityOrigin = vnt.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
        if(PH != null)
        {
            float healthPercentage = PH.currentHealth / PH.maxHP;

            // 체력이 감소할 때 intensity 증가
            if (healthPercentage < 1.0f)
            {
                lerpedIntensity = Mathf.Lerp(intensityOrigin, targetIntensity, 1f - healthPercentage);
            }
            else
            {
                // 체력이 회복되면 intensityOrigin으로 돌아감
                lerpedIntensity = intensityOrigin;
            }

            lerpedColor = Color.Lerp(originalColor, targetColor, 1f - healthPercentage);

            vnt.color.value = lerpedColor;
            vnt.intensity.value = lerpedIntensity;
        }
    }
}
