using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour {

	public float maxMoveSpeed;
	float moveDirection = 1;

	public GameObject player;
	BoatController playerController;
	public GameObject visual;
	public GameObject ocean;

	// height the ufo appears to fly over the water
	public float hoverHeight;
	bool abducting = false;
	bool haveCrew = false;
	public GameObject crew;

	// tractor beam
	// distance the ship has to be in order to channel the tractor beam
	public float tractorBeamRange = 3f;
	public float timeToAbduct = 3;
	public float escapeDistance;
	public GameObject tractorBeam;
	Vector3 lockOnPosition;

	// Use this for initialization
	void Start () {
		if (player == null) {
			player = FindObjectOfType<CharacterController> ().gameObject;
		}
		playerController = player.GetComponent<BoatController> ();
		GetComponent<Health> ().OnDeath += dieFromDamage;
	}
	
	// Update is called once per frame
	void Update () {

		if (abducting) {
			// fly at the same speed as the player
			//transform.position = Vector3.MoveTowards (transform.position, player.transform.position + lockOnPosition, playerController.getCurrentMoveSpeed());
			timeToAbduct -= Time.deltaTime;
			if (timeToAbduct < 0) {
				// fly away with the crew member

				abduct ();
				abducting = false;
				// negative to move in opposite direction from the player
				// 0.8 to make the ufo travel slower
				moveDirection = -0.8f;
				transform.parent = null;
			}
		} else {
			transform.position = Vector3.MoveTowards (transform.position, player.transform.position, moveDirection * maxMoveSpeed);
		}

		if ((Vector3.Distance (transform.position, player.transform.position) < tractorBeamRange) && !abducting && !haveCrew) {
			// now in range
			// just latch on to this position

			//lockOnPosition = transform.position - player.transform.position;
			abducting = true;
			transform.parent = player.transform;
			tractorBeam.SetActive (true);
		}

		if (haveCrew && Vector3.Distance (transform.position, player.transform.position) > escapeDistance) {
			// when far enough away, despawn
			// crew member is lost forever
			// spaceship should be off screen so no need for dissappear animation
			destroySelf();
		}

		visual.transform.position = transform.position + new Vector3(0, ocean.GetComponent<Ocean>().getHeightAtPosition(transform.position) + hoverHeight, 0);
	}

	void abduct()
	{
		// check for abducting again
		// If the alien just died, it may have made it past the last abducting
		// This stops the alien from taking the crew and dying, but not dropping the crew
		if (abducting) {
			// try to abduct a crew member if the player has one
			// tow the crew member below the space craft
			if (playerController.loseCrew ()) {
				haveCrew = true;
				var crewCopy = Instantiate (crew, visual.transform.position + new Vector3 (0, -1.5f, 0), transform.rotation);
				crewCopy.transform.parent = visual.transform;
				crew = crewCopy;
			}
		}
	}

	void dieFromDamage()
	{
		abducting = false;
		// drop crew member, if any
		// death animation
		// Destroy(gameObject);
		if (haveCrew) {
			// drop crew
			crew.GetComponent<Crew>().drop();
			crew.transform.parent = null;
		}

		maxMoveSpeed = 0;
		Invoke ("destroySelf", 8);
	}

	public void dieFromReset()
	{
		abducting = false;
		maxMoveSpeed = 0;
		Invoke ("destroySelf", 8);
	}

	void destroySelf()
	{
		Destroy (gameObject);
	}
		
}
