using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

	public float moveSpeed = 0.2f;
	public int damage = 25;
	Vector3 targetPoint;
	Vector3 startPoint;
	float totalDistance;

	public float timeAlive = 2f;


	GameObject creator;

	Ocean ocean;
	public GameObject visual;
	public AnimationCurve ballHeightCurve;
	public float distanceAboveWater = 2;

	// water splash particle
	public GameObject splash;
	public Vector3 splashHeight;
	bool notSplashed = true;
	GameObject splashToDestroy;

	// wood impact particle
	public GameObject woodImpactParticle;
	public Vector3 woodParticleHeight;
	GameObject woodToDestroy;

	AudioSource woodImpactSource;
	public AudioClip[] woodClips;

	public AudioClip[] splashes;
	// Use this for initialization
	void Start () {
		startPoint = transform.position;
		woodImpactSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		 
		// despawn when it reaches its destination
		if (Vector3.Distance (transform.position, targetPoint) < 0.01f) {
			despawn ();
		}
		// despawn if alive too long
		if (timeAlive < 0) {
			despawn ();
		}
		timeAlive -= Time.deltaTime;

		transform.position = Vector3.MoveTowards (transform.position, targetPoint, moveSpeed);


		visual.transform.position = transform.position + new Vector3 (0, ocean.getHeightAtPosition (transform.position) + ballHeightCurve.Evaluate(Vector3.Distance(transform.position, startPoint)/totalDistance), 0);
		/*
		 * The particle system on the spash doesn't look great
		 * Colours still aren't right
		 */
		if (visual.transform.position.y < ocean.getHeightAtPosition (transform.position) && notSplashed) {

			woodImpactSource.clip = splashes [Random.Range(0, splashes.Length)];
			//woodImpactSource.
			woodImpactSource.Play ();
			notSplashed = false;

			var splashCopy = Instantiate (splash, visual.transform.position + splashHeight, visual.transform.rotation);
			splashToDestroy = splashCopy;

			// this doesn't seem to work
			// might be overwriting it with the material?
			//splashCopy.transform.parent = transform;
			//var mainModule = splashCopy.GetComponent<ParticleSystem> ().main;
			//mainModule.startColor = ocean.getColourAtHeight (ocean.getHeightAtPosition(transform.position));
			//var emitParams = new ParticleSystem.EmitParams ();
			//emitParams.startColor = ocean.GetComponent<Ocean> ().getColourAtHeight (visual.transform.position.y);
			//splashCopy.GetComponent<ParticleSystem> ().main.startColor = ocean.GetComponent<Ocean> ().getColourAtHeight (visual.transform.position.y);
			//notSplashed = false;

		}
	}

	public void setTargetPoint(Vector3 point)
	{
		targetPoint = point;
		totalDistance = Vector3.Distance (transform.position, point);
	}

	public void setCreator(GameObject c)
	{
		creator = c;
	}

	public void setOcean(GameObject o)
	{
		ocean = o.GetComponent<Ocean>();
	}

	void despawn()
	{
		
		// turn off the collider so it does nothing
		GetComponent<SphereCollider> ().enabled = false;
		// make the cannonball dissappear below the water
		visual.SetActive(false);
		// set the cannonball to be destroyed in a second
		// give it time to play an animation
		Invoke ("destroyCannonball", 1);
	}

	void destroyCannonball()
	{
		if (woodToDestroy != null) {
			Destroy (woodToDestroy);
		}
		Destroy (splashToDestroy);
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("Hit something " + col.gameObject);
		if ((col.GetComponent<Health> () != null) && (col.gameObject != creator)) {
			col.GetComponent<Health> ().takeDamage (damage);
			if (woodImpactSource != null) {
				woodImpactSource.clip = woodClips[Random.Range(0, woodClips.Length)];
				woodImpactSource.Play ();
				// don't also play the splash sound
				notSplashed = false;
				var woodCopy = Instantiate (woodImpactParticle, visual.transform.position + woodParticleHeight, visual.transform.rotation);
				woodToDestroy = woodCopy;
			}
		}
	}

}
