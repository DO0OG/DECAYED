using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Move : MonoBehaviour
{
    [Header("Movement")]
    public float forceGravity = 5f;    //�߷� ����
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    public bool readyToJump;
    public bool isSprint;
    public bool crouched;
    public bool isMoving = false;
    public bool isJump = false;
    public bool isLand = false;
    public bool wasCrouched;
    public bool forLand;

    public float crouchYScale;
    private float startYScale;

    public float stamina;
    float maxStamina;

    public Slider staminaBar;
    public float dValue;
    public CanvasGroup staminaBarCanvas;
    public Canvas InventoryCanvas;
    public Canvas PauseCanvas;
    public Canvas SettingCanvas;

    public TextMeshProUGUI P_Text;
    public TextMeshProUGUI G_Text;
    public TextMeshProUGUI Obj_Text;
    public TextMeshProUGUI Info;

    public bool resetStatus = false;

    public bool isPause = false; //���� �Ͻ� ���� ����
    public bool isMenu = false;
    public bool isInven = false;

    public bool isSlope = false;

    public float diff = 1;

    public GameObject SpotLight;
    public bool hasFlash = false;
    public bool isFlash = false;
    public float maxBatteryLife;
    public float batteryLife;
    public float batteryDepletionRate;
    public float batteryRechargeRate;
    public float flashlightIntensity;
    public bool isFlashing;

    public Slider flashBar;
    public CanvasGroup flashBarCanvas;
    public Image flashBarFill;

    public Slider flashBar_Inv;
    public CanvasGroup flashBarCanvas_Inv;
    public Image flashBarFill_Inv;

    private Color originalFlashBarColor; //���� ���� ����

    public AudioClip flashlightClickSound; //�Ҹ� ����
    public AudioSource clickSource; //AudioSource ������Ʈ
    public AudioClip flashlightShakeSound;
    public AudioSource shakeSource;
    public AudioClip grabSound;
    public AudioSource grabSource;

    public SaveLoadManager SLM;
    public Player_Footstep PF;
    public Player_Health PH;

    public KeyCode rechargeKey = KeyCode.Mouse1; //���콺 ��Ŭ���� ����Ͽ� ���͸� ����

    private InventoryManager IM;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float h_input;
    float v_input;

    Vector3 moveDirection;

    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        maxStamina = stamina;
        staminaBar.maxValue = maxStamina;

        startYScale = transform.localScale.y;
        staminaBarCanvas = staminaBar.GetComponent<CanvasGroup>();
        staminaBarCanvas.alpha = 0f;

        flashBarCanvas = flashBar.GetComponent<CanvasGroup>();
        flashBarCanvas.alpha = 0f;
        originalFlashBarColor = flashBarFill.color;

        flashBarCanvas_Inv = flashBar_Inv.GetComponent<CanvasGroup>();
        flashBarCanvas_Inv.alpha = 0f;
        originalFlashBarColor = flashBarFill_Inv.color;

        InventoryCanvas.gameObject.SetActive(false);
        IM = InventoryManager.Instance;

        PauseCanvas.enabled = false;
        SettingCanvas.enabled = false;

        SpotLight.gameObject.SetActive(false);
        clickSource = GetComponent<AudioSource>();
        clickSource.clip = flashlightClickSound; //�Ҹ� ���� ����
        shakeSource = GetComponent<AudioSource>();
        shakeSource.clip = flashlightShakeSound;
        grabSource = GetComponent<AudioSource>();
        grabSource.clip = grabSound;

        diff = PlayerPrefs.GetFloat("DiffLevel");

        PH = FindObjectOfType<Player_Health>();
        SLM = SaveLoadManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        bool hasFlashlight = IM.Items.Exists(item => item.itemName == "������");

        //�� üũ
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.64f + 0.2f, whatIsGround);

        if (!isPause && !PH.isDead)
        {
            K_Input();
            SpeedControl();
            FlashFunc();
        }

        isSlope = OnSlope();

        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;

        if(isSprint)    DecreaseStamina();
        if (!isSprint && stamina != maxStamina) IncreaseStamina();

        staminaBar.value = stamina;

        //staminaBarCanvas�� ���� ����
        if (stamina >= maxStamina)
        {
            //stamina�� �� á�� �� ���̵�ƿ�
            staminaBarCanvas.alpha = Mathf.Lerp(staminaBarCanvas.alpha, 0f, Time.deltaTime * 2f);
        }
        else
        {
            //stamina�� �پ�� �� ���̵���
            staminaBarCanvas.alpha = Mathf.Lerp(staminaBarCanvas.alpha, 1f, Time.deltaTime * 2f);
        }

        flashBar.value = batteryLife;
        flashBar_Inv.value = flashBar.value;

        if (isPause)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (InventoryCanvas != null && !isMenu)
                {
                    G_Text.enabled = true;
                    P_Text.enabled = true;
                    Info.text = "";
                    InventoryCanvas.gameObject.SetActive(false);
                    flashBarCanvas_Inv.alpha = 0f;
                    isPause = false;
                    isInven = false;
                    isMenu = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1; //���� �簳

                    if (isFlash && hasFlashlight)
                    {
                        flashBarCanvas.alpha = 1;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PauseCanvas != null && SettingCanvas != null && !isInven)
                {
                    G_Text.enabled = true;
                    P_Text.enabled = true;
                    PauseCanvas.enabled = false;
                    SettingCanvas.enabled = false;
                    isPause = false;
                    isInven = false;
                    isMenu = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1; //���� �簳

                    if (isFlash)
                    {
                        flashBarCanvas.alpha = 1;
                    }
                }
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //Tab Ű�� ������ �� InventoryCanvas�� ���¸� ����
            if (InventoryCanvas != null && !isMenu && !PH.isDead)
            {
                G_Text.enabled = false;
                P_Text.enabled = false;
                staminaBarCanvas.alpha = 0;
                flashBarCanvas.alpha = 0;
                InventoryCanvas.gameObject.SetActive(true);
                Info.text = "";
                isPause = InventoryCanvas.enabled; //�κ��丮�� ������ �� �Ͻ� ���� ���·� ����
                Time.timeScale = isPause ? 0 : 1; //�ð� �帧 �����Ͽ� ���� �Ͻ� ����
                isInven = true;

                if (hasFlash)
                {
                    float batteryPercentage = batteryLife / maxBatteryLife;
                    flashBarCanvas_Inv.alpha = 1f;
                    flashBarFill_Inv.color = Color.Lerp(originalFlashBarColor, Color.red, 1 - batteryPercentage);
                }
                else
                {
                    flashBarCanvas_Inv.alpha = 0f;
                }

                if (InventoryCanvas.enabled)
                {
                    IM.ListItems();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Tab Ű�� ������ �� InventoryCanvas�� ���¸� ����
            if (PauseCanvas != null && SettingCanvas != null && !isInven && !PH.isDead)
            {
                G_Text.enabled = false;
                P_Text.enabled = false;
                staminaBarCanvas.alpha = 0;
                flashBarCanvas.alpha = 0;
                PauseCanvas.enabled = !PauseCanvas.enabled;
                isPause = PauseCanvas.enabled; //�κ��丮�� ������ �� �Ͻ� ���� ���·� ����
                Time.timeScale = isPause ? 0 : 1; //�ð� �帧 �����Ͽ� ���� �Ͻ� ����
                isPause = true;
                isMenu = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryPickupItem();
        }
    }

    private void FixedUpdate()
    {
        if (!isPause && !PH.isDead)
        {
            MovePlayer();
        }
    }

    private void UpdateFlashlightIntensity()
    {
        float normalizedBatteryLife = batteryLife / maxBatteryLife;
        SpotLight.GetComponent<Light>().intensity = flashlightIntensity * normalizedBatteryLife;
    }

    private void TryPickupItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2.5f))
        {
            ItemPickup itemPickup = hit.collider.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.Pickup();
                grabSource.volume = 0.03f;
                grabSource.PlayOneShot(grabSound);
                if (itemPickup.Item.itemName == "������")
                {
                    hasFlash = true;
                    isFlash = true;
                    SpotLight.gameObject.SetActive(true);
                    flashBarCanvas.alpha = 1f;
                    CancelInvoke("G_ResetText");
                    CancelInvoke("P_ResetText");
                    G_Text.text = "F�� ���� �������� Ű�ų� �� �� �ֽ��ϴ�.\n��Ŭ���� ������ �������� ������ �� �ֽ��ϴ�.";
                    P_Text.text = "���� ������ ���� ������.";
                    Obj_Text.text = "";
                    Invoke("G_ResetText", 8f);
                    Invoke("P_ResetText", 8f);

                    batteryLife = maxBatteryLife;
                    flashBar.maxValue = maxBatteryLife;
                    flashBar_Inv.maxValue = maxBatteryLife;
                    UpdateFlashlightIntensity();
                }
            }
        }
        else if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 4f))
        {
            ItemPickup itemPickup = hit.collider.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                P_Text.text = "";
                Invoke("P_ResetText", 5f);
            }
        }
    }

    public void G_ResetText()
    {
        G_Text.text = "";
    }

    public void P_ResetText()
    {
        P_Text.text = "";
    }

    public void FlashFunc()
    {
        bool hasFlashlight = IM.Items.Exists(item => item.itemName == "������");

        if (hasFlashlight)
        {
            hasFlash = true;

            if (!isPause)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (hasFlash)
                    {
                        if (!isFlash)
                        {
                            //�Ҹ� ���
                            if (clickSource != null && flashlightClickSound != null)
                            {
                                clickSource.volume = 0.35f;
                                clickSource.PlayOneShot(flashlightClickSound);
                            }

                            flashBarCanvas.alpha = 1f;
                            SpotLight.gameObject.SetActive(true);
                            isFlash = true;
                        }
                        else
                        {
                            //�Ҹ� ���
                            if (clickSource != null && flashlightClickSound != null)
                            {
                                clickSource.volume = 0.35f;
                                clickSource.PlayOneShot(flashlightClickSound);
                            }

                            flashBarCanvas.alpha = 0f;
                            SpotLight.gameObject.SetActive(false);
                            isFlash = false;
                        }
                    }
                }

                //���͸� �Ҹ�
                if (isFlash && batteryLife > 0)
                {
                    batteryLife -= batteryDepletionRate * (diff/1.5f) * Time.deltaTime;
                    UpdateFlashlightIntensity();

                    //�����̴� ���� ������ ���������� ����
                    float batteryPercentage = batteryLife / maxBatteryLife;
                    flashBarFill.color = Color.Lerp(originalFlashBarColor, Color.red, 1 - batteryPercentage);
                }

                //���͸� ���� (��Ŭ�� ��)
                if (Input.GetKey(rechargeKey) && batteryLife < maxBatteryLife && hasFlash)
                {
                    // �Ҹ� ���
                    if (shakeSource != null && flashlightShakeSound != null && !shakeSource.isPlaying)
                    {
                        shakeSource.volume = 0.35f;
                        shakeSource.PlayOneShot(flashlightShakeSound);
                    }
                    batteryLife += batteryRechargeRate * Time.deltaTime * 1.25f;
                    UpdateFlashlightIntensity();

                    //�����̴� ���� ������ ���� �������� ����
                    float batteryPercentage = batteryLife / maxBatteryLife;
                    flashBarFill.color = Color.Lerp(originalFlashBarColor, Color.red, 1 - batteryPercentage);
                }
            }
        }
        else
        {
            hasFlash = false;
            flashBarCanvas.alpha = 0f;
            SpotLight.gameObject.SetActive(false);
        }
    }

    public bool IsPaused()
    {
        return isPause;
    }

    private void K_Input()
    {
        h_input = Input.GetAxisRaw("Horizontal");
        v_input = Input.GetAxisRaw("Vertical");

        if ((h_input == -1f || h_input == 1f) || (v_input == -1f || v_input == 1f)) isMoving = true;
        else isMoving = false;

        if(Input.GetKey(jumpKey) && readyToJump && grounded && !crouched)
        {
            readyToJump = false;

            forLand = true;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (grounded) Crouch();
        if (!crouched)  Sprint();
    }

    private void MovePlayer()
    {
        //�̵� ���� ���
        moveDirection = orientation.forward * v_input + orientation.right * h_input;

        //���
        if (OnSlope() && !exitingSlope)
        {
            if (crouched)
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 13f, ForceMode.Force);
            }
            else
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 18f, ForceMode.Force);
            }

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 2f, ForceMode.Force);
        }

        //��
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //����
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }    
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private void Crouch()
    {
        wasCrouched = crouched;

        if (Input.GetKeyDown(crouchKey) && !crouched)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
            moveSpeed = 1.85f;
            forceGravity = 9f;
            isSprint = false;
            crouched = true;
            forLand = false;
        }

        if (!Input.GetKey(crouchKey))
        {
            if (!IsHeadBlocked())
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                moveSpeed = 2.25f;
                forceGravity = 9f;
                crouched = false;
            }
            if (OnSlope() && wasCrouched)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + crouchYScale, transform.position.z);
            }
        }

        //crouched ���°� �ٲ���� ��, ���� �Ӹ� ���� ������ �ٽ� crouched��
        if (wasCrouched != crouched && IsHeadBlocked())
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 8f, ForceMode.Impulse);
            moveSpeed = 1.85f;
            forceGravity = 0f;
            isSprint = false;
            crouched = true;
        }
    }

    private bool IsHeadBlocked()
    {
        //ĳ������ �Ӹ� ���� �� üũ
        RaycastHit headHit;
        if (Physics.Raycast(transform.position, Vector3.up, out headHit, playerHeight * 0.5f))
        {
            return true;
        }
        return false;
    }

    private void Sprint()
    {
        if (stamina <= 0)
        {
            moveSpeed = 2.25f;
            isSprint = false;
            return;
        }
        if (Input.GetKey(sprintKey) && ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))) & !Input.GetKey(KeyCode.S))
        {
            moveSpeed = 4f;
            isSprint = true;
        }
        else
        {
            moveSpeed = 2.25f;
            isSprint = false;
        }
    }

    private void DecreaseStamina()
    {
        if (stamina > 0)
        {
            stamina -= dValue * (diff/2) * Time.deltaTime;
        }
    }

    private void IncreaseStamina()
    {
        if (stamina < maxStamina)
        {
            stamina += dValue * Time.deltaTime / 2;
        }
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("whatIsGround"))
        {
            isLand = true;
            isJump = false;
            if (!crouched) StartCoroutine(ChangeGravity(20f, 9f, 0.5f));
            Debug.Log("TRUE");

            if (!wasCrouched && isLand && forLand)
            {
                PF.LandStep();
                forLand = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("whatIsGround"))
        {
            isLand = false;
            isJump = true;
            Debug.Log("FALSE");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("savePoint"))
        {
            if (hasFlash)
            {
                SLM.Save();
                Destroy(other.gameObject);
            }
            else
            {
                CancelInvoke("G_ResetText");
                CancelInvoke("P_ResetText");
                P_Text.text = "�ʹ� ��Ӵ�. Ʈ������ �������� ì�ܿ���.";
                Invoke("G_ResetText", 8f);
                Invoke("P_ResetText", 8f);
            }
        }
    }

    public IEnumerator ChangeGravity(float startGravity, float targetGravity, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            forceGravity = Mathf.Lerp(startGravity, targetGravity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        forceGravity = targetGravity;
    }
}
