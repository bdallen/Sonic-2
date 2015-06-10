using UnityEngine;
using System.Collections;

public class PlayerSonic : BasePlayerMovement
{

    #region Constants
    public const string PLAYER_ID = "SONIC";
    #endregion

    #region Private Variables
    private bool _superSonic = false;
    #endregion

    public bool IsSuperSonic
    { get { return _superSonic; } set { _superSonic = value; } }

    public PlayerSonic()
    {

    }

    public override void UpdateCharacterAnimation()
    {

    }

}

