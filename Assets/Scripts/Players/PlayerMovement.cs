using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Is the player grounded?
	public int groundState = 0;

	// Player Physics
	public float Acceleration = 0.046875f;
	public float Friction = 0.046875f;
	public float Deceleration = 0.5f;
	public float TopSpeed = 6f;
	public float Gravity = 0.21875f;
	private float JumpForce = 6.5f;

	// RayCasting Objects
	public Transform DownStart, RightDown, LeftDown;

	// Private
	public float CurrentSpeed = 0f;
	private int PlayerDirection = 0;
	private float FallSpeed = 0f;
	private bool Jumping = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update at specific rate independant of framerate currently 50hz - See http://docs.unity3d.com/Manual/ExecutionOrder.html
	void FixedUpdate () {
		groundState = CheckGround();
		Movement();
		Raycasting();
		UpdateAnimations();
	}

	void OnGUI()
	{
		GUILayout.Label( "GroundState = " + groundState + "\n0=Not Grounded, 1=Grounded, 2=Front Not Grounded, Back Is Grounded (Sonic Balancing Animation), 3=Sonic Balancing Other Direction");
		GUILayout.Label ("Location: " + transform.position.ToString());
		GUILayout.Label ("Current Speed: " + CurrentSpeed.ToString());
		GUILayout.Label ("Fall Speed: " + FallSpeed.ToString());
		GUILayout.Label ("Direction: " + PlayerDirection.ToString ());
	}    

	void Movement(){

		if (groundState == 0) {

			FallSpeed = FallSpeed + Gravity;

			if (FallSpeed > 16f)
			{
				FallSpeed = 16f;
			}

			// Move Sprite with Z Axis Locked to 0
			transform.Translate(-Vector2.up * FallSpeed, 0);
		}
		 // Directional Left and Right Movement
		DMovement();

		if (Input.GetKey (KeyCode.Space)) {
			Jumping = true;
		}

		if (Jumping == true) {
			transform.Translate(Vector2.up * JumpForce, 0);
		}
	}

	void DMovement()
	{
		if (Input.GetKey (KeyCode.LeftArrow)) {

			if (CurrentSpeed > 0) {
				CurrentSpeed = CurrentSpeed - Deceleration;
			} else {
				if (CurrentSpeed > -TopSpeed)
				{
					CurrentSpeed = CurrentSpeed - Acceleration;
				}else{
				CurrentSpeed = -TopSpeed;
				}
			}

			// Turn Sprite Left
			transform.eulerAngles = new Vector2 (0, 180);

		} else if (Input.GetKey (KeyCode.RightArrow)) {

			if (CurrentSpeed < 0) {
				CurrentSpeed = CurrentSpeed + Deceleration;
			} else {
				if (CurrentSpeed < TopSpeed)
				{
					CurrentSpeed = CurrentSpeed + Acceleration;
				}else{
					CurrentSpeed = TopSpeed;
				}
			}

			// Turn Sprite Right
			transform.eulerAngles = new Vector2 (0, 0);
	
		} else {
			CurrentSpeed = CurrentSpeed - (Mathf.Min(Mathf.Abs(CurrentSpeed), Friction)*Mathf.Sign(CurrentSpeed));
		}

		transform.Translate (Vector2.right * CurrentSpeed, 0);


	}

	void Raycasting() {
		Debug.DrawLine (DownStart.position, LeftDown.position, Color.green, 0.2f, false);
		Debug.DrawLine (DownStart.position, RightDown.position, Color.green, 0.2f, false);

	}

	void UpdateAnimations()
	{
		Animator anim = GetComponent<Animator> ();
		anim.SetFloat ("Speed", CurrentSpeed);
		anim.SetBool ("Jumping", Jumping);
	}

	/// <summary>
	/// Check the Player Grounding Status
	/// </summary>
	/// <returns>Grounding Interger Value</returns>
	private int CheckGround()
	{
		bool g1 = Physics.Linecast (DownStart.position, RightDown.position);
		bool g2 = Physics.Linecast (DownStart.position, LeftDown.position);

		// We are fully grounded both sensors are intersecting
		if (g1 && g2) {
			FallSpeed = 0f;
			Jumping = false;
			return 1;
		}
		// Near a ledge, this is when we need to change to the Balancing Animations
		if (g1 == false && g2 == true) {
			FallSpeed = 0f;
			Jumping = false;
			return 2;
		}
		if (g2 == false && g1 == true) {
			FallSpeed = 0f;
			Jumping = false;
			return 3;		
		// Nothing below us, oh crap were falling.
		} else {
			return 0;
		}
	}
}
