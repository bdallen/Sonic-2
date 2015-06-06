using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    #region Constants
    private const float FACING_RIGHT = 0f;
    private const float FACING_LEFT = 180f;
    #endregion

    #region Public Variables
    public float GRAVITY = 0.21875f;
    public float MAX_FALL_VELOCITY = 16f;
    public float MAX_JUMP_FORCE = 6.5f;
    public float MIN_JUMP_FORCE = 4f;
    public float ACCELERATION = 0.046875f;
    #endregion


    #region Player States
    //	; ===========================================================================
	//		; off_19F6A: Obj01_States:
	//		Obj01_Index:	offsetTable
	//			offsetTableEntry.w Obj01_Init		;  0
	//		offsetTableEntry.w Obj01_Control	    ;  2
	//		offsetTableEntry.w Obj01_Hurt		    ;  4
	//		offsetTableEntry.w Obj01_Dead		    ;  6
	//		offsetTableEntry.w Obj01_Gone		    ;  8
	//		offsetTableEntry.w Obj01_Respawning	    ; 10

	private int PlayerState = 0;

	#endregion

	#region Player Modes

	//  ; ===========================================================================
	//	; secondary states under state Obj01_Control
	//	; off_1A0BE:
	//	Obj01_Modes:	offsetTable
	//	offsetTableEntry.w Obj01_MdNormal_Checks	; 0 - not airborne or rolling
	//	offsetTableEntry.w Obj01_MdAir			    ; 2 - airborne
	//	offsetTableEntry.w Obj01_MdRoll			    ; 4 - rolling
	//	offsetTableEntry.w Obj01_MdJump			    ; 6 - jumping
	private int PlayerMode = 0;

	#endregion

    #region Private Variables
    private bool _jump = false;
    private bool _jumping = false;
    private bool _jumpCancel = false;
    #endregion


    public float Friction = 0.046875f;
	public float Deceleration = 0.5f;
	public float TopSpeed = 6f;


	// Private Objects
	private Rect box;
	private LayerMask lmGround;
	public float CurrentSpeed = 0f;
	private float FallSpeed = 0f;

	private bool StartJump = false;
    
	private int JumpHold = 0;		// Counts if jump is held down
    private int JumpHold1 = 0;
	private bool FrontEdge = false;
	private bool BackEdge = false;

	private float FEdgeDistance = 0f;



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

	}

    void Update()
    {
        // Grab out box collider and make rect object box for easier user
        BoxCollider collider = GetComponent<BoxCollider>();
        box = new Rect(collider.bounds.min.x,
                       collider.bounds.min.y,
                       collider.bounds.size.x,
                       collider.bounds.size.y);
        GroundState();
        Jumping();
        DMovement();
        UpdateAnimations();
        transform.eulerAngles = new Vector2(0, YRotation);
        transform.Translate(velocity.x, velocity.y, 0f);
    }

	void OnGUI()
	{
		GUILayout.Label ("Grounded: " + grounded + ", Falling: " + falling + ", Jumping: " + _jumping + ", Front Edge: " + FrontEdge + ", Back Edge: " + BackEdge);
		GUILayout.Label ("Location: " + transform.position.ToString());
		GUILayout.Label ("Velocity: " + velocity.ToString ());
		GUILayout.Label ("Sensor A: " + SensorGroundA.ToString() + ", Sensor B: " + SensorGroundB.ToString ());
		GUILayout.Label ("Jump Hold: " + JumpHold.ToString ());
		GUILayout.Label ("Edge Detect Distance: " + FEdgeDistance.ToString());
	}

    void Jumping()
    {

        if (Input.GetButtonDown("Jump") && grounded)
        {
            _jumping = true;
            _jump = true;
        }
        if (_jump)
        {
            velocity = new Vector2(velocity.x, MAX_JUMP_FORCE);
            _jump = false;
        }
        if (Input.GetButtonUp("Jump") && !grounded)
        {
            _jumpCancel = true;
        }
        if (_jumpCancel)
        {
            if (velocity.y > MIN_JUMP_FORCE)
                velocity = new Vector2(velocity.x, MIN_JUMP_FORCE);
            _jumpCancel = false;
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
					CurrentSpeed = CurrentSpeed - ACCELERATION;
				}else{
					CurrentSpeed = -TopSpeed;
				}
			}
			
			// Turn Sprite Left
			YRotation = FACING_LEFT;
			
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			
			if (CurrentSpeed < 0) {
				CurrentSpeed = CurrentSpeed + Deceleration;
			} else {
				if (CurrentSpeed < TopSpeed)
				{
					CurrentSpeed = CurrentSpeed + ACCELERATION;
				}else{
					CurrentSpeed = TopSpeed;
				}
			}
			
			// Turn Sprite Right
			YRotation = FACING_RIGHT;
			
		} else {
			CurrentSpeed = CurrentSpeed - (Mathf.Min(Mathf.Abs(CurrentSpeed), Friction)*Mathf.Sign(CurrentSpeed));
		}
		velocity = new Vector2 (Mathf.Abs(CurrentSpeed), velocity.y);		
		
	}

	void UpdateAnimations()
	{
		Animator anim = GetComponent<Animator> ();
		anim.SetFloat ("Speed", CurrentSpeed);
		anim.SetBool ("Jumping", _jumping);
	}

	void GroundState()
	{
		Vector3 vGaStart, vGbStart;

		// Not grounded apply gravity
		if (!grounded) {
			velocity = new Vector2 (velocity.x, Mathf.Max (velocity.y - GRAVITY, -MAX_FALL_VELOCITY));
		}
		// Were falling
		if (velocity.y < 0) {
			falling = true;
		}
		if (grounded || falling) {

			// Check if we are jumping, if so change the width of the Sensors
			// Also check the roataion and swap the A and B sensors depending on what way we are facing
			switch(_jumping)
			{
			case true:
				if (YRotation == FACING_RIGHT)
				{
					vGaStart = new Vector3(box.center.x + 7,box.center.y,transform.position.z);
					vGbStart = new Vector3(box.center.x - 7,box.center.y,transform.position.z);
				}else
				{
					vGaStart = new Vector3(box.center.x - 7,box.center.y,transform.position.z);
					vGbStart = new Vector3(box.center.x + 7,box.center.y,transform.position.z);
				}
				break;
			default:
				if (YRotation == FACING_RIGHT)
				{
					vGaStart = new Vector3(box.center.x + 9,box.center.y,transform.position.z);
					vGbStart = new Vector3(box.center.x - 9,box.center.y,transform.position.z);
				}else
				{
					vGaStart = new Vector3(box.center.x - 9,box.center.y,transform.position.z);
					vGbStart = new Vector3(box.center.x + 9,box.center.y,transform.position.z);
				}
				break;
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
                _jumping = false;
				grounded = true;
				falling = false;
			
				// Store out the sensor values
				if (hGa.collider != null) {SensorGroundA = hGa.point;}
				if (hGb.collider != null) {SensorGroundB = hGb.point;}

				// Fixes a weird bug where a and b although the same height seem to give different distances
				if (hGa.distance > hGb.distance)
				{
					transform.Translate(Vector3.down * (hGa.distance - box.height/2)); // Places the transform on the ground
				}
				else if (hGa.distance < hGb.distance)
				{
					transform.Translate(Vector3.down * (hGb.distance - box.height/2)); // Places the transform on the ground
				}

				velocity = new Vector2(velocity.x, 0);
			}

			// Uh Oh not on the ground here, were going to fall :O
			if (!bGaConnected && !bGbConnected){
				grounded = false;
				FrontEdge = false;
			}

			// Edge detection for the Balancing Animations
			if (!bGaConnected && bGbConnected && !_jumping)
			{
				FrontEdge = true;
				FEdgeDistance = ((SensorGroundA.x - hGb.point.x) / 2);
			}
			else if (bGaConnected && bGbConnected)
			{
				FrontEdge = false;
			}
		}
	}

}