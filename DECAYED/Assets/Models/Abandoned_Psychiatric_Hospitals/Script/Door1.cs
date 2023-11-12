using UnityEngine;
using UnityEngine.UI;

public class Door1 : MonoBehaviour
{
    [Header("Personalize")]
    public Outline OL;
    public Player_Move PM;

    public bool trig, open; // trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f; // скорость вращения
    public float DoorOpenAngle = 90f; // угол вращения
    public Vector3 defaulRot;
    public Vector3 openRot;
    public Text txt; // text

    public AudioSource Audio;
    public AudioClip doorSound;

    private Quaternion defaultRotation;
    private Quaternion openRotation;

    void Start()
    {
        PM = FindObjectOfType<Player_Move>();
        Audio = GetComponent<AudioSource>();
        defaultRotation = transform.rotation;
        openRotation = Quaternion.Euler(defaulRot.x - DoorOpenAngle, defaulRot.y, defaulRot.z);
        OL = this.GetComponent<Outline>();
        if (OL == null)
        {
            OL = this.gameObject.AddComponent<Outline>();
            OL.enabled = false;
        }
    }

    void Update()
    {
        bool playSound = false;

        if (open)
        {
            Quaternion targetRotation = Quaternion.Euler(defaulRot.x - DoorOpenAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smooth);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, Time.deltaTime * smooth);
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
}