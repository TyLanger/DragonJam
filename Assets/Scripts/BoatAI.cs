using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatAI : MonoBehaviour {

	BoatController boat;
	public GameObject player;
	public GameObject crew;

	delegate void DecisionDelegate();
	DecisionDelegate enemyAction;

	float timeBetweenDecisions = 0.15f;
	float timeOfNextDecision = 0;

	public float attackRange;
	public float aimVariance = 3f;

	// Use this for initialization
	void Start () {
		if (player == null) {
			player = FindObjectOfType<CharacterController> ().gameObject;
		}
		boat = GetComponent<BoatController> ();
		enemyAction = attackPlayer;
		boat.adjustThrottle (1);
		GetComponent<Health> ().OnDeath += die;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > timeOfNextDecision) {
			timeOfNextDecision = Time.time + timeBetweenDecisions;
			enemyAction ();
		}
	}

	void die()
	{
		// death
		// health component ran out of health so it called this
		enemyAction = stopMoving;
		if (Random.Range (0, 4) == 0) {
			dropCrew ();
		}
	}

	public void dieFromReset()
	{
		// called when the player dies
		enemyAction = stopMoving;
	}

	void dropCrew()
	{
		Debug.Log ("Spawning crew");
		var crewCopy = Instantiate (crew, transform.position, transform.rotation);
		crewCopy.GetComponent<Crew> ().drop ();

	}

	void stopMoving()
	{
		// slow the boat down until it stops
		boat.adjustThrottle (-1.0f);
		if (boat.getCurrentMoveSpeed () < 0.1f) {
			enemyAction = sink;
		}
	}
		
	void sink()
	{
		// tell the boat to sink
		boat.sink ();
		// destroy this (boatAI.cs) so the boat stops thinking
		// if it isn't stopped, it keeps calling sink and resetting the sink timer
		Destroy (this);
	}

	void attackPlayer()
	{
		if (Vector3.Distance (player.transform.position, transform.position) > attackRange) {
			enemyAction = moveTowardsPlayer;
		}
		else if (boat.canShoot ()) {
			boat.Shoot (calculateAimPosition ());
		}
	}

	void moveTowardsPlayer()
	{
		if (Vector3.Distance (player.transform.position, transform.position) < attackRange) {
			enemyAction = attackPlayer;
		}

		float playerMoveSpeed = player.GetComponent<BoatController> ().getCurrentMoveSpeed ();
		// move towards some point in front of the player
		Vector3 targetPosition = player.transform.position + player.transform.forward * playerMoveSpeed;
		//Debug.Log (targetPosition);

		boat.steerToVector (targetPosition - transform.position);
		//transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards (transform.forward, targetPosition, 5, 0));

		// am I pointed towards that point?
		// forward is where the boat is facing
		// target - trans is the line from me to the target
		// these should be the same 
		/*
		float dot = Vector3.Angle ((targetPosition - transform.position).normalized, transform.forward);
		Debug.Log ("Angle: " + dot + " forward: "+transform.forward + " line: "+ (targetPosition - transform.position).normalized);
		boat.steer(dot / Mathf.Abs(dot));
		// Vector3.Distance(player.transform.position, transform.position)


		Vector3 lineToPlayer = (player.transform.position - transform.position).normalized;

		boat.adjustThrottle (lineToPlayer.z);
		boat.steer (lineToPlayer.x);
		*/


	}

	Vector3 calculateAimPosition()
	{
		float projectileSpeed = boat.cannonBall.GetComponent<CannonBall> ().moveSpeed;
		float playerMoveSpeed = player.GetComponent<BoatController> ().getCurrentMoveSpeed ();
		float playerDistance = Vector3.Distance (player.transform.position, transform.position);

		// this is actually relatively accurate. Misses at longer distances
		Vector3 aimPoint = player.transform.position + player.transform.forward * playerMoveSpeed * playerDistance / projectileSpeed;
		// add some randomness so the shot isn't always super accurate
		aimPoint += new Vector3(Random.Range(-aimVariance, aimVariance), 0, Random.Range(-aimVariance, aimVariance));
		return aimPoint;
	}
}
