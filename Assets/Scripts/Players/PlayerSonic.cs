using UnityEngine;
using System.Collections;

public class PlayerSonic : BasePlayerMovement
{

    #region Constants
    public const string PLAYER_ID = "SONIC";
    #endregion

    #region Private Variables
    private object _sonicSprites;
    private bool _superSonic = false;
    #endregion

    public bool IsSuperSonic
    { get { return _superSonic; } set { _superSonic = value; } }

    public override void CharacterAwake()
    {
        _sonicSprites = Resources.Load("Characters/Sonic/drown01", typeof(Sprite));
    }

    public override void UpdateCharacterAnimation()
    {

    }

}

