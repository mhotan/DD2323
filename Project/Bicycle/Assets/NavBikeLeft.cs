using UnityEngine;
using System.Collections;

public class NavBikeLeft : MonoBehaviour {

	Transform target;
	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		target = GameObject.Find("TargetRight").transform;
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
