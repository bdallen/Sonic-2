using UnityEngine;
using System.Collections;

public class Buzzer : Badnik 	// Update is called once per frame
{
	
	protected float _currentSpeed = 3.0f;
	
	public override void Update()
	{
		if (_eliminated)
		{
			_destroyDelay += 1;
		}
		if (_destroyDelay == 25)
		{
			Destroy(this.gameObject);
		}
		
		if (_currentSpeed < 0.0f) 															// Buzzer is moving left
		{
			if((this.transform.position.x + _currentSpeed) < _startlocation.x){ 			//Current velocity will take Buzzer out of bounds
				_currentSpeed = 3.0f; 														//Reset speed to move Buzzer right again
				this.transform.position = new Vector3 (_startlocation.x + _currentSpeed, this.transform.position.y, this.transform.position.z); 			//Move Masher upwards
			}
			else { 																			//Buzzer can still move left within bounds
				this.transform.position = new Vector3 (this.transform.position.x + _currentSpeed, this.transform.position.y, this.transform.position.z);  	//Move Masher downwards
			}
		}
		else if (_currentSpeed > 0.0f)														//Buzzer is moving right
		{
			if((this.transform.position.x + _currentSpeed) > (_startlocation.x + 256)){		//Current velocity will take Buzzer out of bounds
				_currentSpeed = -3.0f;														//Reset speed to move Buzzer left again
				this.transform.position = new Vector3 (_startlocation.x + _currentSpeed + 256, this.transform.position.y, this.transform.position.z);		//Move masher downwards
			}
			else {																			//Buzzer can still move right within bounds
				this.transform.position = new Vector3 (this.transform.position.x + _currentSpeed, this.transform.position.y, this.transform.position.z);	//Move Masher upwards
			}
		}
	}
}


