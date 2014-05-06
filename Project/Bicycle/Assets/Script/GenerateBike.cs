using UnityEngine;
using System.Collections;

public class GenerateBike : MonoBehaviour {

	public Transform Bike;
	public Transform SpawnLeft;
	public Transform SpawnRight;
	public Transform TargetLeft;
	public Transform TargetRight;

	private float nextSpawnR = 1.0f;
	private float nextSpawnL = 1.0f;
	private float densitySlider = 7.0f;
	private float speedSlider = 20.0f;
	private string flowLabel = "";
	private string speedLabel = "";

	private readonly float MAX_SPAWN_TIME = 15.0f;
	private readonly float MIN_SPAW_TIME = 1.0f;

	private readonly float MAX_SPEED = 50.0f;
	private readonly float MIN_SPEED = 8.0f;
	private readonly float RANGE = 3.0f;

	// Use this for initialization
	void Start () {
		InvokeRepeating("ComputeFlow", 1, 0.5f);
		nextSpawnR = Random.Range (0.5f, 1.5f);
		nextSpawnL = Random.Range (0.5f, 1.5f);
	
	}

	void OnGUI () {
		//Traffic density GUI
		GUI.Box(new Rect(125, 10, 100, 60), "Traffic Density");
		densitySlider = GUI.HorizontalSlider (new Rect (130, 35, 90, 30), densitySlider, MAX_SPAWN_TIME, MIN_SPAW_TIME);
		GUI.Label(new Rect(130, 45, 95, 30), flowLabel);

		//Speed slider Gui
		GUI.Box(new Rect(235, 10, 100, 60), "Speed");
		speedSlider = GUI.HorizontalSlider (new Rect (240, 35, 90, 30), speedSlider, MIN_SPEED, MAX_SPEED);
		GUI.Label(new Rect(235, 45, 90, 30), speedLabel);
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
		float speed = GetRandomSpeed(speedSlider);
		Bike.GetComponent<NavMeshAgent>().speed = speed;
		Bike.GetComponent<NavMeshAgent>().avoidancePriority = Mathf.RoundToInt(speed);
		Bike.GetComponent<NavBike>().target = TargetLeft;
		Vector3 pos = SpawnRight.position;
		pos.z -= (speed/MAX_SPEED) * 7;
		Instantiate (Bike, pos, Bike.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f));
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
		float speed = GetRandomSpeed(speedSlider);
		Bike.GetComponent<NavMeshAgent>().speed = speed;
		Bike.GetComponent<NavMeshAgent>().avoidancePriority = Mathf.RoundToInt(speed);
		Bike.GetComponent<NavBike>().target = TargetRight;
		Vector3 pos = SpawnLeft.position;
		pos.z += (speed/MAX_SPEED) * 7;
		Instantiate(Bike, pos, Bike.rotation);
	}

	float GetRandomSpeed(float speed){
		return Random.Range(speed-RANGE, speed+RANGE);
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate()
	{
		if(Time.time >= nextSpawnR)
		{
			CreateBikeRightLane();
			nextSpawnR  = Time.time + Random.Range(Mathf.Max(1.0f, densitySlider-2.0f),densitySlider+2.0f);
		}

		if(Time.time >= nextSpawnL)
		{
			CreateBikeLeftLane();
			nextSpawnL  = Time.time + Random.Range(Mathf.Max(1.0f, densitySlider-2.0f),densitySlider+2.0f);
		}



	}

	//Compute the traffic flow according to the LWR model
	void ComputeFlow(){
		int nbBike = GameObject.FindGameObjectsWithTag("Bike").Length;
		
		//each road is 90 units, we have 3 roads: 270 units
		//k is the bike density
		float k = nbBike / 270.0f;
		
		float u = BikeScript.getAverageSpeed();
		//update the speedLabel in the same time
		speedLabel = u.ToString("0.00");

		//u is the average speed => distance per second
		//k is the number of bikes per unit of distance
		//this give us the flow => bike per second
		var flow = u*k;

		//bike per minutes
		flow *= 60;

		//density bike/second
		flowLabel = flow.ToString("0.00") + " bikes/min";

	}
}
