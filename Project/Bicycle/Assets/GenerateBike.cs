using UnityEngine;
using System.Collections;

public class GenerateBike : MonoBehaviour {

	public Transform Bike;
	public Vector3 SpawnPoint;

	// Use this for initialization
	void Start () {

	
	}

	void CreateBike()
	{
		Instantiate (Bike);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1"))
			CreateBike();
	}
}
