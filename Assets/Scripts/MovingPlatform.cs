using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

	public GameObject Player;

	//On collision
	private void OnCollisionEnter(Collision collision)
	{
		//Collision with moving objects parents the player to that object
		if (collision.gameObject == Player)
			Player.transform.parent = transform;
	}

	//After collision
	private void OnCollisionExit(Collision collision)
	{
		//Separating from moving object unparents it
		if (collision.gameObject == Player)
			Player.transform.parent = null;
	}
}
