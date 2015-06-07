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
    public float FRICTION = 0.046875f;
    public float DECELERATION = 0.5f;
    public float TOP_SPEED = 6f;
    #endregion


    #region Player States
    //	    ; ===========================================================================
	//		; off_19F6A: Obj01_States:
	//		Obj01_Index:	offsetTable
	//		offsetTableEntry.w Obj01_Init		    ;  0
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
    private bool _moveLeft = false;
    private bool _moveRight = false;
    private bool _edgeFront = false;
    private bool _edgeBack = false;
    private float _edgeDistance = 0f;
    #endregion


	// Private Objects
	private Rect box;
	private LayerMask lmGround;
	public float CurrentSpeed = 0f;
	private float FallSpeed = 0f;

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
	private float YRotation = FACING_RIGHT;

	void Start()
	{
		// Get layer masks by name rather than Int
		lmGround = LayerMask.NameToLayer ("Ground");
	}

    /// <summary>
    /// Update at a set rate of 50 Cycles per Second (This keeps compatability with the Megadrive)
    /// </summary>
	void FixedUpdate()
	{
        // Grab out box collider and make rect object box for easier user
        BoxCollider collider = GetComponent<BoxCollider>();
        box = new Rect(collider.bounds.min.x,
                       collider.bounds.min.y,
                       collider.bounds.size.x,
                       collider.bounds.size.y);
        GroundState();
        CheckJump();
        DMovement();
        transform.eulerAngles = new Vector2(0, YRotation);
        transform.Translate(velocity.x, velocity.y, 0f);
	}

    /// <summary>
    /// Update Every Single Frame
    /// </summary>
    void Update()
    {
        KeyUpdates();
        UpdateAnimations();
    }

	void OnGUI()
	{
        //GUILayout.Label ("Grounded: " + grounded + ", Falling: " + falling + ", Jumping: " + _jumping + ", Front Edge: " + FrontEdge + ", Back Edge: " + BackEdge);
		GUILayout.Label ("Location: " + transform.position.ToString());
		GUILayout.Label ("Velocity: " + velocity.ToString ());
        GUILayout.Label("Current Speed: " + CurrentSpeed.ToString());
		GUILayout.Label ("Sensor A: " + SensorGroundA.ToString() + ", Sensor B: " + SensorGroundB.ToString ());
		GUILayout.Label ("Edge Detect Distance: " + _edgeDistance.ToString());
	}

    /// <summary>
    /// Checks if mapped keys have been pressed, needs to be in the Update() section to allow fluid keypress detection.
    /// </summary>
    void KeyUpdates()
    {
        // Handle Jump
        if (Input.GetButtonDown("Jump") && grounded) { _jump = true; }
        if (Input.GetButtonUp("Jump") && !grounded) { _jumpCancel = true; }

        // Handle D Movement
        if (Input.GetKey(KeyCode.LeftArrow))  { _moveLeft = true; } else { _moveLeft = false; }
        if (Input.GetKey(KeyCode.RightArrow)) { _moveRight = true; } else { _moveRight = false; }
    }

    /// <summary>
    /// Check For Jumps
    /// </summary>
    void CheckJump()
    {
        /// When you release the jump button in the air after jumping, the computer checks to see if Sonic is moving upward (i.e. Y speed is negative). If he is, then it checks to see if Y speed is less than -4 (e.g. -5 is "less" than -4). If it is, then Y speed is set to -4. In this way, you can cut your jump short at any time, just by releasing the jump button. If you release the button in the very next step after jumping, Sonic makes the shortest possible jump.
        
        #region Jump Assembly
        //        ; ---------------------------------------------------------------------------
        //; Subroutine allowing Sonic to jump
        //; ---------------------------------------------------------------------------

        //; ||||||||||||||| S U B R O U T I N E |||||||||||||||||||||||||||||||||||||||

        //; loc_1AA38:
        //Sonic_Jump:
        //    move.b	(Ctrl_1_Press_Logical).w,d0
        //    andi.b	#button_B_mask|button_C_mask|button_A_mask,d0 ; is A, B or C pressed?
        //    beq.w	return_1AAE6	; if not, return
        //    moveq	#0,d0
        //    move.b	angle(a0),d0
        //    addi.b	#$80,d0
        //    bsr.w	CalcRoomOverHead
        //    cmpi.w	#6,d1			; does Sonic have enough room to jump?
        //    blt.w	return_1AAE6		; if not, branch
        //    move.w	#$680,d2
        //    tst.b	(Super_Sonic_flag).w
        //    beq.s	+
        //    move.w	#$800,d2	; set higher jump speed if super
        //+
        //    btst	#6,status(a0)	; Test if underwater
        //    beq.s	+
        //    move.w	#$380,d2	; set lower jump speed if under
        //+
        //    moveq	#0,d0
        //    move.b	angle(a0),d0
        //    subi.b	#$40,d0
        //    jsr	(CalcSine).l
        //    muls.w	d2,d1
        //    asr.l	#8,d1
        //    add.w	d1,x_vel(a0)	; make Sonic jump (in X... this adds nothing on level ground)
        //    muls.w	d2,d0
        //    asr.l	#8,d0
        //    add.w	d0,y_vel(a0)	; make Sonic jump (in Y)
        //    bset	#1,status(a0)
        //    bclr	#5,status(a0)
        //    addq.l	#4,sp
        //    move.b	#1,jumping(a0)
        //    clr.b	stick_to_convex(a0)
        //    move.w	#SndID_Jump,d0
        //    jsr	(PlaySound).l	; play jumping sound
        //    move.b	#$13,y_radius(a0)
        //    move.b	#9,x_radius(a0)
        //    btst	#2,status(a0)
        //    bne.s	Sonic_RollJump
        //    move.b	#$E,y_radius(a0)
        //    move.b	#7,x_radius(a0)
        //    move.b	#AniIDSonAni_Roll,anim(a0)	; use "jumping" animation
        //    bset	#2,status(a0)
        //    addq.w	#5,y_pos(a0)

        //return_1AAE6:
        //    rts
        //; ---------------------------------------------------------------------------
        //; loc_1AAE8:
        //Sonic_RollJump:
        //    bset	#4,status(a0)	; set the rolling+jumping flag
        //    rts
        //; End of function Sonic_Jump
        

        //; ---------------------------------------------------------------------------
        //; Subroutine letting Sonic control the height of the jump
        //; when the jump button is released
        //; ---------------------------------------------------------------------------

        //; ||||||||||||||| S U B R O U T I N E |||||||||||||||||||||||||||||||||||||||

        //        ; ===========================================================================
        //; loc_1AAF0:
        //Sonic_JumpHeight:
        //    tst.b	jumping(a0)	; is Sonic jumping?
        //    beq.s	Sonic_UpVelCap	; if not, branch

        //    move.w	#-$400,d1
        //    btst	#6,status(a0)	; is Sonic underwater?
        //    beq.s	+		; if not, branch
        //    move.w	#-$200,d1
        //+
        //    cmp.w	y_vel(a0),d1	; is Sonic going up faster than d1?
        //    ble.s	+		; if not, branch
        //    move.b	(Ctrl_1_Held_Logical).w,d0
        //    andi.b	#button_B_mask|button_C_mask|button_A_mask,d0 ; is a jump button pressed?
        //    bne.s	+		; if yes, branch
        //    move.w	d1,y_vel(a0)	; immediately reduce Sonic's upward speed to d1
        //+
        //    tst.b	y_vel(a0)		; is Sonic exactly at the height of his jump?
        //    beq.s	Sonic_CheckGoSuper	; if yes, test for turning into Super Sonic
        //    rts
        //; ---------------------------------------------------------------------------
        //; loc_1AB22:
        //Sonic_UpVelCap:
        //    tst.b	spindash_flag(a0)	; is Sonic charging a spindash or in a rolling-only area?
        //    bne.s	return_1AB36		; if yes, return
        //    cmpi.w	#-$FC0,y_vel(a0)	; is Sonic moving up really fast?
        //    bge.s	return_1AB36		; if not, return
        //    move.w	#-$FC0,y_vel(a0)	; cap upward speed

        //return_1AB36:
        //    rts
        //; End of subroutine Sonic_JumpHeight
        #endregion

        if (_jump)
        {
            _jumping = true;
            velocity = new Vector2(velocity.x, MAX_JUMP_FORCE);
            _jump = false;
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
		if (_moveLeft) {
			
			if (CurrentSpeed > 0f) {
				CurrentSpeed = CurrentSpeed - DECELERATION;
			} else {
				if (CurrentSpeed > -TOP_SPEED)
				{
					CurrentSpeed = CurrentSpeed - ACCELERATION;
				}else{
					CurrentSpeed = -TOP_SPEED;
				}
			}
			
			// Turn Sprite Left
			YRotation = FACING_LEFT;
			
		} else if (_moveRight) {
			
			if (CurrentSpeed < 0f) {
                CurrentSpeed = CurrentSpeed + DECELERATION;
			} else {
				if (CurrentSpeed < TOP_SPEED)
				{
					CurrentSpeed = CurrentSpeed + ACCELERATION;
				}else{
					CurrentSpeed = TOP_SPEED;
				}
			}
			
			// Turn Sprite Right
			YRotation = FACING_RIGHT;
			
		} else {
			CurrentSpeed = CurrentSpeed - (Mathf.Min(Mathf.Abs(CurrentSpeed), FRICTION)*Mathf.Sign(CurrentSpeed));
		}

		velocity = new Vector2 (Mathf.Abs(CurrentSpeed), velocity.y);		
		
	}

	void UpdateAnimations()
	{
		Animator anim = GetComponent<Animator> ();
		anim.SetFloat ("Speed", CurrentSpeed);
		anim.SetBool ("Jumping", _jumping);
        anim.SetFloat("EdgeDistance", _edgeDistance);
        anim.SetBool("FrontEdge", _edgeFront);
        anim.SetBool("BackEdge", _edgeBack);
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
				_edgeFront = false;
			}

			// Edge detection for the Balancing Animations
			if (!bGaConnected && bGbConnected && !_jumping)
			{
				_edgeFront = true;
                _edgeBack = false;
				_edgeDistance = Mathf.Abs(((SensorGroundA.x - hGb.point.x) / 2));
			}
            else if (bGaConnected && !bGbConnected && !_jumping)
            {
                _edgeFront = false;
                _edgeBack = true;
                _edgeDistance = Mathf.Abs(((SensorGroundA.x - hGb.point.x) / 2));
            }
            else
            {
                _edgeBack = false;
                _edgeFront = false;
            }
		}
	}

}