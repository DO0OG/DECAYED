using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetFlashlight : MonoBehaviour
{
	[SerializeField] Vector3 vectOffset;
	[SerializeField] GameObject goFollow;
	[SerializeField] float speed = 1.0f;

	void Start()
	{
		vectOffset = transform.position - goFollow.transform.position;
	}

	void Update()
	{
		transform.position = goFollow.transform.position + vectOffset;
		transform.rotation = Quaternion.Slerp(transform.rotation, goFollow.transform.rotation, speed * Time.deltaTime);
	}
}