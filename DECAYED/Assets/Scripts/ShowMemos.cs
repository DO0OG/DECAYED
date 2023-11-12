using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMemos : MonoBehaviour
{
    [Header("Personalize")]
    public Outline OL;
    public Player_Move PM;
    public Camera_Controller CC;
    public FlashLight_Follow FF;

    [Header("Function")]
    public Image memoImage;
    public TextMeshProUGUI memoText;
    public string memoString;
    public bool enable = false;
    public GameObject savePoint;

    public AudioSource Audio;
    public AudioClip paperSound;

    // Start is called before the first frame update
    void Start()
    {
        CC = FindObjectOfType<Camera_Controller>();
        FF = FindObjectOfType<FlashLight_Follow>();
        PM = FindObjectOfType<Player_Move>();
        Audio = GetComponent<AudioSource>();
        OL = this.GetComponent<Outline>();
        if (OL == null)
        {
            OL = this.gameObject.AddComponent<Outline>();
            OL.enabled = false;
        }
        memoImage.enabled = false;
        memoText.enabled = false;
        savePoint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool playSound = false;

        if (Input.GetMouseButtonDown(0) && OL.enabled && !PM.isPause)
        {
            if (!enable)
            {
                playSound = true;

                if (savePoint != null)
                {
                    savePoint.SetActive(true);
                }
            }

            enable = !enable;
            CC.enabled = !enable;
            FF.enabled = !enable;
            PM.enabled = !enable;
        }

        if (enable)
        {
            memoImage.enabled = true;
            memoText.enabled = true;

            memoText.text = memoString;
        }
        else
        {
            memoImage.enabled = false;
            memoText.enabled = false;

            memoText.text = "";
        }

        if (memoImage.enabled && !PM.isPause)
        {
            Time.timeScale = 0f;
        }
        else if(!memoImage.enabled && !PM.isPause)
        {
            Time.timeScale = 1f;
        }

        if (playSound)
        {
            Audio.clip = paperSound;
            Audio.pitch = 1.25f;
            Audio.volume = 0.1f;
            Audio.Play();
            playSound = false;
        }

        if (PM != null)
        {
            if (PM.isPause)
            {
                Audio.Pause();
            }
            else
            {
                Audio.UnPause();
            }
        }
    }
}
