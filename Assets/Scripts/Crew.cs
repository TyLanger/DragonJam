using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour {


	public GameObject ocean;
	public GameObject visual;

	bool floating = false;

	// Use this for initialization
	void Start () {
		ocean = FindObjectOfType<Ocean> ().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (floating) {
			visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position), 0);
		}

	}

	public void drop()
	{
		// TODO drop animation
		transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		floating = true;
	}

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("Collided with: " + col);
		if (floating) {
			if (col.GetComponent<CharacterController> () != null) {
				// is the player
				col.GetComponent<BoatController> ().gainCrew ();
				Destroy (gameObject);
			}
		}
	}
}
