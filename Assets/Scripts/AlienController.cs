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
	bool sinking = false;
	// minimum sink rate needed to drop 5 units in y in 8 sec
	// 5 because that's what makes it look under water
	float sinkRate = 0.625f;
	float timeStartedSinking;
	Vector3 sinkVector = new Vector3 (0, -2, 0);
	float sinkAngleRate = 0.01f;

	// tractor beam
	// distance the ship has to be in order to channel the tractor beam
	public float tractorBeamRange = 3f;
	public float timeToAbduct = 3;
	public float escapeDistance;
	public GameObject tractorBeam;
	Vector3 lockOnPosition;
	bool alive = true;

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

		if ((Vector3.Distance (transform.position, player.transform.position) < tractorBeamRange) && !abducting && !haveCrew && alive) {
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

		if (!sinking) {
			visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position) + hoverHeight, 0);
		} else {
			// multiply the sink rate by 2 to make it go down faster
			// Need to go down faster to sync up with the rotation
			// slower rotation looks weird
			// also, rotations are hard so it's easier to adjust the position
			visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position) + hoverHeight - 2*sinkRate * (Time.time - timeStartedSinking), 0);
			visual.transform.rotation = Quaternion.LookRotation (Vector3.RotateTowards (visual.transform.forward, transform.forward + sinkVector, sinkAngleRate, 0.0f));
		}
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

	void sink()
	{
		// tilt to one side, then fall into the water
		sinking = true;
		timeStartedSinking = Time.time;
	}

	void dieFromDamage()
	{
		dieFromReset ();
		// drop crew member, if any
		// death animation
		// Destroy(gameObject);
		if (haveCrew) {
			// drop crew
			crew.GetComponent<Crew>().drop();
			crew.transform.parent = null;
		}


	}

	public void dieFromReset()
	{
		sink ();
		alive = false;
		abducting = false;
		transform.parent = null;
		// slowly move forwards
		maxMoveSpeed *= 0.2f;
		Invoke ("destroySelf", 8);
	}

	void destroySelf()
	{
		Destroy (gameObject);
	}
		
}
