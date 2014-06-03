﻿using UnityEngine;
using System.Collections;

public class GenerateBike : MonoBehaviour {

	public Transform Bike;
	public Transform SpawnLeft;
	public Transform SpawnRight;
	public Transform TargetLeft;
	public Transform TargetRight;

	// Customizable Texture for Hats
	public Texture hatTexture1;
	public Texture hatTexture2;
	public Texture hatTexture3;
	public Texture hatTexture4;
	public Texture hatTexture5;

	// Customizable Texture for Shirt
	public Texture shirtTexture1;
	public Texture shirtTexture2;
	public Texture shirtTexture3;
	public Texture shirtTexture4;
	public Texture shirtTexture5;
	
	// Customizable Texture for Pants
	public Texture pantsTexture1;
	public Texture pantsTexture2;
	public Texture pantsTexture3;
	public Texture pantsTexture4;
	public Texture pantsTexture5;

	// Customizable Texture for the Bike Body
	public Texture bikeBodyTexture1;
	public Texture bikeBodyTexture2;
	public Texture bikeBodyTexture3;
	public Texture bikeBodyTexture4;
	public Texture bikeBodyTexture5;

	// Skin Texture S
	public Texture skinTexture1;
	public Texture skinTexture2;
	public Texture skinTexture3;
	public Texture skinTexture4;
	public Texture skinTexture5;

	// Customizable Material for the Bike Wheel.
	public Material bikeWheelMaterial1;
	public Material bikeWheelMaterial2;

	private float nextSpawnR = 1.0f;
	private float nextSpawnL = 1.0f;
	private float densitySlider = 7.0f;
	private float speedSlider = 20.0f;
	private string flowLabel = "";
	private string speedLabel = "";
	private string variabilityLabel = "";
	private float variabilitySlider = 1.0f;

	private readonly float MAX_SPAWN_TIME = 15.0f;
	private readonly float MIN_SPAWN_TIME = 1.0f;

	private readonly float MAX_SPEED = 50.0f;
	private readonly float MIN_SPEED = 8.0f;
	private readonly float RANGE = 3.0f;

	private readonly float MIN_VARIABILITY = 1.0f;
	private readonly float MAX_VARIABILITY = 5.99f;

	// Game Object Indexes
	private readonly int RIDER_INDEX = 6;
	private readonly int HAT_INDEX = 17;
	private readonly int BIKE_UPPER_INDEX = 12;
	private readonly int BIKE_LOWER_INDEX = 13;
	private readonly int BIKE_WHEEL_FRONT_INDEX = 16;
	private readonly int BIKE_WHEEL_BACK_INDEX = 15;

	// Material Indexes.
	private readonly int SKIN_MATERIAL_INDEX = 0;
	private readonly int SHIRT_MATERIAL_INDEX = 1;
	private readonly int PANTS_MATERIAL_INDEX = 6;

	// Use this for initialization
	void Start () {
		InvokeRepeating("ComputeFlow", 1, 0.5f);
		nextSpawnR = Random.Range (0.5f, 1.5f);
		nextSpawnL = Random.Range (0.5f, 1.5f);	
	}

