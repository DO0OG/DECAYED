using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Controller : MonoBehaviour
{
    public Player_Move PM;

    public AudioSource Source;
    public AudioClip Clip;

    // Start is called before the first frame update
    void Start()
    {
        PM = FindObjectOfType<Player_Move>();
        Source = GetComponent<AudioSource>();
        Source.clip = Clip;
        Source.loop = true;
        Source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (PM.isPause)
        {
            Source.Pause();
        }
        else if (!PM.isPause)
        {
            Source.UnPause();
        }
    }
}
