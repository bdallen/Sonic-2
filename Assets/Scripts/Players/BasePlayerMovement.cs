using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BasePlayerMovement : MonoBehaviour
{

    #region Constants
    private const float FACING_RIGHT = 0f;
    private const float FACING_LEFT = 180f;
    #endregion

    #region Public Variables
    // Player Tuning
    public int LIVES = 3;
    public int RINGS = 0;
    public int EMERALDS = 0;
    #endregion

    #region Private Variables
    // Game Manager
    private GameManager _gm;

    // Player Dynamics
    private float GRAVITY = 0.21875f;
    private float MAX_FALL_VELOCITY = 16f;
    private float ACCELERATION = 0.046875f;
    private float FRICTION = 0.046875f;
    private float DECELERATION = 0.5f;
    private float TOP_SPEED = 6f;
    private float KILL_FORCE = 8.853656f;


    // Player Physics
    private float _maxJumpForce = 0f;
    private float _midJumpForce = 0f;

    // Player States Section
    private bool _inWater = false;

    // Player Variables Section
    private Vector3 _lastStarPoleHit;
    private Stack<Vector3> _playerLocationHistory;

    // Player Positioning
    private Transform _newPlayerPosition;

    private int _spinDashCounter = 0;

    // Current Player Dynamics In Use
    private int _currentGravity = 0;
    
    // Collision & Edge Detection
    private LayerMask lmGround;
    private LayerMask lmRoof;
    private LayerMask lmWalls;
    private Vector2 _hGaNormal;
    private Vector2 _hGbNormal;

    #endregion

    #region Protected Variables
    // Game Objects
    protected Animator _animator;
    protected Animator _subAnimator;
    protected AudioSource _audioSource;
    protected SpriteRenderer _spRenderer;
    protected int _gameTime;

    // Player Dynamics
    protected float AIR_MAX_JUMP_FORCE;
    protected float AIR_MID_JUMP_FORCE;
    protected float WATER_MAX_JUMP_FORCE;
    protected float WATER_MID_JUMP_FORCE;

    // Player Information
    protected Vector2 _angle;
    protected Vector3 axis;
    protected float _currentSpeed = 0f;
    protected bool _dead = false;
    protected bool _grounded = false;
    protected bool _jumping = false;
    protected bool _rolling = false;
    protected bool _isSuper = false;
    protected bool _spinDash = false;
    protected bool _hitWall = false;
    protected bool _edgeInfront = false;
    protected bool _edgeBehind = false;
    protected float _edgeDistance = 0f;
    protected int _idleStateCounter = 0;

    // Player Options
    protected string _PlayerCharacter;

    // Scripted Movement
    protected bool _scripted = false;
    #endregion

    #region Abstract Methods
    public abstract void UpdateCharacterAnimation();    // Updates specific Character Animations
    public abstract void CharacterAwake();              // Perform Awake on the Character Class
    #endregion

    /// <summary>
    /// Returns the Current Game Time
    /// </summary>
    public int PlayTime
    { get { return _gameTime; } }

    public string PlayerCharacter
    { get { return _PlayerCharacter; } }

    // Private Objects
	private Rect box;
	
    
	// Ground Sensor Values
	private Vector2 SensorGroundA, SensorGroundB;


	// Checks
	
	bool falling = false;

	// variables for raycasting
	private int margin = 1;

	// Velocity and Rotation
	private Vector2 velocity;
	private float YRotation = FACING_RIGHT;

    #region Unity Subroutines
    void Start()
	{
        // Grab the Game Manager for Global Objects
        _gm = GameObject.Find("_GameManager").GetComponent<GameManager>();

		// Get layer masks by name rather than Int
		lmGround = LayerMask.NameToLayer ("Ground");
        lmRoof = LayerMask.NameToLayer("Roof");
        lmWalls = LayerMask.NameToLayer("Walls");

        // Find the starting position of the character on the level
        transform.position = GameObject.Find("StartPos").transform.position;

        _spRenderer = GetComponent<SpriteRenderer>();
        //_inWater = true;

	}

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _subAnimator = GameObject.Find("DustAnimations").GetComponent<Animator>();
        CharacterAwake();
    }

    void OnGUI()
    {
        GUILayout.Label("Velocity: " + velocity.ToString());

    }
    

    /// <summary>
    /// Update Every Single Frame
    /// </summary>
    void Update()
    {
        // Update Game Time and counters
        _gameTime = (int)Time.timeSinceLevelLoad;

        Obj01_Control();
        
        if (YRotation == FACING_LEFT)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        //transform.eulerAngles = new Vector2(0, YRotation);
        transform.Translate(velocity.x, velocity.y, 0f);

    }

    #endregion

    void Obj01_Control()
    {

        //offsetTableEntry.w Obj01_MdNormal_Checks	; 0 - not airborne or rolling
        //offsetTableEntry.w Obj01_MdAir			; 2 - airborne
        //offsetTableEntry.w Obj01_MdRoll			; 4 - rolling
        //offsetTableEntry.w Obj01_MdJump			; 6 - jumping

        if (_grounded && !_rolling)
        {
            Obj01_MdNormalChecks();
        }

        if (!_grounded)
        {
            Obj01_MdAir();
        }

        UpdateAnimations();
    }

    /// <summary>
    /// Normal Checks when sonic is on the ground and in no other state
    /// </summary>
    void Obj01_MdNormalChecks()
    {
        // Check Logical Button Presses (Scripted Movement) If not then branch off
        if (!_scripted)
        {
            Obj01_MdNormal();
        }
    }

    void Obj01_MdNormal()
    {
        //bsr.w	Sonic_CheckSpindash
        //bsr.w	Sonic_Jump
        //bsr.w	Sonic_SlopeResist
        //bsr.w	Sonic_Move
        //bsr.w	Sonic_Roll
        //bsr.w	Sonic_LevelBound
        //jsr	(ObjectMove).l
        //bsr.w	AnglePos
        //bsr.w	Sonic_SlopeRepel

                      

        Player_Jump();              //bsr.w Sonic_Jump

        Player_Move();              //bsr.w Sonic_Move

        Player_LevelBound();        //bsr.w	Sonic_LevelBound
        Collision();

    }

    void Obj01_MdAir()
    {
        Player_JumpHeight();
        Player_ChgJumpDir();
        Player_LevelBound();

        if (_inWater)
        {
            
        }

        // TODO: JumpAngle

        ObjectMoveAndFall();
        Collision();
        
    }

    /// <summary>
    /// TODO: Check and Go Super
    /// </summary>
    void Player_CheckGoSuper()
    {
        if (_isSuper) { return; }            // Already Super - Exit Routine
        if (EMERALDS != 7) { return; }       // Doesn't have 7 Emeralds - Exit Routine
        if (RINGS < 50) { return; }          // Less than 50 Rings - Exit Routing

        _isSuper = true;                     // Set Super to True

        // TODO: Play Transformation Animation

        // TODO: Change Top Speed
        // TODO: Change Acceleration
        // TODO: Change Deceleration
        // TODO: Set Invincibility Counter to 0
        // TODO: Set Invicibility To True
        // TODO: Play Transformation Sound
        // TODO: Start playing Super Song


    }

    #region Jumping


    /// <summary>
    /// Do Jump
    /// TODO: Angle Jumps, Caluclate Room Overhead, 
    /// </summary>
    void Player_Jump()
    {
        /// When you release the jump button in the air after jumping, the computer checks to see if Sonic is moving upward (i.e. Y speed is negative). If he is, then it checks to see if Y speed is less than -4 (e.g. -5 is "less" than -4). If it is, then Y speed is set to -4. In this way, you can cut your jump short at any time, just by releasing the jump button. If you release the button in the very next step after jumping, Sonic makes the shortest possible jump.
        if (Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") >= 0)
        {
            _maxJumpForce = AIR_MAX_JUMP_FORCE;                        // Set Max Jump Force
            if (_isSuper) { }                                          // TODO: If Super - Change Max Force
            if (_inWater) { _maxJumpForce = WATER_MAX_JUMP_FORCE; }    // In Water - Change Max Force
                                                                
            _jumping = true;
            _grounded = false;
            velocity = new Vector2(velocity.x, _maxJumpForce);
            AudioClip SndJump = Resources.Load<AudioClip>("Sound/SFX/Player/Jump");
            _audioSource.PlayOneShot(SndJump);
        }
    }

    /// <summary>
    /// Variable Jump Heights
    /// </summary>
    void Player_JumpHeight()
    {
        if (_jumping)                                                            // Are we jumping?
        {
            _midJumpForce = AIR_MID_JUMP_FORCE;                                    // Set Mid Jump Force
            if (_inWater) { _midJumpForce = WATER_MID_JUMP_FORCE; }                // Change Mid Jump Force if in Water
            if (Input.GetButtonUp("Jump"))                                      // Button Released means we want a Mid Jump
            {
                if (velocity.y > _midJumpForce)
                    velocity = new Vector2(velocity.x, _midJumpForce);
            }
            else
            {
                if (velocity.y >= _maxJumpForce){ Player_CheckGoSuper(); }      // At the height of the jump - Test for Super
            }
        }
    }

    /// <summary>
    /// Change Jump Direction Mid Air
    /// </summary>
    void Player_ChgJumpDir()
    {
        if (_rolling)
        {

        }
        else
        {

            // Test axis to change mid air jump direction
            if (Input.GetAxis("Horizontal") < 0)
            {

                if (_currentSpeed > -TOP_SPEED)
                {
                    _currentSpeed = _currentSpeed - ACCELERATION;
                }
                else
                {
                    _currentSpeed = -TOP_SPEED;
                }
                YRotation = FACING_LEFT;

            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                if (_currentSpeed < TOP_SPEED)
                {
                    _currentSpeed = _currentSpeed + ACCELERATION;
                }
                else
                {
                    _currentSpeed = TOP_SPEED;
                }
                YRotation = FACING_RIGHT;
            }
        }

        // This section applies Air Drag
        if (velocity.y < 0f && velocity.y > -4f)
        {
            _currentSpeed = _currentSpeed - ((_currentSpeed / 0.125f) / 256f);
        }

        velocity = new Vector2(_currentSpeed, velocity.y);
    }

    #endregion

    /// <summary>
    /// Check the Level Bounds for the Player
    /// </summary>
    void Player_LevelBound()
    {
        // Check the Left Bounds of the player
        if (transform.position.x - (box.width / 2) <= _gm.leftBound.position.x)
        {
            transform.position = new Vector3(_gm.leftBound.position.x + (box.width / 2), transform.position.y, transform.position.z);
        }

        // Check the Right Bounds of the player
        if (transform.position.x - (box.width / 2) >= _gm.rightBound.position.x)
        {
            transform.position = new Vector3(_gm.rightBound.position.x + (box.width / 2), transform.position.y, transform.position.z);
        }

        // Check the Right Bounds of the player
        if (transform.position.y - (box.height / 2) <= _gm.bottomBound.position.y && !_dead)
        {
            KillCharacter();        // Player dies when they fall out of bounds!
        }

    }

    /// <summary>
    /// Kills the Player
    /// </summary>
    void KillCharacter()
    {
        _gm.SetCamFollowing = false;   // Camera is Not to follow movements
        _dead = true;
        _currentSpeed = 0;
        velocity = new Vector2(0, KILL_FORCE);

        // Need to check if hurt by spikes or general death as they have different sounds
        AudioClip HitDeath = Resources.Load<AudioClip>("Sound/SFX/Player/HitDeath");
        _audioSource.PlayOneShot(HitDeath);
    }



	void Player_Move()
	{

        if (Input.GetAxis("Horizontal") < 0)
        {
            // Player_MoveLeft      
            if (_currentSpeed > 0f)
            {
                _currentSpeed = _currentSpeed - DECELERATION;

                // Play Braking Sound
                if (YRotation == FACING_RIGHT && _currentSpeed < -4.5f && _jumping == false)
                { AudioClip SndBrake = Resources.Load<AudioClip>("Sound/SFX/Player/Brake"); _audioSource.PlayOneShot(SndBrake); }

            }
            else
            {
                if (_currentSpeed > -TOP_SPEED)
                {
                    _currentSpeed = _currentSpeed - ACCELERATION;
                }
                else
                {
                    _currentSpeed = -TOP_SPEED;
                }
            }

            // Flip sprite the other way
            YRotation = FACING_LEFT;

        }
        else if (Input.GetAxis("Horizontal") > 0 )
        {
            // Player_MoveRight
			if (_currentSpeed < 0f) 
            {
                _currentSpeed = _currentSpeed + DECELERATION;

                // Play braking Sound
                if (YRotation == FACING_LEFT && _currentSpeed > 4.5f && _jumping == false)
                { AudioClip SndBrake = Resources.Load<AudioClip>("Sound/SFX/Player/Brake"); _audioSource.PlayOneShot(SndBrake); }

			} 
            else 
            {
				if (_currentSpeed < TOP_SPEED)
				{
					_currentSpeed = _currentSpeed + ACCELERATION;
				}
                else
                {
					_currentSpeed = TOP_SPEED;
				}
			}

            // Turn Sprite Right
            YRotation = FACING_RIGHT;
			
		} else {
			_currentSpeed = _currentSpeed - (Mathf.Min(Mathf.Abs(_currentSpeed), FRICTION)*Mathf.Sign(_currentSpeed));
		}

		velocity = new Vector2 (_currentSpeed, velocity.y);	
	
		
	}

	void UpdateAnimations()
	{
		UpdateCharacterAnimation();
	}

    void ObjectMoveAndFall()
    {

        velocity = new Vector2(velocity.x, Mathf.Max(velocity.y - GRAVITY, -MAX_FALL_VELOCITY));

        // Were falling
        if (velocity.y < 0)
        {
            falling = true;
        }

    }

	void Collision()
	{
        // Grab out box collider and make rect object box for easier user
        BoxCollider collider = GetComponent<BoxCollider>();
        box = new Rect(collider.bounds.min.x,
                       collider.bounds.min.y,
                       collider.bounds.size.x,
                       collider.bounds.size.y);

		Vector3 vGaStart, vGbStart, vMaStart, vMbStart;

        // Check if we are jumping, if so change the width of the Sensors
        // Also check the roataion and swap the A and B sensors depending on what way we are facing
        switch (_jumping)
        {
            case true:
                vGaStart = new Vector3(box.center.x + 7, box.center.y, transform.position.z);
                vGbStart = new Vector3(box.center.x - 7, box.center.y, transform.position.z);
                break;
            default:
                vGaStart = new Vector3(box.center.x + 9, box.center.y, transform.position.z);
                vGbStart = new Vector3(box.center.x - 9, box.center.y, transform.position.z);
                break;
        }

        #region Ground Collision
        if (_grounded || falling) {

            RaycastHit2D hGa, hGb;

            // TODO: Fix this
            // This may need to be revised - This is what was causing the bouncing issue, but dividing the velocity has helped
            float distance = (box.height / 2) +(_grounded? margin: Mathf.Abs (velocity.y)/1.5f);


			// No were not connected, no yet anyway
			bool bGaConnected = false;
			bool bGbConnected = false;

			// Make the ray vectors
			Ray2D rGa = new Ray2D(vGaStart,Vector3.down);
			Ray2D rGb = new Ray2D(vGbStart,Vector3.down);

			// Debug this shiz
			Debug.DrawRay(vGaStart,Vector3.down * distance,Color.green,2f);
			Debug.DrawRay(vGbStart,Vector3.down * distance,Color.green,2f);

            hGa = Physics2D.Raycast(vGaStart, -Vector2.up, distance, 1 << lmGround);
            hGb = Physics2D.Raycast(vGbStart, -Vector2.up, distance, 1 << lmGround);

			// If anything collides were on the floor
			if (hGa.collider || hGb.collider)
			{
           
                _jumping = false;
				_grounded = true;
				falling = false;
			
                //// Store out the sensor values
                if (hGa.collider != null) { bGaConnected = true; SensorGroundA = hGa.point; }
                if (hGb.collider != null) { bGbConnected = true; SensorGroundB = hGb.point; }

                // Fixes a weird bug where a and b although the same height seem to give different distances
                if (hGa.distance > hGb.distance)
                {
                    transform.Translate(Vector3.down * (hGa.distance - box.height / 2)); // Places the transform on the ground

                }
                else if (hGa.distance < hGb.distance)
                {
                    transform.Translate(Vector3.down * (hGb.distance - box.height / 2)); // Places the transform on the ground
                }

                _hGaNormal = hGa.normal;
                _hGbNormal = hGb.normal;


                velocity = new Vector2(velocity.x, 0);

			}
            else
            {
                _grounded = false;
            }


			// Edge detection for the Balancing Animations
            if (!bGaConnected && bGbConnected && YRotation == FACING_RIGHT && !_jumping)         // We are facing right and edge is infront of us
            {
                _edgeInfront = true;
                _edgeBehind = false;
                _edgeDistance = Mathf.Abs(((SensorGroundA.x - hGb.point.x) / 2));
            }
            else if (!bGaConnected && bGbConnected && YRotation == FACING_LEFT && !_jumping)    // We are facing right and the ledge is behind us
            {
                _edgeInfront = false;
                _edgeBehind = true;
                _edgeDistance = Mathf.Abs(((SensorGroundA.x - hGb.point.x) / 2));
            }
            else if (bGaConnected && !bGbConnected && YRotation == FACING_LEFT && !_jumping)   // We are facing left and the ledge is infront of us
            {
                _edgeInfront = true;
                _edgeBehind = false;
                _edgeDistance = Mathf.Abs(((SensorGroundB.x - hGa.point.x) / 2));
            }
            else if (bGaConnected && !bGbConnected && YRotation == FACING_RIGHT && !_jumping)  // We are facing right and the ledge is behind us
            {
                _edgeInfront = false;
                _edgeBehind = true;
                _edgeDistance = Mathf.Abs(((SensorGroundB.x - hGa.point.x) / 2));
            }
            else
            {
                _edgeInfront = false;
                _edgeBehind = false;
            }
        }
        #endregion

        #region Side Collision
        vMaStart = new Vector3(box.center.x, box.center.y, transform.position.z);
        Debug.DrawRay(vMaStart, Vector2.right * 11f, Color.cyan, 2f);
        RaycastHit2D hMa = Physics2D.Raycast(vMaStart, Vector2.right, 11f);

        vMbStart = new Vector3(box.center.x, box.center.y, transform.position.z);
        Debug.DrawRay(vMbStart, -Vector2.right * 11f, Color.cyan, 2f);
        RaycastHit2D hMb = Physics2D.Raycast(vMbStart, -Vector2.right, 11f);

        if (hMa.collider)
        {
            DoCollisionCheck(hMa);
        }
        if (hMb.collider)
        {
            DoCollisionCheck(hMb);
        }
        #endregion

        #region Top Collision
        Debug.DrawRay(vGaStart, Vector3.up * 16f, Color.cyan, 2f);
        RaycastHit2D hTa = Physics2D.Raycast(vGaStart, Vector2.up, 16f);

        Debug.DrawRay(vGbStart, Vector3.up * 16f, Color.cyan, 2f);
        RaycastHit2D hTb = Physics2D.Raycast(vGbStart, Vector2.up, 16f);

        if (hTa.collider)
        {
            DoCollisionCheck(hTa);
        }
        if (hTb.collider)
        {
            DoCollisionCheck(hTb);
        }
        #endregion

        #region Bottom Collision
        Debug.DrawRay(vGaStart, -Vector2.up * 16f, Color.cyan, 2f);
        RaycastHit2D hBa = Physics2D.Raycast(vGaStart, -Vector2.up, 16f);

        Debug.DrawRay(vGbStart, -Vector2.up * 16f, Color.cyan, 2f);
        RaycastHit2D hBb = Physics2D.Raycast(vGbStart, -Vector2.up, 16f);

        if (hBa.collider)
        {
            DoCollisionCheck(hBa);
        }
        if (hBb.collider)
        {
            DoCollisionCheck(hBb);
        }
        #endregion

        _hitWall = false;

    }

    void Player_DoLevelCollision()
    {

    }

    void DoCollisionCheck(RaycastHit2D _rc)
    {

        // Handle Wall Hits
        if (_rc.transform.gameObject.layer == lmWalls)
        {
            velocity = new Vector2(0, velocity.y);
            _hitWall = true;
            transform.Translate(Vector3.right * (_rc.distance - 3f));
        }
        else
        {
            _hitWall = false;
        }

        // Handle Rings Hits
        if (_rc.collider.GetComponent("Ring"))
        {
            Ring ring = _rc.collider.GetComponent<Ring>();

            // If Ring isn't collected but we have hit it then do this
            if (!ring._collected) 
            { 
                RINGS += 1;

                // When rings gets to a block of 100, r2l will be 0
                int r2l = RINGS % 100;
                
                // If the block has come back to 0, we have 100 Rings
                if (r2l == 0)
                {
                    AudioClip snd1up = Resources.Load<AudioClip>("Sound/BGM/1Up");
                    _audioSource.PlayOneShot(snd1up);
                    LIVES += 1;
                }
            }

            // Then collect it to say we have used the object, poor rings, getting used
            ring.CollectRing();
        }
    }
}