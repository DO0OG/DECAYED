using UnityEngine;

public class FlashLight_Controller : MonoBehaviour
{
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    public float peekDistance = 0.5f;
    public float idleBobbingSpeed = 3f;
    public float idleBobbingAmount = 0.0125f;
    public float idleBobbingMidpoint = 0.0f;
    public float movingBobbingSpeed = 15f;
    public float movingBobbingAmount = 0.035f;
    public float movingBobbingMidpoint = 0.0f;
    public bool isJumping = false;
    public bool check = false;
    public float shakeIntensity = 0.05f;
    public float shakeDuration = 0.1f;
    public bool isSprint;
    public bool isMove;
    public bool isPause;
    private Vector3 originalLocalPosition;

    public Player_Health PH;

    private void Start()
    {
        PH = FindObjectOfType<Player_Health>();
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        isPause = GameObject.Find("Char").GetComponent<Player_Move>().isPause;

        if(!isPause && !PH.isDead)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                Shake();
            }
            else
            {
                // 우클릭을 놓으면 원래 상태로 돌아갑니다.
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, Time.deltaTime * movingBobbingSpeed);
            }
        }
    }

    private void FixedUpdate()
    {
    }

    void Shake()
    {
        float xNoise = Mathf.PerlinNoise(0f, Time.time * movingBobbingSpeed) * 2f - 1f;
        float yNoise = Mathf.PerlinNoise(1f, Time.time * movingBobbingSpeed) * 2f - 1f;

        float xBobbingAmount = (xNoise * movingBobbingAmount) + (xNoise * idleBobbingAmount * (1f)) + (xNoise * idleBobbingAmount);
        float yBobbingAmount = (yNoise * movingBobbingAmount) + (yNoise * idleBobbingAmount * (1f)) + (yNoise * idleBobbingAmount);

        Vector3 localPosition = transform.localPosition;
        localPosition.x = movingBobbingMidpoint + xBobbingAmount;
        localPosition.y = movingBobbingMidpoint + yBobbingAmount;

        transform.localPosition = localPosition;
    }
}
