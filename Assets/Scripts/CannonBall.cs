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

	GameObject ocean;
	public GameObject visual;
	public AnimationCurve ballHeightCurve;
	public float distanceAboveWater = 2;

	AudioSource woodImpactSource;
	public AudioClip[] woodClips;
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


		visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position) + ballHeightCurve.Evaluate(Vector3.Distance(transform.position, startPoint)/totalDistance), 0);
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
		ocean = o;
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

			}
		}
	}

}
