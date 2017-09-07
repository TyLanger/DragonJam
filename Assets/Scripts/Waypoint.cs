using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

	public GameObject target;
	public GameObject player;
	public CameraController cameraController;

	public float distanceFromPlayer;
	public float distancAboveWater;
	Vector3 playerToTargetVector;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (cameraController.isOffScreen (target.transform.position)) {
			if (!gameObject.GetComponent<MeshRenderer>().enabled) {
				show ();
			}
			playerToTargetVector = target.transform.position - player.transform.position;

			transform.rotation = Quaternion.LookRotation (playerToTargetVector);

			transform.position = player.transform.position + playerToTargetVector.normalized * distanceFromPlayer + new Vector3 (0, distancAboveWater, 0);
		} else if (gameObject.GetComponent<MeshRenderer>().enabled) {
			hide ();
		}
	}

	public void hide()
	{
		gameObject.GetComponent<MeshRenderer>().enabled = false;
	}

	public void show()
	{
		gameObject.GetComponent<MeshRenderer>().enabled = true;
	}

	public void destroyArrow()
	{
		Destroy(gameObject);
	}
}
