using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// User configurable options
	public float Gravity = 0.21875f;
	public float MaxFall = 16f;
	public float Jump = 6.5f;
	public float Acceleration = 0.046875f;
	public float Friction = 0.046875f;
	public float Deceleration = 0.5f;
	public float TopSpeed = 6f;


	// Private Objects
	private Rect box;
	private LayerMask lmGround;
	public float CurrentSpeed = 0f;
	private int PlayerDirection = 0;
	private float FallSpeed = 0f;
	private bool Jumping = false;
	private bool StartJump = false;

	// Ground Sensor Values
	private Vector2 SensorGroundA, SensorGroundB;


	// Checks
	bool grounded = false;
	bool falling = false;

	// variables for raycasting
	private int verticalRays = 2;
	private int margin = 2;

	// Velocity and Rotation
	private Vector2 velocity;
	private float YRotation;

	void Start()
	{
		// Get layer masks by name rather than Int
		lmGround = LayerMask.NameToLayer ("Ground");
	}

	void FixedUpdate()
	{
		// Grab out box collider and make rect object box for easier user
		BoxCollider collider = GetComponent<BoxCollider>();
		box = new Rect (collider.bounds.min.x,
		               collider.bounds.min.y,
		               collider.bounds.size.x,
		               collider.bounds.size.y);
		GroundState ();
		DMovement ();
		if (Input.GetKey (KeyCode.A) && grounded) {
			StartJump = true;
		}
		if (StartJump && Input.GetKey(KeyCode.A)){
			Jumping = true;
			StartJump = false;
			velocity = new Vector2 (velocity.x, Jump);
		}
		else if (StartJump)
		{
			Jumping = true;
			StartJump = false;
			velocity = new Vector2 (velocity.x, 4f);
		}
		UpdateAnimations ();
		transform.eulerAngles = new Vector2(0,YRotation);
		transform.Translate (velocity.x,velocity.y,0f);
	}

	void OnGUI()
	{
		GUILayout.Label ("Grounded: " + grounded + ", Falling: " + falling + ", Jumping: " + Jumping);
		GUILayout.Label ("Location: " + transform.position.ToString());
		GUILayout.Label ("Velocity: " + velocity.ToString ());
		GUILayout.Label ("Sensor A: " + SensorGroundA.ToString() + ", Sensor B: " + SensorGroundB.ToString ());
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
			YRotation = 180;
			
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
			YRotation = 0;
			
		} else {
			CurrentSpeed = CurrentSpeed - (Mathf.Min(Mathf.Abs(CurrentSpeed), Friction)*Mathf.Sign(CurrentSpeed));
		}
		velocity = new Vector2 (Mathf.Abs(CurrentSpeed), velocity.y);		
		
	}

	void UpdateAnimations()
	{
		Animator anim = GetComponent<Animator> ();
		anim.SetFloat ("Speed", CurrentSpeed);
		anim.SetBool ("Jumping", Jumping);
	}

	void GroundState()
	{
		Vector3 vGaStart, vGbStart;

		// Not grounded apply gravity
		if (!grounded) {
			velocity = new Vector2 (velocity.x, Mathf.Max (velocity.y - Gravity, -MaxFall));
		}
		// Were falling
		if (velocity.y < 0) {
			falling = true;
		}
		if (grounded || falling) {

			// If not jumping then rays are 9 pixels apart from X=0 of the sprite
			if (!Jumping)
			{
				vGaStart = new Vector3(box.center.x + 9,box.center.y,transform.position.z);
				vGbStart = new Vector3(box.center.x - 9,box.center.y,transform.position.z);
			}
			// Else set the rays 7 pixels apart from X=0 of the sprite
			else
			{
				vGaStart = new Vector3(box.center.x + 7,box.center.y,transform.position.z);
				vGbStart = new Vector3(box.center.x - 7,box.center.y,transform.position.z);
			}

			RaycastHit hGa, hGb;

			float distance = box.height/2+(grounded? margin: Mathf.Abs (velocity.y));

			// No were not connected, no yet anyway
			bool bGaConnected = false;
			bool bGbConnected = false;

			// Make the ray vectors
			Ray rGa = new Ray(vGaStart,Vector3.down);
			Ray rGb = new Ray(vGbStart,Vector3.down);

			// Debug this shiz
			Debug.DrawRay(vGaStart,Vector3.down * distance,Color.green,2f);
			Debug.DrawRay(vGbStart,Vector3.down * distance,Color.green,2f);

			bGaConnected = Physics.Raycast(rGa, out hGa, distance,1 << lmGround); // Shoot out Ray A and set layer mask to ground
			bGbConnected = Physics.Raycast(rGb, out hGb, distance,1 << lmGround); // Shoot out Ray B and set layer mask to ground

			// If anything collides were on the floor
			if (bGaConnected || bGbConnected)
			{
				grounded = true;
				falling = false;
				Jumping = false;

				// Store out the sensor values
				if (hGa.collider != null) {SensorGroundA = hGa.point;}
				if (hGb.collider != null) {SensorGroundB = hGb.point;}

				// Fixes a weird bug where a and b although the same height seem to give different distances
				if (hGb.distance > hGa.distance)
				{
					transform.Translate(Vector3.down * (hGb.distance - box.height/2)); // Places the transform on the ground
				}
				else if (hGa.distance > hGb.distance)
				{
					transform.Translate(Vector3.down * (hGa.distance - box.height/2)); // Places the transform on the ground
				}
				else
				{
					transform.Translate(Vector3.down * (hGa.distance - box.height/2)); // Places the transform on the ground
				}

				velocity = new Vector2(velocity.x, 0);
			}

			// Uh Oh not on the ground here, were going to fall :O
			if (!bGaConnected && !bGbConnected){
				grounded = false;
			}
		}

	}
}