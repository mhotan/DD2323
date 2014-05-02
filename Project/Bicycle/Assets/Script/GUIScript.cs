using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {

	public Material skyboxDay;
	public Material skyboxNight;

	bool day = true;

	void OnGUI () {
		if (GUI.Button (new Rect (10,10,100,50), "Light")) {
			foreach(var light in GetComponentsInChildren<Light>())
				light.enabled = !light.enabled;

			if(day){
				RenderSettings.skybox = skyboxNight;
				day = false;
			}
			else{
				RenderSettings.skybox = skyboxDay;
				day = true;
			}
		}
	}
}
