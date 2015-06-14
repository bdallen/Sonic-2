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

    #endregion

    #region Private Variables
    private bool _superSonic = false;
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
        if (Mathf.Abs(_currentSpeed) > 0.01f && !_jumping)
        {
            if (Mathf.Abs(_currentSpeed) > 4.5f)
            {
                _animator.Play(SonAni_Run);
                _animator.speed = 10f;
            }
            else
            {
                _animator.Play(SonAni_Walk);
                _animator.speed =Mathf.Abs(_currentSpeed) / 1.8f;
            }
        }

        // Balancing on Forward Ledge
        else if (!_jumping && _edgeInfront  && _edgeDistance < 6f && _edgeDistance > 4f && _currentSpeed == 0f)
        {
            _animator.Play(SonAni_Balance);
            _animator.speed = 2f;
        }

        // Balancing on Baward Ledge
        else if (!_jumping && _edgeBehind && _edgeDistance < 6f && _currentSpeed == 0f)
        {
            _animator.Play(SonAni_Balance2);
            _animator.speed = 2f;
        }

        // Balancing on Forward Ledge (About to Fall Off)
        else if (!_jumping && _edgeInfront && _edgeDistance < 4f && _currentSpeed == 0f)
        {
            _animator.Play(SonAni_Balance3);
            _animator.speed = 6f;
        }
        
        // Jumping
        else if (_jumping)
        {
            _animator.Play(SonAni_Jump);
            _animator.speed = 1f;
        }

        // Default State
        else
        {
            _animator.Play(SonAni_Stand);
            _animator.speed = 1f;
        }
        
    }

}

