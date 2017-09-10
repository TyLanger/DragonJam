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
	float sinkRate = 0.15f;
	float sinkTime = 0;
	Vector3 sinkVector;

	public float turnSpeed;
	float turnDirection;

	public GameObject cannonBall;
	public Transform cannonTransform;
	public float timeBetweenAttacks = 0.1f;
	float timeOfNextAttack = 0;

	public GameObject visual;
	public GameObject ocean;

	string[] abductionMessages = { "They took ", "Crew down", "WTF", "I miss him already", "Get him back", "Aliens?!?!" };
	string[] names = { "Larry", "One-eyed Pete", "New guy", "Henry", "Iron hook", "my dog" };
	string[] deathWords = { "We're going down", "Too much damage", "To Davy Jones' locker", "Cap'n, you've doomed us all", "It was an honour sailing with ye lads", "A watery grave" };
	public TextMesh crewText;
	public int numberOfCrew = 1;
	public float fireSpeedBonusPerCrew;
	public float accelerationBonusPerCrew;
	public float healSpeedPerCrew;
	float healAmount;
	// amount of health needed in healAmount before it can tick
	int healThreshold = 5;
	Health health;

	AudioSource audioSource;
	public AudioClip[] bubbles;
	//public AudioClip[] cannonFire;
	//public float cannonPitch;

	// Use this for initialization
	void Start () {
		currentMoveSpeed = maxMoveSpeed;
		health = GetComponent<Health> ();
		sinkVector = new Vector3 (0, 1, 0);
		audioSource = GetComponent<AudioSource> ();
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
		visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position) + (sinking?-(2*sinkRate*(Time.time - sinkTime)):0), 0);
		if (sinking) {
			// cause the boat's front to turn to the sky
			visual.transform.rotation = Quaternion.LookRotation (Vector3.RotateTowards (visual.transform.forward, transform.forward + sinkVector, Mathf.Abs(sinkRate*(Time.time - sinkTime)/10), 0.0f));
			/*if (visual.transform.forward == transform.forward) {
				
				sinking = false;
				Debug.Log ("Sinking = false");
			}*/
			/*if ((visual.transform.forward - transform.forward).magnitude < 0.01f) {
				Debug.Log ("Close enough");

			}*/
		}

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
		audioSource.clip = bubbles [Random.Range (0, bubbles.Length)];
		audioSource.Play ();
		Invoke ("destroyBoat", 8);
	}

	public void sinkPlayer()
	{
		TextMesh deathWordCopy = Instantiate (crewText, transform.position, FindObjectOfType<CameraController> ().transform.rotation);
		deathWordCopy.text = deathWords [Random.Range (0, deathWords.Length)];
		sinking = true;
		sinkTime = Time.time;
		// don't destroy the boat
		Invoke("rise", 8);
	}

	void rise()
	{
		// reset sinkTime or else the boat will bob back up at ta really fast rate
		sinking = true;
		sinkTime = Time.time + 3;
		sinkRate *= -0.5f;
		sinkVector = Vector3.zero;
		// reset the crew to the default
		// if I keep the crew after I die, it could make it slightly easier
		// already easy enough with how you can get crew when all enemies are killed
		numberOfCrew = 1;
		Invoke ("stopRising", 3);
	}
	void stopRising()
	{
		// set it back exactly where it should be
		sinking = false;
		sinkRate *= -2;
		sinkVector = Vector3.up;
		visual.transform.forward = transform.forward;
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
			
			// This works to make a cannon sound, but I don't have any good cannon sounds 
			/*
			audioSource.clip = cannonFire [Random.Range (0, cannonFire.Length)];
			audioSource.pitch = cannonPitch;
			audioSource.Play ();
			audioSource.pitch = 1;
			*/
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
			TextMesh crewMessageCopy = Instantiate(crewText, transform.position, FindObjectOfType<CameraController>().transform.rotation);
			int randomMessage = Random.Range (0, abductionMessages.Length);
			crewMessageCopy.text = abductionMessages [randomMessage];
			if (randomMessage == 0) {
				// add a name
				int randomName = Random.Range(0, names.Length);
				crewMessageCopy.text += names [randomName];
			}

			for (int i = 0; i < Random.Range (0, 4); i++) {
				crewMessageCopy.text += "!";
			}
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
