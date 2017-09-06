using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {

	public GameObject ocean;
	public GameObject visual;
	public Gameplay gameplay;

	bool levelOver = false;

	public float distanceUnderWater; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position) - distanceUnderWater, 0);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<CharacterController> () != null) {
			gameplay.advanceLevel ();
			levelOver = true;
			GetComponent<SphereCollider> ().radius = 0.5f;
			GetComponent<SphereCollider> ().isTrigger = false;
		}
	}
		
}
