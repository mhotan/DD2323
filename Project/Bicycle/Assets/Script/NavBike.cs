using UnityEngine;
using System.Collections;

public class NavBike : MonoBehaviour {

	public Transform target;
	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();

	}
	
	// Update is called once per frame
	void Update () {
		agent.SetDestination(target.position);

	}

	void FixedUpdate(){
		if (Vector3.Distance(target.position, gameObject.transform.position) < 10)
			Destroy(gameObject.gameObject);

	}
}
