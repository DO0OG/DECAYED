using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger_Controller2 : MonoBehaviour
{
    public Player_Move PM;
    public GameObject eventFirst;

    public AudioClip eventSound;

    public string eventString;

    public Vector3 startPos;
    public Vector3 endPos;

    public bool isTrig = false;

    public float moveSpeed = 0.25f; // �̵� �ӵ� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        PM = FindObjectOfType<Player_Move>();
        startPos = eventFirst.transform.position;
        endPos = new Vector3(278.212f, 1, 330.827f);
        //Invoke(nameof(StartDisable), 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrig)
        {
            MoveEventObject();
        }
    }

    void MoveEventObject()
    {
        // ���� ��ġ���� ��ǥ ��ġ�� ������ �̵�
        eventFirst.transform.position = Vector3.Lerp(eventFirst.transform.position, endPos, moveSpeed * Time.deltaTime);

        // ���� �Ÿ� ���Ϸ� ��������� �̵� �Ϸ�� ����
        if (Vector3.Distance(eventFirst.transform.position, endPos) < 0.1f)
        {
            isTrig = false;
        }
    }

    void StartDisable()
    {
        eventFirst.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        eventFirst.SetActive(true);
        isTrig = true;
    }
}
