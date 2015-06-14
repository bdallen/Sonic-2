using UnityEngine;
using System.Collections;

public class PlayerSonic : BasePlayerMovement
{

    #region Constants
    private const string PLAYER_ID = "SONIC";
    #endregion

    #region Private Variables
    private bool _superSonic = false;

    // Animations
    private AnimationClip _sonicRun;
    #endregion

    public PlayerSonic()
    {
        _PlayerCharacter = PLAYER_ID;
     
    }

    public bool IsSuperSonic
    { get { return _superSonic; } set { _superSonic = value; } }

    public override void CharacterAwake()
    {
        // Instantiate all Animation Clips
        _sonicRun = Resources.Load<AnimationClip>("Characters/Sonic/SonicRun");
    }

    public override void UpdateCharacterAnimation()
    {

    }

}

