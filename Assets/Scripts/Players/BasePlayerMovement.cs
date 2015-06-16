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

    public AudioClip SndJump;
    public AudioClip SndBrake;
    #endregion

    #region Private Variables
    // Player Dynamics
    private float GRAVITY = 0.21875f;
    private float MAX_FALL_VELOCITY = 16f;
    private float MAX_JUMP_FORCE = 6.5f;
    private float MIN_JUMP_FORCE = 4f;
    private float ACCELERATION = 0xC;
    private float FRICTION = 0xC;
    private float DECELERATION = 0x80;
    private float TOP_SPEED = 0x600;

    // Player States Section
    private bool _inWater = false;

    // Player Variables Section
    private Vector3 _savePoint;
    private List<Vector3> _playerPosBuf;

    private int _spinDashCounter = 0;
    

    // Collision & Edge Detection
    private LayerMask lmGround;
    private LayerMask lmRoof;
    private LayerMask lmWalls;
    #endregion

    #region Protected Variables
    // Game Objects
    protected Animator _animator;
    protected Animator _subAnimator;
    protected AudioSource _audioSource;
    protected SpriteRenderer _spRenderer;
    protected int _gameTime;

    // Player Information
    protected float _angle = 0f;
    protected float _currentSpeed = 0f;
    protected bool _grounded = false;
    protected bool _jumping = false;
    protected bool _spinDash = false;
    protected bool _edgeInfront = false;
    protected bool _edgeBehind = false;
    protected float _edgeDistance = 0f;
    protected int _idleStateCounter = 0;

    // Player Options
    protected string _PlayerCharacter;
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

	void Start()
	{
		// Get layer masks by name rather than Int
		lmGround = LayerMask.NameToLayer ("Ground");
        lmRoof = LayerMask.NameToLayer("Roof");
        lmWalls = LayerMask.NameToLayer("Walls");

        // Find the starting position of the character on the level
        transform.position = GameObject.Find("StartPos").transform.position;

        _spRenderer = GetComponent<SpriteRenderer>();
        //_inWater = true;
        SonicAssembler.CalcAngle(0.03f, 0.23f);
	}

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _subAnimator = GameObject.Find("DustAnimations").GetComponent<Animator>();
        CharacterAwake();
    }

    /// <summary>
    /// Update Every Single Frame
    /// </summary>
    void Update()
    {
        // Update Game Time and counters
        _gameTime = (int)Time.timeSinceLevelLoad;

        /// Seems the original code did the checks in the following order
        /// CheckSpindash
        /// Jump
        //  SlopeResist
        //  Move
        //	Roll
        //  LevelBound - Checks the Boundaries of the Level
        //  Update Player Position
        //  AnglePos
        //  SlopeRepel

        Collision();
        PlayerJump();
        PlayerMove();
        CheckWater();
        ApplyGravity();

        
        UpdateAnimations();

        transform.eulerAngles = new Vector2(0, YRotation);
        transform.Translate(velocity.x, velocity.y, 0f);

    }

	void OnGUI()
	{
        //GUILayout.Label ("Angle: " + _angle);

	}
    
    /// <summary>
    /// Check For Jumps
    /// </summary>
    void PlayerJump()
    {
        /// When you release the jump button in the air after jumping, the computer checks to see if Sonic is moving upward (i.e. Y speed is negative). If he is, then it checks to see if Y speed is less than -4 (e.g. -5 is "less" than -4). If it is, then Y speed is set to -4. In this way, you can cut your jump short at any time, just by releasing the jump button. If you release the button in the very next step after jumping, Sonic makes the shortest possible jump.

        if (Input.GetButtonDown("Jump") && _grounded)
        {
            _jumping = true;
            velocity = new Vector2(velocity.x, MAX_JUMP_FORCE);
            _audioSource.PlayOneShot(SndJump);
        }

        if (Input.GetButtonUp("Jump") && !_grounded)
        {
            if (velocity.y > MIN_JUMP_FORCE)
                velocity = new Vector2(velocity.x, MIN_JUMP_FORCE);
        }
    }

	void PlayerMove()
	{

        
        if (Input.GetAxis("Horizontal") < 0)
        {
            // Player_MoveLeft      
            if (_currentSpeed > 0f)
            {
                _currentSpeed = _currentSpeed - DECELERATION;

                // Play Braking Sound
                if (YRotation == FACING_RIGHT && Mathf.Abs(_currentSpeed) > 4.5f && _jumping == false) 
                { _audioSource.PlayOneShot(SndBrake); }

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
        else if (Input.GetAxis("Horizontal") > 0)
        {
            // Player_MoveRight
			if (_currentSpeed < 0f) 
            {
                _currentSpeed = _currentSpeed + DECELERATION;

                // Play braking Sound
                if (YRotation == FACING_LEFT && Mathf.Abs(_currentSpeed) > 4.5f && _jumping == false) 
                { _audioSource.PlayOneShot(SndBrake); }

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

		velocity = new Vector2 (Mathf.Abs(_currentSpeed), velocity.y);		
		
	}

	void UpdateAnimations()
	{
		UpdateCharacterAnimation();
	}

    void ApplyGravity()
    {
        // Not grounded apply gravity
        if (!_grounded)
        {
            velocity = new Vector2(velocity.x, Mathf.Max(velocity.y - GRAVITY, -MAX_FALL_VELOCITY));
        }
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

		Vector3 vGaStart, vGbStart, vRStart, vLStart;

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
        vRStart = new Vector3(box.center.x, box.center.y, transform.position.z);
        Ray rR = new Ray(vRStart, Vector3.right);
        bool bRConnected = false;
        Debug.DrawRay(vRStart, Vector3.right * 10f, Color.cyan, 2f);
        RaycastHit hR;
        bRConnected = Physics.Raycast(rR, out hR, 10f, 1 << lmWalls);

        vLStart = new Vector3(box.center.x, box.center.y, transform.position.z);
        Ray rL = new Ray(vLStart, -Vector3.right);
        bool bLConnected = false;
        Debug.DrawRay(vLStart, -Vector3.right * 10f, Color.cyan, 2f);
        RaycastHit hL;
        bLConnected = Physics.Raycast(rL, out hL, 10f, 1 << lmWalls);
        #endregion

        #region Top Collision
        Ray rTa = new Ray(vGaStart, Vector3.up);
        bool bTaConnected = false;
        Debug.DrawRay(vGaStart, Vector3.up * 16f, Color.cyan, 2f);
        RaycastHit hTa;
        bTaConnected = Physics.Raycast(rTa, out hTa, 16f, 1 << lmWalls);

        Ray rTb = new Ray(vGbStart, Vector3.up);
        bool bTbConnected = false;
        Debug.DrawRay(vGbStart, Vector3.up * 16f, Color.cyan, 2f);
        RaycastHit hTb;
        bTbConnected = Physics.Raycast(rTb, out hTb, 16f, 1 << lmWalls);
        #endregion
    }

    void CheckWater()
    {
        if (_inWater)   // Are we underwater? If so then set appropriate values
        {
            GRAVITY = 0.0625f;
            MAX_FALL_VELOCITY = 16f;
            MAX_JUMP_FORCE = 2f;
            MIN_JUMP_FORCE = 3.5f;
            ACCELERATION = 0.0234375f;
            FRICTION = 0.0234375f;
            DECELERATION = 0.25f;
            TOP_SPEED = 6f;
        }
        else
        {
            GRAVITY = 0.21875f;
            MAX_FALL_VELOCITY = 16f;
            MAX_JUMP_FORCE = 6.5f;
            MIN_JUMP_FORCE = 4f;
            ACCELERATION = 0.046875f;
            FRICTION = 0.046875f;
            DECELERATION = 0.5f;
            TOP_SPEED = 6f;
        }
    }

    private void MoveLeft()
    {

    }
}