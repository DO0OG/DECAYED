using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [Header("Personalize")]
    public Outline OL;
    public Player_Move PM;
    public InventoryManager IM;

    [Header("Function")]
    public GameObject secretDoor;
    public GameObject leverArm;

    bool isUnlock = false;
    bool hasKey = false;
    bool trig, open;
    public float smooth = 2.0f;
    public float DoorOpenAngle = 27.0f;
    private Vector3 defaulRot;
    private Vector3 openRot;

    public AudioSource Audio;
    public AudioClip doorSound;
    public AudioClip lockSound;
    public AudioClip keySound;
    public AudioClip secretDoorSound;

    // Start is called before the first frame update
    void Start()
    {
        IM = FindObjectOfType<InventoryManager>();
        PM = FindObjectOfType<Player_Move>();
        Audio = GetComponent<AudioSource>();
        defaulRot = leverArm.transform.eulerAngles;
        openRot = new Vector3(defaulRot.x + DoorOpenAngle, defaulRot.y, defaulRot.z);
        OL = this.GetComponent<Outline>();
        if (OL == null)
        {
            OL = this.gameObject.AddComponent<Outline>();
            OL.enabled = false;
        }
        leverArm.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool playSound = false;

        if (open && isUnlock)
        {
            leverArm.transform.eulerAngles = Vector3.Slerp(leverArm.transform.eulerAngles, openRot, Time.deltaTime * smooth);
            

            if(secretDoor.transform.position.x >= 277.8f)
            {
                secretDoor.transform.position = Vector3.Lerp(secretDoor.transform.position, new Vector3(secretDoor.transform.position.x - 0.1f, secretDoor.transform.position.y, secretDoor.transform.position.z), Time.deltaTime * smooth);
                if (!secretDoor.GetComponent<AudioSource>().isPlaying)
                {
                    secretDoor.GetComponent<AudioSource>().clip = secretDoorSound;
                    secretDoor.GetComponent<AudioSource>().volume = 0.5f;
                    secretDoor.GetComponent<AudioSource>().Play();
                }
            }
        }
        else if(!open && isUnlock)
        {
            leverArm.transform.eulerAngles = Vector3.Slerp(leverArm.transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
        }

        if (Input.GetMouseButtonDown(0) && OL.enabled && !PM.isPause)
        {
            if (hasKey)
            {
                foreach (var item in IM.Items)
                {
                    if (item.itemInfo.Contains(gameObject.name))
                    {
                        IM.Remove(item);
                        leverArm.SetActive(true);
                        Audio.PlayOneShot(keySound);
                        Invoke("ChangeStatus", 1.5f);
                        PM.P_Text.text = "Great. It's perfectly fit.";
                        PM.Invoke("P_ResetText", 5f);
                        break;
                    }
                }
            }

            if (isUnlock)
            {
                open = true;
                playSound = true;
            }
        }

        if (playSound && isUnlock)
        {
            Audio.clip = doorSound;
            Audio.pitch = 1.25f;
            Audio.volume = 0.1f;
            Audio.Play();
            playSound = false;
        }

        if (playSound && !isUnlock && !hasKey)
        {
            PM.P_Text.text = "It's not work. I need something to connect.";
            PM.Invoke("P_ResetText", 5f);
            playSound = false;
        }

        foreach (var item in IM.Items)
        {
            if (item.itemInfo.Contains(gameObject.name))
            {
                hasKey = true;
                break;
            }
        }


        if (PM != null)
        {
            if (PM.isPause)
            {
                Audio.Pause();
                secretDoor.GetComponent<AudioSource>().Pause();
            }
            else
            {
                Audio.UnPause();
                secretDoor.GetComponent<AudioSource>().UnPause();
            }
        }
    }

    void ChangeStatus()
    {
        isUnlock = true;
    }
}
