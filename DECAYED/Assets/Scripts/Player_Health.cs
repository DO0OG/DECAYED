using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_Health : MonoBehaviour
{
    [Header("Health")]
    public float currentHealth;
    public float maxHP;

    [Header("Blood")]
    public CanvasGroup CG;

    [Header("Player")]
    public Player_Move PM;
    public Player_Cam PC;
    public Camera_Controller CC;

    [Header("JumpScare")]
    public AudioClip Scream;
    public AudioSource Sounds; //AudioSource 컴포넌트
    public Player_Footstep PF; 
    public GameObject JC;
    public TrackerAI TAI;
    public SaveLoadManager SLM;
    public GameManager GM;
    public Canvas END;
    private string saveFilePath;

    public float diff = 1;
    public bool isHit = false;
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        diff = PlayerPrefs.GetFloat("DiffLevel");
        PM = FindObjectOfType<Player_Move>();
        PC = FindObjectOfType<Player_Cam>();
        PF = FindObjectOfType<Player_Footstep>();
        TAI = FindObjectOfType<TrackerAI>();
        SLM = SaveLoadManager.GetInstance();
        GM = FindObjectOfType<GameManager>();
        CC = FindObjectOfType<Camera_Controller>();
        SetMaxHealth();

        Sounds = GetComponent<AudioSource>();

        isDead = false;

        CG.alpha = 0;
    }

    private void SetMaxHealth()
    {
        // 데모버전은 그냥 무조건 한 번에 죽도록
        if (diff == 1)
        {
            maxHP = 50;
            currentHealth = maxHP;
        }
        else if (diff == 1.5)
        {
            maxHP = 50;
            currentHealth = maxHP;
        }
        else if (diff == 2)
        {
            maxHP = 50;
            currentHealth = maxHP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHit && !isDead)
        {
            if(currentHealth <= maxHP)
            {
                RestoreHealth();
            }
        }
        DeadJump();
    }

    public void RestoreHealth()
    {
        currentHealth += 5 * Time.deltaTime / diff;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHP);

        if (currentHealth < maxHP)
        {
            CG.alpha -= 0.01f * Time.deltaTime;
            CG.alpha = Mathf.Clamp(CG.alpha, 0f, 0.25f);
        }
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHP); //체력이 음수가 되지 않도록

        StartCoroutine(PM.ChangeGravity(50f, 9f, 0.5f)); //피격 시 속도 감소(중력 증가)
        StartCoroutine(PC.HitShakeCamera(0.1f, 0.2f));

        CG.alpha += 0.125f;
        CG.alpha = Mathf.Clamp(CG.alpha,0f, 0.25f);
    }

    public void DeadJump()
    {
        if(currentHealth <= 0 && isHit)
        {
            SLM.SaveDead();
            if (!Sounds.isPlaying)
            {
                Sounds.volume = 1f;
                Sounds.PlayOneShot(Scream);
            }
            isDead = true;
            PF.AudioSource.enabled = false;
            PM.rb.velocity = Vector3.zero;
            PC.transform.localPosition = Vector3.zero;
            CC.transform.rotation = new Quaternion(0, 0, 0, 0);
            TAI.gameObject.SetActive(false);
            JC.SetActive(true);
            PC.MoveHeadBob();

            StartCoroutine(WaitFirst());
        }
    }

    IEnumerator WaitFirst()
    {
        yield return new WaitForSeconds(2.85f); //대기 시간만큼 기다림

        if(END != null)
        {
            END.gameObject.SetActive(true);
            StartCoroutine(WaitAndResume());
        }
    }

    IEnumerator WaitAndResume()
    {
        yield return new WaitForSeconds(5f); //대기 시간만큼 기다림

        isDead = false;

        GM.DeactivateAllObjects();

        PlayerPrefs.SetInt("isSave", 1);

        Application.Quit();

        // 비동기로 씬 로드
        //LoadingManager.Instance.LoadScene("MainMenu");
    }
}
