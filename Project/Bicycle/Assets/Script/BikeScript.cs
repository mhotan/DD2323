using UnityEngine;
using System.Collections;

public class BikeScript : MonoBehaviour {

	public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		float moveHorizontal = -25.0f*speed;

		rigidbody.AddForce(new Vector3(moveHorizontal, 0.0f, 0.0f)*Time.deltaTime);
	}
}
