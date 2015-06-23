using UnityEngine;
using System.Collections;

public class Badnik : MonoBehaviour {
	
	protected Animator _animator;
	public bool _eliminated = false;
	public string _badnikType;
	protected int _destroyDelay = 0;
	protected int _currentFrame = 0;
	protected Vector3 _startlocation = new Vector3(0,0,0);
	protected float _currentSpeed;
	// Use this for initialization
	public void Awake () {
		_startlocation = this.transform.position;
		switch (_badnikType) {
		case "Masher":
			_currentSpeed = 5.0f;
		break;
		case "Buzzer":
			_currentSpeed = 1.0f;
		break;
		case "Coconuts":
			_currentSpeed = -5.0f;
		break;
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (_eliminated)
		{
			_destroyDelay += 1;
		}
		if (_destroyDelay == 25)
		{
			Destroy(this.gameObject);
		}
		switch (_badnikType) {
		case "Masher":
			UpdateMasher();
			break;
		case "Buzzer":
			UpdateBuzzer();
			break;
		case "Coconuts":
			UpdateCoconuts();
			break;
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


	private void UpdateMasher(){
		if (_currentSpeed < 0.0f) 															// Masher is falling
		{
			if((this.transform.position.y + _currentSpeed) < _startlocation.y){ 			//Current velocity will take Masher out of bounds
				_currentSpeed = 5.0f; 													//Reset speed to bounce Masher upward again
				this.transform.position = new Vector3 (this.transform.position.x, _startlocation.y + _currentSpeed, this.transform.position.z); 			//Move Masher upwards
			}
			else { 																		//Masher can still move downwards within bounds
				this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + _currentSpeed,this.transform.position.z);  	//Move Masher downwards
				_currentSpeed = _currentSpeed - 0.1f;
			}
		}
		else if (_currentSpeed > 0.0f)														//Masher is rising
		{
			if((this.transform.position.y + _currentSpeed) > (_startlocation.y + 128)){		//Current velocity will take Masher out of bounds
				_currentSpeed = -0.1f;														//Reset speed to drop Masher downwards again
				this.transform.position = new Vector3 (this.transform.position.x, _startlocation.y + 128 + _currentSpeed, this.transform.position.z);		//Move masher downwards
			}
			else {																		//Masher can still move up without breaking bounds
				this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + _currentSpeed, this.transform.position.z);	//Move Masher upwards
				_currentSpeed = _currentSpeed - 0.1f;	
			}
		}
	}
	
	private void UpdateBuzzer() {
		if (_currentSpeed < 0.0f) 															// Buzzer is moving left
		{
			this.transform.eulerAngles = new Vector3(0,0,0);
			if((this.transform.position.x + _currentSpeed) < _startlocation.x){ 			//Current velocity will take Buzzer out of bounds
				_currentSpeed = 1.0f; 														//Reset speed to move Buzzer right again
				this.transform.position = new Vector3 (_startlocation.x + _currentSpeed, this.transform.position.y, this.transform.position.z); 			//Move Masher upwards
			}
			else { 																			//Buzzer can still move left within bounds
				this.transform.position = new Vector3 (this.transform.position.x + _currentSpeed, this.transform.position.y, this.transform.position.z);  	//Move Masher downwards
			}
		}
		else if (_currentSpeed > 0.0f)														//Buzzer is moving right
		{
			this.transform.eulerAngles = new Vector3(0,180,0);
			if((this.transform.position.x + _currentSpeed) > (_startlocation.x + 256)){		//Current velocity will take Buzzer out of bounds
				_currentSpeed = -1.0f;														//Reset speed to move Buzzer left again
				this.transform.position = new Vector3 (_startlocation.x + _currentSpeed + 256, this.transform.position.y, this.transform.position.z);		//Move masher downwards
			}
			else {																			//Buzzer can still move right within bounds
				this.transform.position = new Vector3 (this.transform.position.x + _currentSpeed, this.transform.position.y, this.transform.position.z);	//Move Masher upwards
			}
		}

	}

	private void UpdateCoconuts() {
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + _currentSpeed,this.transform.position.z);  	//Move Coconuts in appropriate direction
		//3*8 24 1.5
		//2*8 16 1.0
		//4*8 32 2.0
		//3*8 24 1.5
		//2*8 16 1.0
		//4*8 32 2.0

		switch (_currentFrame) {
		case 0:
			//if you have some poop, throw it now
			_currentSpeed = -0.75f;
			break;
		case 30:
			_currentSpeed = 0.0f;
			break;
		case 40:
			_currentSpeed = 0.5f;
			break;
		case 70:
			_currentSpeed = 0.0f;
			break;
		case 80:
			//if you have some poop, throw it now
			_currentSpeed = -1.0f;
			break;
		case 110:
			_currentSpeed = 0.0f;
			break;
		case 120:
			_currentSpeed = 0.75f;
			break;
		case 150:
			_currentSpeed = 0.0f;
			break;
		case 160:
			//if you have some poop, throw it now
			_currentSpeed = -0.5f;
			break;
		case 190:
			_currentSpeed = 0.0f;
			break;
		case 200:
			_currentSpeed = 1.0f;
			break;
		case 230:
			_currentSpeed = 0.0f;
			break;
		case 240:
			_currentFrame = -1; //Reset so increment will set back to frame 0
			break;
		}

		_currentFrame ++;


	}


}
