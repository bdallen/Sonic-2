using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{

    #region Private
    BasePlayerMovement _bpm;
    PlayerSonic _pSonic;
    TextMesh _livesText;
    TextMesh _ringsText;
    TextMesh _secondsText;
    TextMesh _minutesText;
    #endregion

    // Use this for initialization
	void Start () {
        _bpm = GameObject.Find("Player").GetComponent<BasePlayerMovement>();
        _livesText = GameObject.Find("LivesText").GetComponent<TextMesh>();
        _ringsText = GameObject.Find("Rings").GetComponent<TextMesh>();
        _secondsText = GameObject.Find("TimeSeconds").GetComponent<TextMesh>();
        _minutesText = GameObject.Find("TimeMinutes").GetComponent<TextMesh>();

        // Check the caracter and load up the required Class
        switch(_bpm.PlayerCharacter)
        {
            case "SONIC":
                {
                    _pSonic = GameObject.Find("Player").GetComponent<PlayerSonic>();
                    break;
                }
        }
	}
	
	// Update is called once per frame
	void Update () {
        // Update Lives Text
        _livesText.text = _bpm.LIVES.ToString();

        _ringsText.text = _bpm.RINGS.ToString();

        
        // Update Score

        // Update Game Time
        UpdateGameTime();

        // Update HUD Animations
        UpdateAnimations();
    }

    /// <summary>
    /// Updates the Game Time on the HUD
    /// </summary>
    void UpdateGameTime()
    {
       
        // Run a Modulus and Remainder on the Minutes and Seconds as Playtime is in Total Seconds
        int minutes = _bpm.PlayTime / 60;
        int seconds = _bpm.PlayTime % 60;

        // Update the Text Objects
        _secondsText.text = seconds.ToString("D2");
        _minutesText.text = minutes.ToString();
    }

    void UpdateAnimations()
    {
        Animator anim = GetComponentInChildren<Animator>();

        // If we have rings or not
        if (_bpm.RINGS > 0) { anim.SetBool("HasRings", true); } else { anim.SetBool("HasRings", false); }
        
        // Check the caracter and change variables as required
        switch (_bpm.PlayerCharacter)
        {
            case "SONIC":
                {
                    //_pSonic.IsSuperSonic
                    break;
                }
        }
    }
}
