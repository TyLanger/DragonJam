using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {


	public BoatController boat;
	public Gameplay gameplay;
	Vector3 aimPoint;
	bool dead = false;

	public Transform cursor;

	// Use this for initialization
	void Start () {
		GetComponent<Health> ().OnDeath += playerDeath;
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!dead) {
			boat.adjustThrottle (Input.GetAxisRaw ("Vertical"));
			boat.steer (Input.GetAxisRaw ("Horizontal"));
		}

		Ray CameraRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		Plane aimPlane = new Plane (Vector3.up, Vector3.zero);

		float cameraDistance;

		if (aimPlane.Raycast (CameraRay, out cameraDistance)) {
			aimPoint = CameraRay.GetPoint (cameraDistance);
			cursor.position = aimPoint;
		}


		if (Input.GetButtonDown ("Fire1") && !dead) {
			// left click
			boat.Shoot(aimPoint);
		}
	}

	void playerDeath()
	{
		Debug.Log ("You have died");
		dead = true;
		gameplay.resetLevel ();
		boat.sinkPlayer ();
		Invoke ("revive", 8);
	}

	void revive()
	{
		dead = false;
		GetComponent<Health> ().revive ();
		gameplay.playerRevived ();
	}
}
