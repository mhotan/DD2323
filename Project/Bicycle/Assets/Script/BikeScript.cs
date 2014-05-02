using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BikeScript : MonoBehaviour {

	Vector3 oldPos;
	float speed;

	void OnDestroy(){
		UnRegister(this);
	}

	// Use this for initialization
	void Start () {
		Register(this);

		oldPos = transform.position;
		speed = 0;
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){

		var newPos = transform.position;
		var distance = Vector3.Distance(oldPos, newPos);
		
		//each road is 10 bikes long, bike average lenght is ~180 cm
		//a road is then ~18m
		//the road is 90 unity unit
		
		var realWorldDist = (distance*18)/90.0f;

		speed = realWorldDist / Time.fixedDeltaTime;

		oldPos = newPos;

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

		float sum = 0;
		foreach(var b in bikes){
			sum += b.speed;
		}
	
		return sum/bikes.Count;
	}
}
