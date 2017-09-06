using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

	public float moveSpeed = 0.2f;
	public int damage = 25;
	Vector3 targetPoint;

	public float timeAlive = 2f;

	GameObject creator;

	GameObject ocean;
	public GameObject visual;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (timeAlive < 0) {
			despawn ();
		}
		timeAlive -= Time.deltaTime;

		transform.position = Vector3.MoveTowards (transform.position, targetPoint, moveSpeed);

		visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position), 0);
	}

	public void setTargetPoint(Vector3 point)
	{
		targetPoint = point;
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
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("Hit something " + col.gameObject);
		if ((col.GetComponent<Health> () != null) && (col.gameObject != creator)) {
			col.GetComponent<Health> ().takeDamage (damage);
		}
	}

}
