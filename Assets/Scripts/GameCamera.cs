using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;

    public int TARGET_FRAMERATE = 50;

    // Init
    void Start()
    {
        // Lock the framerate to 50 FPS for engine compatability with Megadrive
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FRAMERATE;
    }

	// Update is called once per frame
	void LateUpdate () {
		if (target)
		{
			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
			Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
	}
}
