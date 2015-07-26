using UnityEngine;
using System.Collections;

public class Masher : Badnik {

	// Use this for initialization
	public void Awake () {
		base.Awake ();
		_currentSpeed = 5.0f;
		_animator.Play("Masher");
	}
	
	// Update is called once per frame
	public virtual void Update () {
		base.Update ();	
		if (_eliminated) {
			if (_destroyDelay == 500) {
				base.EliminateBadnik ();
			}
			if (_destroyDelay == 1)	{
				_animator.Play ("Explosion");
			}
			_destroyDelay += 1;
		} 
		else {
		
			if (_currentSpeed < 0.0f) { 															// Masher is falling
				if ((this.transform.position.y + _currentSpeed) < _startlocation.y) { 			//Current velocity will take Masher out of bounds
					_currentSpeed = 5.0f; 													//Reset speed to bounce Masher upward again
					this.transform.position = new Vector3 (this.transform.position.x, _startlocation.y + _currentSpeed, this.transform.position.z); 			//Move Masher upwards
				} else { 																		//Masher can still move downwards within bounds
					this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + _currentSpeed, this.transform.position.z);  	//Move Masher downwards
					_currentSpeed = _currentSpeed - 0.1f;
				}
			} else if (_currentSpeed > 0.0f) {														//Masher is rising
				if ((this.transform.position.y + _currentSpeed) > (_startlocation.y + 128)) {		//Current velocity will take Masher out of bounds
					_currentSpeed = -0.1f;														//Reset speed to drop Masher downwards again
					this.transform.position = new Vector3 (this.transform.position.x, _startlocation.y + 128 + _currentSpeed, this.transform.position.z);		//Move masher downwards
				} else {																		//Masher can still move up without breaking bounds
					this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + _currentSpeed, this.transform.position.z);	//Move Masher upwards
					_currentSpeed = _currentSpeed - 0.1f;	
				}
			}
		}
	}
}
