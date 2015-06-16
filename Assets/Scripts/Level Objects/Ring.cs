using UnityEngine;
using System.Collections;

public class Ring : MonoBehaviour {

    protected Animator _animator;
    public bool _collected = false;
    protected int _destroyDelay = 0;

	// Use this for initialization
	void Awake () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (_collected)
        {
            _destroyDelay += 1;
        }
        if (_destroyDelay == 25)
        {
            Destroy(this.gameObject);
        }
	}

    public void CollectRing()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Ring_Hit");
        if (!_collected) { GetComponent<AudioSource>().Play(); }
        _collected = true;
    }
}
