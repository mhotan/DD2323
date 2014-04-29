using UnityEngine;
using System.Collections;

public class GenerateBike : MonoBehaviour {

	public Transform BikeLeft;
	public Transform BikeRight;
	

	private float nextSpawn = 1;
	private float hSliderValue = 10.0f;

	// Use this for initialization
	void Start () {
		//InvokeRepeating("CreateBike", 1, 1);
	
	}

	void OnGUI () {
		hSliderValue = GUI.HorizontalSlider (new Rect (125, 25, 100, 30), hSliderValue, 10.0f, 50.0f);
		
	}

	void CreateBikeRightLane()
	{
		Instantiate (BikeRight);
	}

	void CreateBikeLeftLane()
	{
		Instantiate(BikeLeft);
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate()
	{
		if(Time.time >= nextSpawn)
		{
			BikeRight.GetComponent<NavMeshAgent>().speed = Random.Range (8, 20);
			CreateBikeRightLane();

			BikeLeft.GetComponent<NavMeshAgent>().speed = Random.Range (8, 20);
			CreateBikeLeftLane();

			nextSpawn  = Time.time + Random.Range(2,5);
		}

		//BikeScript.SetSpeed(hSliderValue);

	}
}
