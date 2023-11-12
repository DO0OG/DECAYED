using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Vector3 vectOffset;
    public GameObject goFollow;
    public float speed = 1.0f;
    public GameObject obj;
    public GameObject func;
    public GameObject crosshair;
    public float maxRaycastDistance = 10f; //����ĳ��Ʈ �ִ� �Ÿ�

    private Vector3 originalCrosshairScale; //���� ũ�ν���� ũ�� ����
    private Vector3 targetCrosshairScale;   //��ǥ ũ�ν���� ũ��
    private float crosshairScaleChangeSpeed = 0.05f; //ũ�ν���� ũ�� ���� �ӵ�

    void Start()
    {
        vectOffset = transform.position - goFollow.transform.position;
        originalCrosshairScale = crosshair.transform.localScale; //���� ũ�ν���� ũ�� ����
        targetCrosshairScale = originalCrosshairScale;
    }

    void Update()
    {
        transform.position = goFollow.transform.position + vectOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, goFollow.transform.rotation, speed * Time.deltaTime);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxRaycastDistance))
        {
            if (hit.transform.gameObject.CompareTag("Selectable"))
            {
                obj = hit.transform.gameObject;
                Outline outline = obj.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = true;
                }
                //ũ�ν���� ũ�⸦ Ű�쵵�� ��ǥ ũ�⸦ ����
                targetCrosshairScale = new Vector3(0.25f, 0.25f, crosshair.transform.localScale.z);
                func.SetActive(true);
            }
            else
            {
                if (obj != null)
                {
                    Outline outline = obj.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                    obj = null;
                }
                //ũ�ν���� ũ�⸦ ������� ���ư����� ��ǥ ũ�⸦ ����
                targetCrosshairScale = originalCrosshairScale;
                func.SetActive(false);
            }
        }
        else
        {
            if (obj != null)
            {
                Outline outline = obj.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
                obj = null;
            }
            //ũ�ν���� ũ�⸦ ������� ���ư����� ��ǥ ũ�⸦ ����
            targetCrosshairScale = originalCrosshairScale;
            func.SetActive(false);
        }

        //ũ�ν���� ũ�⸦ ������ ����
        crosshair.transform.localScale = Vector3.Lerp(crosshair.transform.localScale, targetCrosshairScale, crosshairScaleChangeSpeed);

        if (GameObject.Find("Char").GetComponent<Player_Move>().crouched)
        {
            vectOffset = new Vector3(0f, 0.2f, 0f);
        }
        else
        {
            vectOffset = new Vector3(0f, 0.5f, 0f);
        }
    }
}
