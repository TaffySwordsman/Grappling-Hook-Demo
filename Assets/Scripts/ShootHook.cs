using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootHook : MonoBehaviour
{

	private LineRenderer line;
	private Vector3 grapplePoint, firstPoint;
	private SpringJoint joint;
	private bool firstAttach = false;
	private bool cableAttached = false;
	private bool retract = false;
	static float t = 0.0f;

	private GameObject obj1, obj2;
	private RaycastHit hit1, hit2;

	[SerializeField] LayerMask validSurfaces = 0;
	[SerializeField] Transform firePoint = null;
	[SerializeField] Transform cam = null;
	[SerializeField] Transform player = null;
	[SerializeField] float maxGrapple = 20f;
	[Range(0.0f, 1.0f)]
	[SerializeField] float cableMax = 1f;

	void Awake()
    {
		line = GetComponent<LineRenderer>();
		line.positionCount = 0;
	}

    void Update()
    {
		
		if (Input.GetButtonDown("Fire2"))
			ShootStart();
		if (Input.GetButtonUp("Fire2"))
			ShootStop();

		if (cableAttached && Input.GetKey(KeyCode.LeftControl))
		{
			retract = true;
			obj1.GetComponent<Rigidbody>().WakeUp();
			obj2.GetComponent<Rigidbody>().WakeUp();
		}
		else { retract = false; t = 0; }

	}

	void FixedUpdate()
	{
		if (retract)
		{
			t += 0.5f * Time.deltaTime;
			float distanceFromPoint = Vector3.Distance(obj1.transform.position, obj2.transform.position);
			joint.maxDistance = Mathf.Lerp(distanceFromPoint, 0f, t);
			obj1.transform.position = obj1.transform.position;
		}
	}

	void LateUpdate()
	{
		DrawCable();
	}

	void ShootStart()
	{
		DestroyCable();

		RaycastHit hit;
		if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapple, validSurfaces))
		{
			if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Grapple Object"))
			{
				grapplePoint = hit.transform.InverseTransformPoint(hit.point);
				line.positionCount = 2;

				firstAttach = true;
				obj1 = hit.transform.gameObject;
				hit1 = hit;
			}
			else { firstAttach = false;  }

		}
	}

	void ShootStop()
	{
		RaycastHit hit;
		if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapple, validSurfaces))
		{
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Grapple Object"))
			{
				firstPoint = grapplePoint;
				grapplePoint = hit.transform.InverseTransformPoint(hit.point);
				hit2 = hit;

				obj2 = hit.transform.gameObject;

				if (obj1 == obj2)
				{
					DestroyCable();
					return;
				}

				joint = obj1.AddComponent<SpringJoint>();
				joint.anchor = firstPoint;
				joint.autoConfigureConnectedAnchor = false;
				joint.connectedBody = obj2.GetComponent<Rigidbody>();
				joint.connectedAnchor = grapplePoint;
				joint.enableCollision = true;

				float distanceFromPoint = Vector3.Distance(obj1.transform.position, obj2.transform.position);

				joint.maxDistance = distanceFromPoint * cableMax;
				joint.minDistance = 0f;

				joint.spring = 1000f;
				joint.damper = 0f;
				joint.massScale = 1f;

				firstAttach = false;
				cableAttached = true;
				line.positionCount = 2;
			}
			else { DestroyCable(); }
		}
	}

	void DrawCable()
	{
		if (!cableAttached && !firstAttach) return;

		if(firstAttach)
			line.SetPosition(0, firePoint.position);
		else { line.SetPosition(0, obj2.transform.position); }

		line.SetPosition(1, obj1.transform.position);
	}

	void DestroyCable()
	{
		line.positionCount = 0;
		Destroy(joint);
		firstAttach = false;
		cableAttached = false;
	}
}
