using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour {

	public float fade = 0.01f;
	TextMesh textMesh;

	// Use this for initialization
	void Start () {
		textMesh = GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {
		// each frame, reduce the alpha by fade
		// this will make the damage text fade away
		textMesh.color = textMesh.color + new Color (0, 0, 0, -fade);
		// once it is almost entirely tranparent, delete it
		if (textMesh.color.a < 0.01f) {
			Destroy (gameObject);
		}
	}
}
