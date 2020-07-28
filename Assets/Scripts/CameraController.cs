using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	public bool invertX = false;
	public bool invertY = false;
	private int invX = 1;
	private int invY = 1;

	public float mouseSensitivity = 10f;
	public Transform target;
	public float dstFromTarget = 2f;
	public float maxDstFollow = 4f;
	public Vector2 pitchMinMax = new Vector2(-40, 80);

	public float rotSmooth = .12f;
	Vector3 rotSmoothVel;
	Vector3 curRot;
	public float transSmooth = .1f;
	Vector3 transSmoothVel;
	Vector3 camPos;

	float yaw;
	float pitch;

    void LateUpdate()
    {
		//Check for inverted controls
		if (invertX) { invX = -1; } else { invX = 1; }
		if (invertY) { invY = -1; } else { invY = 1; }

		//Get yaw and pitch and limit them
		yaw += Input.GetAxis("Mouse X") * mouseSensitivity * invX;
		pitch += Input.GetAxis("Mouse Y") * mouseSensitivity * invY;
		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

		//Rotate camera
		curRot = Vector3.SmoothDamp(curRot, new Vector3(pitch, yaw), ref rotSmoothVel, rotSmooth);
		transform.eulerAngles = curRot;

		//Orbit camera around player and follow
		camPos = target.position - transform.forward * dstFromTarget;
		transform.position = camPos;
		//transform.position = Vector3.SmoothDamp(transform.position, camPos, ref transSmoothVel, transSmooth);
	}
}
