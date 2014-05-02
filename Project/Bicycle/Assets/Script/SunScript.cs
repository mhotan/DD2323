using UnityEngine;
using System.Collections;

public class SunScript : MonoBehaviour {

	//public int DayLenght = 24;
	public float DayLenght = 24;


	void OnGUI () {
		DayLenght = GUI.HorizontalSlider (new Rect (250, 25, 100, 30), DayLenght, 5.0f, 50.0f);		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


	
	}

	void FixedUpdate(){
		float degreeDelta = 360.0f/DayLenght;
		gameObject.transform.Rotate(new Vector3(0,0, degreeDelta*Time.fixedDeltaTime));

		var rotation =  gameObject.transform.rotation.eulerAngles.z % 360;

		//we need to dim the sun at night because of the way directionnal light work
		if(rotation > 90 && rotation < 270)
		{
			rotation = Mathf.Abs(rotation -180);
			float intensity = rotation/90 * 0.3f;
			GetComponentInChildren<Light>().intensity = intensity;
		}
	}
}
