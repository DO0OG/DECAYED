using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [Header("Personalize")]
    public Outline OL;
    public Player_Move PM;

    bool trig, open;//trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f;//скорость вращения
    public float DoorOpenAngle = 90.0f;//угол вращения 
    private Vector3 defaulRot;
    private Vector3 openRot;

    public AudioSource Audio;
    public AudioClip doorSound;

    // Start is called before the first frame update
    void Start()
    {
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

        if (open)//открыть
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        }
        else//закрыть
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
        }
        if (Input.GetMouseButtonDown(0) && OL.enabled && !PM.isPause)
        {
            open = !open;
            playSound = true;
        }
        if (playSound)
        {
            Audio.clip = doorSound;
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
    /*private void OnTriggerEnter(Collider coll)//вход и выход в\из  триггера 
    {
        if (coll.tag == "Player")
        {
            if (!open)
            {
                txt.text = "Close E ";
            }
            else
            {
                txt.text = "Open E";
            }
            trig = true;
        }
    }
    private void OnTriggerExit(Collider coll)//вход и выход в\из  триггера 
    {
        if (coll.tag == "Player")
        {
            txt.text = " ";
            trig = false;
        }
    }*/
}
