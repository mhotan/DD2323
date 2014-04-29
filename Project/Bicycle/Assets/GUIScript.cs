using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {

	void OnGUI () {
		if (GUI.Button (new Rect (10,10,100,50), "Light")) {
			print ("You clicked the button!");
			light.enabled = !light.enabled;

		}
	}
}
