using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour {


	public GameObject ocean;
	public GameObject visual;
	Transform cameraTrans;
	public TextMesh crewText;
	string[] crewSayings = {"Help!", "Help me!", "Save me!", "Save me, please", "I'm drowning!", "Hurry!", "Hurry, I beg you!", "I can't swim!", "I'll swab the deck if you save me", "Hurry, I see sharks", "Help, there are sharks in these waters" };


	bool floating = false;

	public float timeOfNextCallForHelp = 0;
	public float timeBetweenCallsForHelp = 5;

	// Use this for initialization
	void Start () {
		ocean = FindObjectOfType<Ocean> ().gameObject;
		cameraTrans = FindObjectOfType<CameraController> ().transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (floating) {
			visual.transform.position = transform.position + new Vector3 (0, ocean.GetComponent<Ocean> ().getHeightAtPosition (transform.position), 0);
			if (timeOfNextCallForHelp < Time.time) {
				timeOfNextCallForHelp = Time.time + timeBetweenCallsForHelp;
				callForHelp ();
			}
		}

	}

	void callForHelp()
	{
		var textCopy = Instantiate (crewText, transform.position, cameraTrans.rotation);
		textCopy.text = crewSayings[Random.Range (0, crewSayings.Length)];
	}

	public void drop()
	{
		// TODO drop animation
		transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		floating = true;
	}

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log ("Collided with: " + col);
		if (floating) {
			if (col.GetComponent<CharacterController> () != null) {
				// is the player
				col.GetComponent<BoatController> ().gainCrew ();
				Destroy (gameObject);
			}
		}
	}
}
