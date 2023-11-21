using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger_Controller1 : MonoBehaviour
{
    public Player_Move PM;
    public GameObject eventFirst;

    public AudioClip eventSound;

    public string eventString;

    public bool isTrig = false;

    // Start is called before the first frame update
    void Start()
    {
        PM = FindObjectOfType<Player_Move>();
        //Invoke(nameof(StartDisable), 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartDisable()
    {
        eventFirst.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        eventFirst.SetActive(true);
        if (!eventFirst.GetComponent<AudioSource>().isPlaying)
        {
            eventFirst.GetComponent<AudioSource>().clip = eventSound;
            eventFirst.GetComponent<AudioSource>().volume = 0.5f;
            eventFirst.GetComponent<AudioSource>().Play();
        }
        PM.P_Text.text = eventString;
        PM.Invoke("P_ResetText", 8f);
        Destroy(gameObject);
    }
}
