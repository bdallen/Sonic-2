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

	// Checks
	bool grounded = false;
	bool falling = false;

	// variables for raycasting
	private int verticalRays = 4;
	private int margin = 2;

	// Velocity
	private Vector2 velocity;

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
		UpdateAnimations ();
		transform.Translate (velocity);
	}

	void OnGUI()
	{
		GUILayout.Label ("Grounded: " + grounded + ", Falling: " + falling);
		GUILayout.Label ("Location: " + transform.position.ToString());
		GUILayout.Label ("Velocity: " + velocity.ToString ());
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
		// Not grounded apply gravity
		if (!grounded) {
			velocity = new Vector2 (velocity.x, Mathf.Max (velocity.y - Gravity, -MaxFall));
		}
		// Were falling
		if (velocity.y < 0) {
			falling = true;
		}
		//
		if (grounded || falling) {
			Vector3 startPoint = new Vector3(box.xMin + margin, box.center.y, transform.position.z);
			Vector3 endPoint = new Vector3(box.xMax - margin, box.center.y, transform.position.z);

			RaycastHit hitInfo;

			float distance = box.height/2+(grounded? margin: Mathf.Abs (velocity.y));

			bool connected = false;

			for (int i =0; i < verticalRays; i++){
				float lerpAmount = (float)i / (float) verticalRays -1;
				Vector3 origin = Vector3.Lerp (startPoint, endPoint, lerpAmount);
				Ray ray = new Ray(origin, Vector3.down);

				connected = Physics.Raycast(ray, out hitInfo, distance,1 << lmGround);

				if (connected){
					grounded = true;
					falling = false;
					transform.Translate(Vector3.down * (hitInfo.distance - box.height/2));
					velocity = new Vector2(velocity.x, 0);
					break;
				}
			}
			if (!connected){
				grounded = false;
			}
		}

	}

}