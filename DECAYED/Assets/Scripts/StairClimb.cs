using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairClimb : MonoBehaviour
{
    Rigidbody rigidBody;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;

    public bool isStair = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    private void FixedUpdate()
    {
        if (rigidBody != null)
        {
            stepClimb();
        }
    }

    void stepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            if (!hitLower.collider.CompareTag("Slope") && !hitLower.collider.CompareTag("PP"))
            {
                RaycastHit hitUpper;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f) || !hitUpper.collider.CompareTag("Slope"))
                {
                    rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                    isStair = true;
                }
            }
        }

        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f))
        {
            if (hitLower.collider != null && !hitLower.collider.CompareTag("Slope") && !hitLower.collider.CompareTag("PP"))
            {
                RaycastHit hitUpper45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f) || !hitUpper45.collider.CompareTag("Slope"))
                {
                    rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                    isStair = true;
                }
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
        {
            if (hitLower.collider != null && !hitLower.collider.CompareTag("Slope") && !hitLower.collider.CompareTag("PP"))
            {
                RaycastHit hitUpperMinus45;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f) || !hitUpperMinus45.collider.CompareTag("Slope"))
                {
                    rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                    isStair = true;
                }
            }
        }
        else
        {
            isStair = false;
        }
    }
}
