using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class Chromatic_Controller : MonoBehaviour
{
    public VolumeProfile volumeProfile;

    ChromaticAberration ca;
    public float idle = 0.2f;
    public float sprint = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // VolumeProfile에서 ChromaticAberration 설정을 가져오기
        volumeProfile.TryGet(out ca);
    }

    // Update is called once per frame
    void Update()
    {
        SetIntensity();
    }

    void SetIntensity()
    {
        if (GameObject.Find("Char").GetComponent<Player_Move>().isSprint)
        {
            ca.intensity.value = Mathf.Lerp(ca.intensity.value, sprint, Time.deltaTime * 10f);
        }
        else if (!GameObject.Find("Char").GetComponent<Player_Move>().isSprint)
        {
            ca.intensity.value = Mathf.Lerp(ca.intensity.value, idle, Time.deltaTime * 10f);
        }
    }
}
