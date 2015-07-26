using UnityEngine;
using System.Collections;

public class Badnik : MonoBehaviour {
	
	protected Animator _animator;
	public bool _eliminated = false;
	protected int _destroyDelay = 0;
	protected int _currentFrame = 0;
	public Vector3 _startlocation = new Vector3(0,0,0);
	public float _currentSpeed;

	// Use this for initialization
	public void Awake () {
		_animator = GetComponent<Animator>();
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
