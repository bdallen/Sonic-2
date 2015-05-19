using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds (3f);
		float fadeTime = GameObject.Find ("_GM").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		var IntroText = GameObject.Find ("IntroText");
		DestroyObject (IntroText);

		// For now just go to EHZ
		Application.LoadLevel ("Emerald Hill Zone");
	}
	
	// Update is called once per frame
	void Update () {

	}
}
