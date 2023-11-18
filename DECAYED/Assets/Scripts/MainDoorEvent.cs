using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoorEvent : MonoBehaviour
{
    public Player_Move PM;
    public GameObject mainDoor;
    public GameObject eventDoor;

    public AudioClip eventSound;

    public static bool isTrig = false;

    // Start is called before the first frame update
    void Start()
    {
        PM = FindObjectOfType<Player_Move>();
        mainDoor.SetActive(true);
        eventDoor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isTrig)
        {
            mainDoor.SetActive(false);
            eventDoor.SetActive(true);
            if (!eventDoor.GetComponent<AudioSource>().isPlaying)
            {
                eventDoor.GetComponent<AudioSource>().clip = eventSound;
                eventDoor.GetComponent<AudioSource>().volume = 0.5f;
                eventDoor.GetComponent<AudioSource>().Play();
            }
            PM.P_Text.text = "무슨... 입구에서 난 소리같은데..?";
            PM.Invoke("P_ResetText", 5f);
        }
        isTrig = true;
    }
}
