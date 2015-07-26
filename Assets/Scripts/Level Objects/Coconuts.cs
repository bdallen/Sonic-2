using UnityEngine;
using System.Collections;

public class Coconuts : Badnik {

	// Use this for initialization
	public void Awake () {
		base.Awake ();
		_currentSpeed = -5.0f;
		_animator.Play("Coconuts");
	}
	
	// Update is called once per frame
	public virtual void Update () {
		base.Update ();	
		if (_eliminated) {
			if (_destroyDelay == 500) {
				base.EliminateBadnik ();
			}
			if (_destroyDelay == 1) {
				_animator.Play ("Explosion");
			}
			_destroyDelay += 1;
		} 
		else {

			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + _currentSpeed, this.transform.position.z);  	//Move Coconuts in appropriate direction
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
}
