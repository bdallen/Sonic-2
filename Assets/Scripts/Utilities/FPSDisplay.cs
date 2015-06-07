using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    float udeltaTime = 0.0f;
    float pdeltaTime = 0.0f;

    void FixedUpdate()
    {
        pdeltaTime += (Time.deltaTime - pdeltaTime) * 0.1f;
    }


    void Update()
    {
        udeltaTime += (Time.deltaTime - udeltaTime) * 0.1f;
    }

    void OnGUI()
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