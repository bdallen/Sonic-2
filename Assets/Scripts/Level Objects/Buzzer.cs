using UnityEngine;
using System.Collections;

public class Buzzer : Badnik {
	
	// Use this for initialization
	public void Awake () {
		base.Awake ();
		_currentSpeed = 1.0f;
		_animator.Play("BuzzerFly");
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

			if (_currentSpeed < 0.0f) { 															// Buzzer is moving left
				this.transform.eulerAngles = new Vector3 (0, 0, 0);
				if ((this.transform.position.x + _currentSpeed) < _startlocation.x) { 			//Current velocity will take Buzzer out of bounds
					_currentSpeed = 1.0f; 														//Reset speed to move Buzzer right again
					this.transform.position = new Vector3 (_startlocation.x + _currentSpeed, this.transform.position.y, this.transform.position.z); 			//Move Masher upwards
				} else { 																			//Buzzer can still move left within bounds
					this.transform.position = new Vector3 (this.transform.position.x + _currentSpeed, this.transform.position.y, this.transform.position.z);  	//Move Masher downwards
				}
			} else if (_currentSpeed > 0.0f) {														//Buzzer is moving right
				this.transform.eulerAngles = new Vector3 (0, 180, 0);
				if ((this.transform.position.x + _currentSpeed) > (_startlocation.x + 256)) {		//Current velocity will take Buzzer out of bounds
					_currentSpeed = -1.0f;														//Reset speed to move Buzzer left again
					this.transform.position = new Vector3 (_startlocation.x + _currentSpeed + 256, this.transform.position.y, this.transform.position.z);		//Move masher downwards
				} else {																			//Buzzer can still move right within bounds
					this.transform.position = new Vector3 (this.transform.position.x + _currentSpeed, this.transform.position.y, this.transform.position.z);	//Move Masher upwards
				}
			}
		}
	}

	public void EliminateBadnik()
	{
		//_animator = GetComponent<Animator>();
		//_animator.Play("Badnik_Puff"); // TODO: Create animation for animals being freed
		//TODO: instanciate a squirrel to run off screen
		//TODO: Create '100' test to show
		//if (!_eliminated) { 
			this.GetComponent<AudioSource>().Play(); 
		//}
		_eliminated = true;

	}	
}
