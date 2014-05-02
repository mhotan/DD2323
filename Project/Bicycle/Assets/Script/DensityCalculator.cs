using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DensityCalculator : MonoBehaviour {


	List<int> nbBike = new List<int>(10);
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20, 1 << 8);


		int size = hitColliders.Length;

		nbBike.Add (size);
		if(nbBike.Count > 100)
			nbBike.RemoveAt(0);

		var density = nbBike.Sum ()/ Time.realtimeSinceStartup;

		print (density);
	
	}
}
