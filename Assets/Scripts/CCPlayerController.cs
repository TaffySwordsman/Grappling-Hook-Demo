using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCPlayerController : MonoBehaviour
{

	private CharacterController controller;
	public Vector3 velocity;
	private const float gravity = -9.81f;

	Transform cam;
	Animator animator;

	public float turnSmooth = 0.2f;
	float turnSmoothVel;
	public float speedSmooth = 0.1f;
	float speedSmoothVel;

	float curSpeed;
	public float velY;

	public float walkSpeed = 3f;
	public float runSpeed = 6f;
	public float jumpHeight = 2f;

	[Range(0, 1)]
	public float airControl = 1f;

	bool isGrounded;
	bool jumping;
	bool falling;
	bool inAir;

	Ray rGround;
	RaycastHit hit;

	public Transform groundChecker;
	public float GroundDistance = 0.1f;
	public LayerMask Ground;


	void Start()
	{
		//Initialize appropriate components
		animator = GetComponent<Animator>();
		cam = Camera.main.transform;
		controller = GetComponent<CharacterController>();
		isGrounded = true;
	}

	void Update()
	{

		//Player input
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Vector2 inputDir = input.normalized;
		bool running = Input.GetKey(KeyCode.LeftShift);

		//Move player
		Move(inputDir, running);

		//Jump check
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}

		//Set animation speed
		float animSpeed = ((running) ? curSpeed / runSpeed : curSpeed / walkSpeed * .5f);
		animator.SetFloat("Speed", animSpeed, speedSmooth, Time.deltaTime);

		//Set jumping
		animator.SetBool("Jump", jumping);

		//Set falling
		inAir = (!jumping && falling) ? true : false;
		animator.SetBool("inAir", inAir);

		//Set animation direction
		//float animDir = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg / 90 % 2f;
		//animator.SetFloat("Direction", animDir, speedSmooth, Time.deltaTime);

	}

	void Move(Vector2 inputDir, bool running)
	{
		//Prevent rotations when the player is not moving in any direction
		if (inputDir != Vector2.zero)
		{
			//Rotate player
			float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVel, GetSmoothedTime(turnSmooth));
		}

		//Calculate speed
		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		curSpeed = Mathf.SmoothDamp(curSpeed, targetSpeed, ref speedSmoothVel, GetSmoothedTime(speedSmooth));

		//Calculate velocity with gravity
		velY += gravity * Time.deltaTime;
		velocity = transform.forward * curSpeed + Vector3.up * velY;
		controller.Move(velocity * Time.deltaTime);
		curSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

		//Ground check
		if (controller.isGrounded && velY < 0)
		{
			velY = 0f;
		}

		//Ground check
		isGrounded = Physics.CheckSphere(groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

		//Height of jump check
		if (jumping && velY < 0)
		{
			jumping = false;
			falling = true;
		}

		//Falling check
			if (!isGrounded && velY < -5f)
		{
			falling = true;
		}

		//Distance to ground
		if (!isGrounded)
		{
			rGround = new Ray(groundChecker.position + new Vector3(0, .1f), Vector3.down);
			float gravDist = (-velY / 5);

			if (Physics.Raycast(rGround, out hit, maxDistance: gravDist))
			{
				Debug.DrawLine(rGround.origin, hit.point, Color.red);
				jumping = false;
				falling = false;
			}
		}

		//Grounded after fall check
		if (isGrounded && velY == 0)
		{
			jumping = false;
			falling = false;
		}

	}


	//Jump <jumpHeight> units up
	void Jump()
	{
		
		if (isGrounded)
		{
			float jumpVel = Mathf.Sqrt(-2 * gravity * jumpHeight);
			velY = jumpVel;
			jumping = true;
		}
	}

	//Air control modification
	float GetSmoothedTime(float smoothTime)
	{
		if (controller.isGrounded)
			return smoothTime;

		if (airControl == 0)
		{
			return float.MaxValue;
		}

		return smoothTime / airControl;
	}
}
