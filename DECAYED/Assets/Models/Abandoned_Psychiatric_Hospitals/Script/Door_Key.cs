using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Door_Key : MonoBehaviour
{
    [Header("Personalize")]
    public Outline OL;
    public Player_Move PM;
    public InventoryManager IM;

    bool isUnlock = false;
    bool hasKey = false;
    bool trig, open;
    public float smooth = 2.0f;
    public float DoorOpenAngle = 90.0f;
    private Vector3 defaulRot;
    private Vector3 openRot;

    public AudioSource Audio;
    public AudioClip doorSound;
    public AudioClip lockSound;
    public AudioClip keySound;

    // Start is called before the first frame update
    void Start()
    {
        IM = FindObjectOfType<InventoryManager>();
        PM = FindObjectOfType<Player_Move>();
        Audio = GetComponent<AudioSource>();
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);
        OL = this.GetComponent<Outline>();
        if (OL == null)
        {
            OL = this.gameObject.AddComponent<Outline>();
            OL.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool playSound = false;

        if (open && isUnlock)
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        }
        else if(!open && isUnlock)
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
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
                        Audio.PlayOneShot(keySound);
                        Invoke("ChangeStatus", 1.5f);
                        PM.P_Text.text = "Door unlocked.";
                        PM.Invoke("P_ResetText", 5f);
                        break;
                    }
                }
            }

            if (isUnlock)
            {
                open = !open;
            }
            playSound = true;
        }

        if (playSound && isUnlock)
        {
            if (!Audio.isPlaying)
            {
                Audio.clip = doorSound;
                Audio.pitch = 1.25f;
                Audio.volume = 0.1f;
                Audio.Play();
                playSound = false;
            }
        }
        if (playSound && !isUnlock && !hasKey)
        {
            if (!Audio.isPlaying)
            {
                Audio.clip = lockSound;
                Audio.pitch = 1.25f;
                Audio.volume = 0.1f;
                Audio.Play();
                PM.P_Text.text = "It's locked. I need a key.";
                PM.Invoke("P_ResetText", 5f);
                playSound = false;
            }
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
            }
            else
            {
                Audio.UnPause();
            }
        }
    }

    void ChangeStatus()
    {
        isUnlock = true;
    }
}
