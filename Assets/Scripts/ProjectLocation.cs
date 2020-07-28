using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectLocation : MonoBehaviour
{
	public GameObject player;
	CharacterController controller;
	CCPlayerController script;
	float velY;

    // Start is called before the first frame update
    void Start()
    {
		controller = player.GetComponent<CharacterController>();
		script = player.GetComponent<CCPlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!controller.isGrounded)
		{
			Vector3 predictedPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z) + (controller.velocity / 3) * Time.deltaTime;
			transform.position = predictedPoint;
		}
		else { transform.position = player.transform.position; }
	}
}
