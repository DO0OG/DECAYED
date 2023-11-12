using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class TrackerAI : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource Audio;
    public AudioClip FootStep;
    public AudioClip Idle;
    public AudioClip Attack;

    [Header("AttackCollision")]
    public GameObject attackCollision;

    public Player_Move PM;
    public Transform player;
    public Transform[] wanderPoints;
    public LayerMask obstacleMask;
    public float minWanderTime = 1.0f;
    public float maxWanderTime = 4.0f;
    public float pauseTime = 2.0f;
    public Light flashlight;

    public AudioSource[] soundDetectors;
    public float viewDistance = 15.0f;
    public float soundDetectionDistance = 20.0f;
    public float proximitySoundDetectionDistance = 10.0f;

    public float diff = 1;

    private NavMeshAgent navMeshAgent;
    public bool isChasing = false;
    public bool isWandering = false;
    public bool isMoving = false;
    private float normalMoveSpeed = 3.0f;
    private float chaseMoveSpeed = 6f;
    private Vector3 lastSoundPosition;
    public float speed = 30f;

    private float currentPauseTime;

    private Animator animate;
    public Vector3 directionToPlayer;

    public FrameCounter FC;

    void Start()
    {
        FC = FindObjectOfType<FrameCounter>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animate = GetComponent<Animator>();
        Audio = GetComponent<AudioSource>();
        PM = FindObjectOfType<Player_Move>();
        SetRandomWanderTime();

        diff = PlayerPrefs.GetFloat("DiffLevel");

        viewDistance = viewDistance * diff / 2;
        soundDetectionDistance = soundDetectionDistance * diff / 2;
        proximitySoundDetectionDistance = proximitySoundDetectionDistance * diff / 2;
    }

    private void Update()
    {
        if (PM.isPause)
        {
            Audio.Pause();
        }
        else
        {
            Audio.UnPause();

            if (navMeshAgent.isStopped && !isMoving && navMeshAgent.speed == normalMoveSpeed)
            {
                if (!Audio.isPlaying)
                {
                    Audio.pitch = Random.Range(1f, 1.1f);
                    Audio.volume = 0.15f;
                    Audio.clip = Idle;
                    Audio.Play();
                }
            }

            if (!navMeshAgent.isStopped && isMoving && navMeshAgent.speed == normalMoveSpeed)
            {
                if (!Audio.isPlaying)
                {
                    animate.speed = 0.8f;
                    Audio.pitch = Random.Range(1f, 1.1f);
                    Audio.volume = 0.15f;
                    Audio.clip = FootStep;
                    Audio.Play();
                }
            }
            if (!navMeshAgent.isStopped && isMoving && navMeshAgent.speed == chaseMoveSpeed)
            {
                if (!Audio.isPlaying)
                {
                    animate.speed = 1.7f;
                    Audio.pitch = Random.Range(1.6f, 1.8f);
                    Audio.volume = 0.2f;
                    Audio.clip = FootStep;
                    Audio.Play();
                }
            }
        }
    }

    void FixedUpdate()
    {
        directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (animate.GetCurrentAnimatorStateInfo(0).IsName("root|Anim_monster_scavenger_attack") ||
            animate.GetCurrentAnimatorStateInfo(0).IsName("root|Anim_monster_scavenger_attack1"))
        {
            isMoving = false;
            navMeshAgent.isStopped = true;
        }
        else
        {
            navMeshAgent.isStopped = false;
        }

        if (distanceToPlayer <= 2.5f && distanceToPlayer >= 1.25f)
        {
            //AI가 플레이어와 원하는 거리까지 다가가면 멈추도록
            navMeshAgent.isStopped = true;
            animate.SetBool("isIdle", false);
            animate.SetBool("isChasing", false);
            animate.SetBool("isMoving", false);
            animate.SetBool("isAttack", true);

            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            float rotationSpeed = 5.0f;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animate.SetBool("isAttack", false);
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < 100f && distanceToPlayer < viewDistance)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, viewDistance, obstacleMask))
                {
                    if (hit.transform != player)
                    {
                        isChasing = false;
                    }
                }
                else
                {
                    Vector3 directionToFlashlight = flashlight.transform.position - transform.position;
                    float angleToFlashlight = Vector3.Angle(transform.forward, directionToFlashlight);

                    if (angleToFlashlight < 100f && directionToFlashlight.magnitude < viewDistance)
                    {
                        Debug.Log("VISUAL");
                        isChasing = true;
                        lastSoundPosition = player.position;
                    }
                }

                if (isChasing)
                {
                    isMoving = true;

                    animate.SetBool("isChasing", true);
                    animate.SetBool("isMoving", false);
                    navMeshAgent.speed = chaseMoveSpeed;
            
                    //플레이어에서 원하는 거리까지 다가가도록
                    Vector3 desiredPosition = player.position;
                    navMeshAgent.destination = desiredPosition;
                }
            }
            else
            {
                foreach (var soundDetector in soundDetectors)
                {
                    if (soundDetector != null && soundDetector.isPlaying)
                    {
                        float distanceToSound = Vector3.Distance(transform.position, soundDetector.transform.position);

                        float currentSoundDetectionDistance = soundDetectionDistance;
                        if (soundDetector.volume < 0.35f)
                        {
                            currentSoundDetectionDistance = proximitySoundDetectionDistance;
                        }

                        if (distanceToSound < currentSoundDetectionDistance)
                        {
                            isMoving = true;

                            navMeshAgent.speed = normalMoveSpeed;
                            Debug.Log("SOUND");
                            UpdateLastSoundPosition(soundDetector.transform.position);
                            navMeshAgent.destination =  lastSoundPosition;
                            isChasing = true;
                            break;
                        }
                    }
                    else
                    {
                        isChasing = false;
                    }
                }

                if (!isChasing && !isWandering)
                {
                    if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
                    {
                        isMoving = true;

                        animate.SetBool("isChasing", false);
                        animate.SetBool("isMoving", true);
                        isWandering = true;
                        navMeshAgent.speed = normalMoveSpeed;
                        Transform randomWanderPoint = wanderPoints[Random.Range(0, wanderPoints.Length)];
                        navMeshAgent.destination =  randomWanderPoint.position;
                        currentPauseTime = pauseTime; //대기 시간 설정
                    }
                }
                else if (isWandering)
                {
                    if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
                    {
                        isMoving = false;
                        animate.SetBool("isChasing", false);
                        animate.SetBool("isMoving", false);
                        animate.SetBool("isIdle", true);
                        StartCoroutine(WaitAndResume());
                    }
                }
            }
        }
    }

    public void E_OnAttackCollision()
    {
        attackCollision.SetActive(true);
    }

    public void AttackSound()
    {
        Audio.pitch = Random.Range(1.6f, 1.8f);
        Audio.volume = 0.025f;
        Audio.clip = Attack;
        Audio.Play();
    }

    void UpdateLastSoundPosition(Vector3 soundPosition)
    {
        lastSoundPosition = soundPosition;
    }

    IEnumerator WaitAndResume()
    {
        yield return new WaitForSeconds(currentPauseTime); //대기 시간만큼 기다림

        animate.SetBool("isChasing", false);
        animate.SetBool("isMoving", true);
        animate.SetBool("isIdle", false);
        isWandering = false;
        SetRandomWanderTime();
    }

    void SetRandomWanderTime()
    {
        float waitTime = Random.Range(minWanderTime, maxWanderTime);
        pauseTime = waitTime;
    }
}
