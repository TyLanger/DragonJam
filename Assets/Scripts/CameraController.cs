using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


	public Transform followObject;
	public Vector3 offset;
	public float followSpeed;
	public float horizontalDistance;
	public Vector2 verticalNearFarDistance;
	float catchupRate = 0.001f;
	public float catchup = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		

		if (tooFarAway ()) {
			catchup += catchupRate;
		} else {
			if (catchup > 0) {
				catchup -= catchupRate / 10;
			}
		}

		/*
		if (catchup > 0.03f) {
			catchup = 0;
		}*/


		transform.position = Vector3.MoveTowards (transform.position, followObject.position + offset, followSpeed+catchup);

	}

	bool tooFarAway()
	{
		if ((followObject.position.x > (transform.position.x + horizontalDistance)) || (followObject.position.x < (transform.position.x - horizontalDistance))) {
			// too far left or right
			return true;
		}
		else if((followObject.position.z < transform.position.z + verticalNearFarDistance.x) || (followObject.position.z > transform.position.z + verticalNearFarDistance.y))
		{
			return true;
		}
		return false;
	}
}
