using UnityEngine;
using System.Collections;

public class Badnik : MonoBehaviour {
	
	protected Animator _animator;
	public bool _eliminated = false;
	protected int _destroyDelay = 0;
	
	// Use this for initialization
	void Awake () {
		
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
