using UnityEngine;
using System.Collections;

public class Badnik : MonoBehaviour {
	
	protected Animator _animator;
	public bool _eliminated = false;
	protected int _destroyDelay = 0;
	protected Vector3 _startlocation = new Vector3(0,0,0);
	
	// Use this for initialization
	public void Awake () {
		_startlocation = this.transform.position;
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
	}

	public void EliminateBadnik()
	{
		_animator = GetComponent<Animator>();
		_animator.Play("Badnik_Puff"); // TODO: Create animation for animals being freed
		//TODO: instanciate a squirrel to run off screen
		//TODO: Append 100 to score
		//TODO: Create '100' test to show
		if (!_eliminated) { GetComponent<AudioSource>().Play(); } //TODO: Obtain sound clip
		_eliminated = true;
	}
	
	class Masher : Badnik
	{
		protected float _currentSpeed = 0f;
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

			if (_currentSpeed < 0) 															// Masher is falling
			{
				if(this.transform.position.y + _currentSpeed < _startlocation.y){ 			//Current velocity will take Masher out of bounds
					_currentSpeed = 10; 													//Reset speed to bounce Masher upward again
					this.transform.position.Set(this.transform.position.x, _startlocation.y + _currentSpeed, this.transform.position.z); 			//Move Masher upwards
				}
				else { 																		//Masher can still move downwards within bounds
					this.transform.position.Set(this.transform.position.x, this.transform.position.y + _currentSpeed,this.transform.position.z);  	//Move Masher downwards
				}
			}
			else if (_currentSpeed > 0)														//Masher is rising
			{
				if(this.transform.position.y + _currentSpeed < _startlocation.y + 128){		//Current velocity will take Masher out of bounds
					_currentSpeed = 0;														//Reset speed to drop Masher downwards again
					this.transform.position.Set(this.transform.position.x, _startlocation.y + 128 + _currentSpeed, this.transform.position.z);		//Move masher downwards
				}
				else {																		//Masher can still move up without breaking bounds
					this.transform.position.Set(this.transform.position.x, this.transform.position.y + _currentSpeed, this.transform.position.z);	//Move Masher upwards
				}
			}
		}
	}

	class Buzzer : Badnik
	{
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
		}
	}

	class Coconuts : Badnik
	{
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
		}
	}
	

}
