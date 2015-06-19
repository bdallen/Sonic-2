using UnityEngine;
using System.Collections;

public class PlayerSonic : BasePlayerMovement
{

    #region Constants
    private const string PLAYER_ID = "SONIC";

    // Animation States
    private const string SonAni_Stand = "SonAni_Stand";
    private const string SonAni_Walk = "SonAni_Walk";
    private const string SonAni_Jump = "SonAni_Jump";
    private const string SonAni_Run = "SonAni_Run";
    private const string SonAni_Balance = "SonAni_Balance";
    private const string SonAni_Balance2 = "SonAni_Balance2";
    private const string SonAni_Balance3 = "SonAni_Balance3";
    private const string SonAni_Blink = "SonAni_Blink";
    private const string SonAni_Wait1 = "SonAni_Wait1";
    private const string SonAni_Wait2 = "SonAni_Wait2";
    private const string SonAni_Wait3 = "SonAni_Wait3";
    private const string SonAni_Wait4 = "SonAni_Wait4";
    private const string SonAni_Dead = "SonAni_Dead";
    #endregion

    #region Private Variables
    private bool _superSonic = false;
    private bool _idleState1 = false;
    private bool _idleState2 = false;
    private bool _idleState3 = false;
    private bool _idleState4 = false;
    private bool _idleWatchFrame = false;
    private int _idleTapCounter = 0;
    #endregion

    public PlayerSonic()
    {
        _PlayerCharacter = PLAYER_ID;
     
    }

    public bool IsSuperSonic
    { get { return _superSonic; } set { _superSonic = value; } }

    public override void CharacterAwake()
    {

    }

    /// <summary>
    /// Updates the Character Animations, Depending on State of the Character
    /// </summary>
    public override void UpdateCharacterAnimation()
    {
        // Walking to Running Transition State
        if (Mathf.Abs(_currentSpeed) > 0.01f && !_jumping && !_dead)
        {
            if (Mathf.Abs(_currentSpeed) > 4.5f)
            {
                _animator.Play(SonAni_Run);
                _animator.speed = 10f;
                ResetIdleState();
            }
            else
            {
                _animator.Play(SonAni_Walk);
                _animator.speed =Mathf.Abs(_currentSpeed) / 1.8f;
                ResetIdleState();
            }
        }

        // Balancing on Forward Ledge
        else if (!_jumping && _edgeInfront  && _edgeDistance < 6f && _edgeDistance > 4f && _currentSpeed == 0f)
        {
            _animator.Play(SonAni_Balance);
            _animator.speed = 2f;
            ResetIdleState();
        }

        // Balancing on Baward Ledge
        else if (!_jumping && _edgeBehind && _edgeDistance < 6f && _currentSpeed == 0f)
        {
            _animator.Play(SonAni_Balance2);
            _animator.speed = 2f;
            ResetIdleState();
        }

        // Balancing on Forward Ledge (About to Fall Off)
        else if (!_jumping && _edgeInfront && _edgeDistance < 4f && _currentSpeed == 0f)
        {
            _animator.Play(SonAni_Balance3);
            _animator.speed = 6f;
            ResetIdleState();
        }
        
        // Jumping
        else if (_jumping)
        {
            _animator.Play(SonAni_Jump);
            _animator.speed = 1f;
            ResetIdleState();
        }

        // Performing a Spin Dash
        else if (_spinDash)
        {
            // Sub Animator that controlls the dust
            _subAnimator.Play("SonicSpinDashDust");
            _subAnimator.speed = 7f;
            ResetIdleState();
        }

        else if (_dead)
        {
            _animator.Play(SonAni_Dead);
        }

        // Default State
        else
        {
            
            _idleStateCounter += 1;

            // Begin Idle State and Blink
            if (_idleStateCounter == 180 && !_idleState1 && !_idleState2 && !_idleState3 && !_idleState4)
            {
                _idleState1 = true;
                _animator.Play(SonAni_Blink);
                _idleStateCounter = 0;
            }

            // Resume tapping after 60 Frames of looking at watch
            else if (_idleWatchFrame && _idleStateCounter == 60 && !_idleState3 && !_idleState4)
            {
                _animator.Play(SonAni_Wait1);
                _idleWatchFrame = false;
                _idleStateCounter = 0;
                _idleTapCounter += 1;
            }

            // Start tapping the foot after Blinking
            else if (_idleState1 && _idleStateCounter == 6 && !_idleState3 && !_idleState4)
            {
                _animator.Play(SonAni_Wait1);
                _animator.speed = 1.5f;
                _idleStateCounter = 0;
                _idleState2 = true;
                _idleState1 = false;
            }

            // Look at watch after 144 Frames
            else if (_idleState2 && _idleStateCounter == 144 && !_idleState3 && !_idleState4)
            {
                _animator.Play(SonAni_Wait2);
                _idleStateCounter = 0;
                _idleWatchFrame = true;
            }

            else if (_idleTapCounter == 4 && !_idleState3)
            {
                _idleState3 = true;
            }

            // Lay Sonic Down
            else if (_idleState3 && !_idleState4)
            {
                _animator.Play(SonAni_Wait3);
                _animator.speed = 2f;
                _idleStateCounter = 0;
                _idleState4 = true;
            }

            // Continue Laying Down
            else if (_idleState4 && _idleStateCounter == 6)
            {
                _animator.Play(SonAni_Wait4);
                _animator.speed = 2f;
            }

            // Standard Standing Position
            else if (!_idleState1 && !_idleState2 && !_idleState3 && !_idleState4)
            {
                _animator.Play(SonAni_Stand);
                _animator.speed = 1f;
            }
        }
        
    }

    void ResetIdleState()
    {
        _idleState1 = false;
        _idleState2 = false;
        _idleState3 = false;
        _idleState4 = false;
        _idleStateCounter = 0;
        _idleTapCounter = 0;
        _idleWatchFrame = false;
    }

}

