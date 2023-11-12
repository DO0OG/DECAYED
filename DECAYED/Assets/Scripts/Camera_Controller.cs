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
    public float maxRaycastDistance = 10f; //레이캐스트 최대 거리

    private Vector3 originalCrosshairScale; //원래 크로스헤어 크기 저장
    private Vector3 targetCrosshairScale;   //목표 크로스헤어 크기
    private float crosshairScaleChangeSpeed = 0.05f; //크로스헤어 크기 변경 속도

    void Start()
    {
        vectOffset = transform.position - goFollow.transform.position;
        originalCrosshairScale = crosshair.transform.localScale; //원래 크로스헤어 크기 저장
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
                //크로스헤어 크기를 키우도록 목표 크기를 설정
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
                //크로스헤어 크기를 원래대로 돌아가도록 목표 크기를 설정
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
            //크로스헤어 크기를 원래대로 돌아가도록 목표 크기를 설정
            targetCrosshairScale = originalCrosshairScale;
            func.SetActive(false);
        }

        //크로스헤어 크기를 서서히 변경
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
