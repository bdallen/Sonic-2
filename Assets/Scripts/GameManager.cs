using UnityEngine;
using System.Collections;

public class GameManager: MonoBehaviour {

    public bool showFPS = false;
	public float dampTime = 0.15f;
	public Transform target;
    public Camera camera;
    public Transform leftBound;
    public Transform rightBound;

    public int TARGET_FRAMERATE = 50;

    private float udeltaTime = 0.0f;
    private float pdeltaTime = 0.0f;
    private float _camHeight = 0;
    private float _camWidth = 0;
    private Vector3 velocity = Vector3.zero;

    // Init
    void Start()
    {
        // Lock the framerate to 50 FPS for engine compatability with Megadrive
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FRAMERATE;

        // Get the width and height of the camera
        _camHeight = 2f * camera.orthographicSize;
        _camWidth = _camHeight * camera.aspect;

    }

    void FixedUpdate()
    {
        pdeltaTime += (Time.deltaTime - pdeltaTime) * 0.1f;
    }
    void Update()
    {
        udeltaTime += (Time.deltaTime - udeltaTime) * 0.1f;
    }

	// Late Update is called once per frame Last!
	void LateUpdate () {

        // Camera to Follow Character
        CameraFollow();

	}

    void OnGUI()
    {
        if (showFPS) { FPSDisplay(); }      // If Show FPS is enabled
    }

    /// <summary>
    /// Camera following routine - Follow the player
    /// </summary>
    void CameraFollow()
    {
        if (target)
        {
            Vector3 point = camera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = camera.transform.position + delta;

            // Camera hits the left bound
            if (camera.transform.position.x - (_camWidth / 2) <= leftBound.position.x && target.position.x <= camera.transform.position.x)
            {
                point = camera.WorldToViewportPoint(new Vector3(camera.transform.position.x, target.position.y, target.position.z));
                delta = new Vector3(leftBound.position.x + (_camWidth / 2), target.position.y, target.position.z) - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
                destination = camera.transform.position + delta;
            }

            // Shift the Camera
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, destination, ref velocity, dampTime);
        }
    }

    void FPSDisplay()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.yellow;
        float msec = udeltaTime * 1000.0f;
        float fps = 1.0f / udeltaTime;
        float pmsec = pdeltaTime * 1000.0f;
        float pfps = 1.0f / pdeltaTime;
        string text = string.Format("GFX: {0:0.0} ms ({1:0.} fps)\n", msec, fps);
        text = text + string.Format("PHY: {0:0.0} ms ({1:0.} fps)", pmsec, pfps);
        GUI.Label(rect, text, style);
    }
}
