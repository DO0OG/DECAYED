using UnityEngine;
using System.Collections;

public class Menu_Cam : MonoBehaviour
{
    public Transform peekLeft;
    public Transform peekRight;
    public Transform peekIdle;
    public Transform orientation;
    public Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    public float sensX;
    public float sensY;

    float xRotation;
    float yRotation;

    Vector2 velocity;
    Vector2 frameVelocity;

    public float peekAngle = 10f; //작은 각도로 변경
    public float peekSpeed = 6f; //느린 속도로 변경
    private float currentPeekAngle = 0f; //현재 기울인 각도

    public float peekDistance = 0.5f; //피킹 시 카메라가 이동할 거리

    //서 있을 때의 Head bobbing
    public float idleBobbingSpeed = 3f;
    public float idleBobbingAmount = 0.0125f;
    public float idleBobbingMidpoint = 0.0f;

    //이동할 때의 Head bobbing
    public float movingBobbingSpeed = 15f;
    public float movingBobbingAmount = 0.035f;
    public float movingBobbingMidpoint = 0.0f;

    private bool isMoving = false;
    private bool isMovingTransitioning = false;
    public bool isJumping = false;
    public bool check = false;
    private Vector3 initialCameraPosition;
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.1f;
    public bool isSprint;
    public bool isMove;
    public bool isPause;
    public bool isCrouch;

    private float timer = 0.0f;

    void Start()
    {
        initialCameraPosition = transform.localPosition;
    }

    void Update()
    {
        MoveHeadBob();
    }

    void MoveHeadBob()
    {
        float isSprinting = 0f;

        float xNoise = Mathf.PerlinNoise(0f, Time.time * movingBobbingSpeed) * 2f - 1f;
        float yNoise = Mathf.PerlinNoise(1f, Time.time * movingBobbingSpeed) * 2f - 1f;

        idleBobbingSpeed = 0.5f;
        idleBobbingAmount = 0.2f;
        movingBobbingSpeed = 0.5f;
        movingBobbingAmount = 0.25f;

        float xBobbingAmount = (xNoise * movingBobbingAmount * isSprinting) + (xNoise * idleBobbingAmount * (1f - isSprinting)) + (xNoise * idleBobbingAmount);
        float yBobbingAmount = (yNoise * movingBobbingAmount * isSprinting) + (yNoise * idleBobbingAmount * (1f - isSprinting)) + (yNoise * idleBobbingAmount);

        Vector3 localPosition = transform.localPosition;
        localPosition.x = movingBobbingMidpoint + xBobbingAmount;
        localPosition.y = movingBobbingMidpoint + yBobbingAmount;

        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, Time.deltaTime * movingBobbingSpeed);

        //상태 변경 시 부드럽게 전환
        if (isMovingTransitioning)
        {
            isMovingTransitioning = false;
            isMoving = !isMoving;
            if (!isMoving)
            {
                IdleHeadBob();
            }
        }
    }

    void IdleHeadBob()
    {
        float waveslice = Mathf.Sin(timer);

        Vector3 localPosition = transform.localPosition;

        idleBobbingSpeed = 2f;
        idleBobbingAmount = 0.03f;

        //움직임 여부에 따라 설정 변경
        float bobbingSpeed = idleBobbingSpeed;
        float bobbingAmount = idleBobbingAmount;
        float midpoint = idleBobbingMidpoint;

        timer += bobbingSpeed * Time.deltaTime;

        if (timer > Mathf.PI * 2)
        {
            timer -= (Mathf.PI * 2);
        }

        float translateChange = waveslice * bobbingAmount;
        localPosition.y = midpoint + translateChange;

        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, Time.deltaTime * bobbingSpeed);

        //상태 변경 시 부드럽게 전환
        if (isMovingTransitioning)
        {
            isMovingTransitioning = false;
            isMoving = !isMoving;
            if (isMoving)
            {
                MoveHeadBob();
            }
        }
    }
}
