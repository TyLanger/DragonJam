using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour {

	public GameObject ocean;
	public GameObject visual;
	public Gameplay gameplay;

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

			// only want to advance level once
			// make the collider no longer a trigger so that this function can't be called again
			// this also makes it so the boat can collide with the island and not pass through it
			// reduce the radius so it matches the visual
			GetComponent<SphereCollider> ().radius = 0.5f;
			GetComponent<SphereCollider> ().isTrigger = false;
		}

	}

	public void resetIsland()
	{
		// want the island to sink like all the enemies

		// easy solution
		Destroy(gameObject);
	}
		
}
