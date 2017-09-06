using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour {

	public float maxMoveSpeed;
	float currentMoveSpeed;
	float throttlePercent = 0;
	public float throttleRate;
	float throttleSign = 0;


	bool sinking = false;
	float sinkRate = 0.1f;
	float sinkTime = 0;

	public float turnSpeed;
	float turnDirection;

	public GameObject cannonBall;
	public Transform cannonTransform;
	public float timeBetweenAttacks = 0.1f;
	float timeOfNextAttack = 0;

	public GameObject visual;
	public GameObject ocean;

	public int numberOfCrew = 1;
	public float fireSpeedBonusPerCrew;
	public float accelerationBonusPerCrew;
	public float healSpeedPerCrew;
	float healAmount;
	// amount of health needed in healAmount before it can tick
	int healThreshold = 5;
	Health health;

	// Use this for initialization
	void Start () {
		currentMoveSpeed = maxMoveSpeed;
		health = GetComponent<Health> ();
	}
	
	// Update is called once per frame
	void Update () {

		throttlePercent += throttleSign * (throttleRate + accelerationBonusPerCrew*numberOfCrew);

		if (throttlePercent > 1.0f) {
			//Debug.Log ("Max speed reached");
			throttlePercent = 1.0f;
		} else if (throttlePercent < 0) {
			throttlePercent = 0f;
			//Debug.Log ("Min speed reached");
		}

		currentMoveSpeed = maxMoveSpeed * throttlePercent;

		// rotates the boat
		transform.Rotate (new Vector3 (0, turnSpeed * turnDirection, 0));

		// moves the boat forward constantly.
		transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward + new Vector3 (0, 0, currentMoveSpeed), currentMoveSpeed);

		// controls the visual part of the boat
		// makes the boat ride the waves. The collider stays in the same y pos, however
		// When the boat is sinking, makes it be slightly lower than the wave
		visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position) + (sinking?-(sinkRate*(Time.time - sinkTime)):0), 0);

		if (!health.isFullHealth ()) {
			// heal a little bit every frame
			healAmount += healSpeedPerCrew * numberOfCrew;
			// when healed enough to fix 1 hp,
			// heal 1 hp
			if (healAmount > healThreshold) {
				health.takeDamage (-healThreshold);
				// remove the 1 hp healed from the total
				// this saves the decimal place
				healAmount -= healThreshold;
			}
		}
	}

	public void sink()
	{
		sinking = true;
		sinkTime = Time.time;
		Invoke ("destroyBoat", 8);
	}

	void destroyBoat()
	{
		Destroy (gameObject);
	}

	public void adjustThrottle(float adjustmentMagnitude)
	{
		// adjustmentMagnitude is -1 to 1
		// Input Horizontal for the player
		throttleSign = adjustmentMagnitude;
	}

	public void steer(float steerDirection)
	{
		// steerDirection is -1 to 1
		// Input Vertical for the player
		turnDirection = steerDirection;
	}

	public void steerToVector(Vector3 targetDir)
	{
		transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards (transform.forward, targetDir, turnSpeed, 0.0F));
	}

	public bool canShoot()
	{
		return Time.time > timeOfNextAttack;
	}

	public void Shoot(Vector3 aimPoint)
	{
		if (Time.time > timeOfNextAttack) {
			timeOfNextAttack = Time.time + (timeBetweenAttacks - fireSpeedBonusPerCrew*numberOfCrew);
			var ballClone = Instantiate (cannonBall, transform.position, transform.rotation);
			ballClone.GetComponent<CannonBall> ().setOcean (ocean);
			ballClone.GetComponent<CannonBall> ().setCreator (gameObject);
			ballClone.GetComponent<CannonBall> ().setTargetPoint (aimPoint);
		}
	}

	public float getCurrentMoveSpeed()
	{
		return currentMoveSpeed;
	}

	public bool loseCrew()
	{
		if (numberOfCrew > 0) {
			numberOfCrew--;
			// lose the crew
			// return true to say that there was a crew member to lose
			return true;
		}
		// return false if no crew member to lose
		return false;
	}

	public void gainCrew()
	{
		numberOfCrew++;
	}
}
