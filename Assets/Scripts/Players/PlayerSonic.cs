using UnityEngine;
using System.Collections;

public class PlayerSonic : BasePlayerMovement
{

    #region Constants
    private const string PLAYER_ID = "SONIC";
    #endregion

    #region Private Variables
    private Object[] _sonicSprites;
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
        _sonicSprites = Resources.LoadAll("Characters/Sonic/Sprites", typeof(Sprite));
    }

    public override void UpdateCharacterAnimation()
    {

    }

}

