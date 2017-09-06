using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {


	public BoatController boat;
	Vector3 aimPoint;

	// Use this for initialization
	void Start () {
		GetComponent<Health> ().OnDeath += playerDeath;
	}
	
	// Update is called once per frame
	void Update () {
		boat.adjustThrottle (Input.GetAxisRaw ("Vertical"));
		boat.steer (Input.GetAxisRaw ("Horizontal"));

		Ray CameraRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		Plane aimPlane = new Plane (Vector3.up, Vector3.zero);

		float cameraDistance;

		if (aimPlane.Raycast (CameraRay, out cameraDistance)) {
			aimPoint = CameraRay.GetPoint (cameraDistance);
		}


		if (Input.GetButtonDown ("Fire1")) {
			// left click
			boat.Shoot(aimPoint);
		}
	}

	void playerDeath()
	{
		Debug.Log ("You have died");
	}
}
