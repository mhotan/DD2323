using UnityEngine;
using System.Collections;

public class GenerateBike : MonoBehaviour {

	public Transform BikeLeft;
	public Transform BikeRight;
	

	private float nextSpawn = 1;
	private float densitySlider = 5.0f;
	private float averageSpeedSlider = 20.0f;
	private string densityLabel = "";

	private readonly float MAX_SPEED = 30.0f;
	private readonly float MIN_SPEED = 8.0f;

	// Use this for initialization
	void Start () {
		InvokeRepeating("ComputeDensity", 1, 0.5f);
	
	}

	void OnGUI () {
		GUI.Box(new Rect(125, 10, 100, 60), "Traffic Density");
		densitySlider = GUI.HorizontalSlider (new Rect (130, 35, 90, 30), densitySlider, 10.0f, 2.0f);
		GUI.Label(new Rect(130, 45, 90, 30), densityLabel);

		GUI.Box(new Rect(235, 10, 100, 50), "Average Speed");
		averageSpeedSlider = GUI.HorizontalSlider (new Rect (240, 35, 90, 30), averageSpeedSlider, MIN_SPEED, MAX_SPEED);
		
	}

	/**
	 * Instantiate a Bike in the right lane (top lane)
	 * the speed is a random number between 8 and 20 for now
	 * the avoidance priority is the same as the speed, so faster bikes
	 * are the one avoiding slower bikes
	 * The starting position change in function with the speed so faster bikes 
	 * spawn closer to the center of the road than slower bike, make the avoidance 
	 * process more fluid
	 */
	void CreateBikeRightLane()
	{
		float speed = GetSpeedWithMean(averageSpeedSlider);
		BikeRight.GetComponent<NavMeshAgent>().speed = speed;
		BikeRight.GetComponent<NavMeshAgent>().avoidancePriority = Mathf.RoundToInt(speed);
		Vector3 pos = BikeRight.position;
		pos.z -= (speed/MAX_SPEED) * 7;
		Instantiate (BikeRight, pos, BikeRight.rotation);
	}

	/**
	 * Instantiate a Bike in the left lane (bottom lane)
	 * the speed is a random number between 8 and 20 for now
	 * the avoidance priority is the same as the speed, so faster bikes
	 * are the one avoiding slower bikes
	 * The starting position change in function with the speed so faster bikes 
	 * spawn closer to the center of the road than slower bike, make the avoidance 
	 * process more fluid
	 */
	void CreateBikeLeftLane()
	{
		float speed = GetSpeedWithMean(averageSpeedSlider);
		BikeLeft.GetComponent<NavMeshAgent>().speed = speed;
		BikeLeft.GetComponent<NavMeshAgent>().avoidancePriority = Mathf.RoundToInt(speed);
		Vector3 pos = BikeLeft.position;
		pos.z += (speed/MAX_SPEED) * 7;
		Instantiate(BikeLeft, pos, BikeLeft.rotation);
	}

	float GetSpeedWithMean(float mean){

		float diff;

		if(mean < MAX_SPEED/2)
			diff = mean-MIN_SPEED;
		else
			diff = MAX_SPEED-mean;

		return Random.Range(mean-diff, mean+diff);
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate()
	{
		if(Time.time >= nextSpawn)
		{
			CreateBikeRightLane();

			CreateBikeLeftLane();

			nextSpawn  = Time.time + Random.Range(2,densitySlider);
		}



	}

	void ComputeDensity(){
		int nbBike = GameObject.FindGameObjectsWithTag("Bike").Length;
		
		//each road is 18m, the 3 road are 54m
		float k = nbBike / 54.0f;
		
		float u = BikeScript.getAverageSpeed();

		//not sure about the unit;
		var density = u*k;


		//density bike/second
		densityLabel = density.ToString("0.00") + " bikes/sec";

	}
}