	void OnGUI () {
		//Traffic density GUI
		GUI.Box(new Rect(125, 10, 100, 60), "Traffic Density");
		densitySlider = GUI.HorizontalSlider (new Rect (130, 35, 90, 30), densitySlider, MAX_SPAWN_TIME, MIN_SPAWN_TIME);
		GUI.Label(new Rect(130, 45, 95, 30), flowLabel);

		//Speed slider Gui
		GUI.Box(new Rect(235, 10, 100, 60), "Speed");
		speedSlider = GUI.HorizontalSlider (new Rect (240, 35, 90, 30), speedSlider, MIN_SPEED, MAX_SPEED);
		GUI.Label(new Rect(235, 45, 90, 30), speedLabel);

		//Variability slider Gui
		GUI.Box(new Rect(345, 10, 100, 60), "Variability");
		variabilitySlider = GUI.HorizontalSlider (new Rect (350, 35, 90, 30), variabilitySlider, MIN_VARIABILITY, MAX_VARIABILITY);
		GUI.Label(new Rect(345, 45, 90, 30), variabilityLabel);
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
		var clone = Instantiate (Bike, pos, Bike.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f)) as Transform;
		updateBike (clone);
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
		var clone = Instantiate(Bike, pos, Bike.rotation) as Transform;
		updateBike (clone);
	}

	int getVariability() {
		return (int)Random.Range (MIN_VARIABILITY, variabilitySlider);
	}

	void updateBike(Transform biker) {
		updateHat (biker);
		updateShirt (biker);
		updatePants (biker);
		updateBikeBody (biker);
		updateBikeWheel (biker);
		updateSkin (biker);
	}

	// Update the texture of the hat based on the variability defined.
	void updateHat(Transform biker) {
		// Obtain the Hat Material
		Material mat = biker.GetChild (0).GetChild (HAT_INDEX).renderer.material;
		Debug.Log ("Hat texture originally " + mat.mainTexture.name);
		switch (getVariability ()) {		
		case 1:
			mat.mainTexture = hatTexture1;
			break;
		case 2:
			mat.mainTexture = hatTexture2;
			break;
		case 3:
			mat.mainTexture = hatTexture3;
			break;
		case 4:
			mat.mainTexture = hatTexture4;
			break;
		default:
			mat.mainTexture = hatTexture5;
			return;
		}
		Debug.Log ("Hat Updated to " + biker.GetChild (0).GetChild (HAT_INDEX).renderer.material.mainTexture.name);
	}


	void updateShirt(Transform biker) {
		// The Shirt Material.
		Material mat = biker.GetChild (0).GetChild (RIDER_INDEX).renderer.materials [SHIRT_MATERIAL_INDEX];
		Debug.Log ("Shirt texture originally " + mat.mainTexture.name);
		switch (getVariability ()) {		
		case 1:
			mat.mainTexture = shirtTexture1;
			break;
		case 2:
			mat.mainTexture = shirtTexture2;
			break;
		case 3:
			mat.mainTexture = shirtTexture3;
			break;
		case 4:
			mat.mainTexture = shirtTexture4;
			break;
		default:
			mat.mainTexture = shirtTexture5;
			return;
		}
		Debug.Log ("Shirt Updated to " + biker.GetChild (0).GetChild (RIDER_INDEX).renderer.materials [SHIRT_MATERIAL_INDEX].mainTexture.name);
	}

	// Update the pants material based off the level of variability.
	void updatePants(Transform biker) {
		// The Pants Material.
		Material mat = biker.GetChild (0).GetChild (RIDER_INDEX).renderer.materials [PANTS_MATERIAL_INDEX];
		Debug.Log ("Pants texture originally " + mat.mainTexture.name);
		switch (getVariability ()) {		
		case 1:
			mat.mainTexture = pantsTexture1;
			break;
		case 2:
			mat.mainTexture = pantsTexture2;
			break;
		case 3:
			mat.mainTexture = pantsTexture3;
			break;
		case 4:
			mat.mainTexture = pantsTexture4;
			break;
		default:
			mat.mainTexture = pantsTexture5;
			return;
		}
		Debug.Log ("Pants Updated to " + biker.GetChild (0).GetChild (RIDER_INDEX).renderer.materials [PANTS_MATERIAL_INDEX].mainTexture.name);
	}

	void updateBikeBody(Transform biker) {
		Material upperMat = biker.GetChild (0).GetChild (BIKE_UPPER_INDEX).renderer.material;
		Material lowerMat = biker.GetChild (0).GetChild (BIKE_LOWER_INDEX).renderer.material;
		Debug.Log ("Bike Upper Body texture was " + upperMat.mainTexture.name + " and lower texture was " + lowerMat.mainTexture.name);
		switch (getVariability ()) {		
		case 1:
			lowerMat.mainTexture = bikeBodyTexture1;
			upperMat.mainTexture = bikeBodyTexture1;
			break;
		case 2:
			lowerMat.mainTexture = bikeBodyTexture2;
			upperMat.mainTexture = bikeBodyTexture2;
			break;
		case 3:
			lowerMat.mainTexture = bikeBodyTexture3;
			upperMat.mainTexture = bikeBodyTexture3;
			break;
		case 4:
			lowerMat.mainTexture = bikeBodyTexture4;
			upperMat.mainTexture = bikeBodyTexture4;
			break;
		default:
			lowerMat.mainTexture = bikeBodyTexture5;
			upperMat.mainTexture = bikeBodyTexture5;
			return;
		}
		Debug.Log ("Bike Upper Body texture is now " + upperMat.mainTexture.name + " and lower texture is now " + lowerMat.mainTexture.name);
	}

	// update the material of the bike wheel.
	void updateBikeWheel(Transform biker) {
		if (getVariability() % 2 == 0) {
			biker.GetChild (0).GetChild (BIKE_WHEEL_BACK_INDEX).renderer.material = bikeWheelMaterial1;
			biker.GetChild (0).GetChild (BIKE_WHEEL_FRONT_INDEX).renderer.material = bikeWheelMaterial1; 
		} else {
			biker.GetChild (0).GetChild (BIKE_WHEEL_BACK_INDEX).renderer.material = bikeWheelMaterial2;
			biker.GetChild (0).GetChild (BIKE_WHEEL_FRONT_INDEX).renderer.material = bikeWheelMaterial2;
		}
	}

	void updateSkin(Transform biker) {
		Material mat = biker.GetChild (0).GetChild (RIDER_INDEX).renderer.materials [SKIN_MATERIAL_INDEX];
		Debug.Log ("Skin texture originally " + mat.mainTexture.name);
		switch (getVariability ()) {		
		case 1:
			mat.mainTexture = skinTexture1;
			break;
		case 2:
			mat.mainTexture = skinTexture2;
			break;
		case 3:
			mat.mainTexture = skinTexture3;
			break;
		case 4:
			mat.mainTexture = skinTexture4;
			break;
		default:
			mat.mainTexture = skinTexture5;
			return;
		}
		Debug.Log ("Skin updated to " + biker.GetChild (0).GetChild (RIDER_INDEX).renderer.materials [SKIN_MATERIAL_INDEX].mainTexture.name);
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

		// Variability label
		variabilityLabel = "Variation: " + variabilitySlider.ToString ("0");
	}
}
