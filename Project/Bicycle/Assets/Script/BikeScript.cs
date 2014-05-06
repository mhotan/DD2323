using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BikeScript : MonoBehaviour {

	Vector3 oldPos;
	float oldTime;
	float speed;

	void OnDestroy(){
		UnRegister(this);
	}

	// Use this for initialization
	void Start () {
		Register(this);

		oldPos = transform.position;
		oldTime = Time.time;
		speed = 0;

		InvokeRepeating("ComputeSpeed", 0.5f, 1.0f);
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){

	}

	void ComputeSpeed(){
		var newPos = transform.position;
		var distance = Vector3.Distance(oldPos, newPos);
		var deltaTime = Time.time - oldTime;
		
		speed = distance / deltaTime;
		oldPos = newPos;
		oldTime = Time.time;

	}



	//static bike manager

	private static List<BikeScript> bikes = new List<BikeScript>();

	private static void Register(BikeScript bike){
		if(!bikes.Contains(bike))
			bikes.Add(bike);
	}

	private static void UnRegister(BikeScript bike){
		if(bikes.Contains(bike))
			bikes.Remove(bike);
	}

	public static float getAverageSpeed(){
		if(bikes.Count == 0)
			return 0.0f;

		float sum = 0;
		foreach(var b in bikes){
			sum += b.speed;
		}
	
		return sum/bikes.Count;
	}
}
