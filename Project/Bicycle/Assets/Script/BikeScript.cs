using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BikeScript : MonoBehaviour {
	

	void OnDestroy(){
		UnRegister(this);
	}

	// Use this for initialization
	void Start () {
		Register(this);
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){

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
	
}
