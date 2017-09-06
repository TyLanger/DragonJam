using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {


	public int maxHealth = 100;
	public int currentHealth;

	public TextMesh damageText;

	string[] damageWords = { "Boom", "Ouch", "Crash", "Fire", "More Fire", "Hit", "Damage", "Smash", "Thunk" };
	string[] healWords = { "Heal", "Fixed", "Bandage Applied", "Hole Patched", "Fire out"};

	Transform cameraTrans;

	bool dead = false;

	public delegate void DeathDelegate();
	public DeathDelegate OnDeath;



	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		cameraTrans = FindObjectOfType<Camera> ().transform;
	}

	public void takeDamage(int damage)
	{
		// if already dead, dont take any more damage
		if (dead) {
			return;
		}

		bool addWord = (Random.Range (0, 4) == 0);

		var clone = Instantiate (damageText, transform.position, cameraTrans.rotation);
		if (damage > 0) {
			clone.text = "-" + damage.ToString ();
			if (addWord) {
				// add a word on the end
				clone.text += " " + damageWords [Random.Range (0, damageWords.Length)];
			}
		} else {
			// change the colour to green for a heal
			clone.color = Color.green;
			// remove the - from the string
			clone.text = "+" + damage.ToString ().Substring(1);
				if(addWord)
				{
				clone.text += " " + healWords[Random.Range(0, healWords.Length)];
				}
			clone.transform.localScale = Vector3.one;
		}
		if (addWord) {
			for (int i = 0; i < Random.Range (0, 4); i++) {
				clone.text += "!";
			}
		}

		currentHealth -= damage;
		if (currentHealth <= 0) {
			die ();
		}
		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}
	}

	public void heal(int healAmount)
	{
		// don't heal if full health
		if (!isFullHealth ()) {
			currentHealth += healAmount;
			// in case of ovverhealing
			if (currentHealth > maxHealth) {
				currentHealth = maxHealth;
			}
		}
	}

	void die()
	{
		dead = true;
		if (OnDeath != null) {
			OnDeath ();
		}
	}

	public bool isFullHealth()
	{
		return !(currentHealth < maxHealth);
	}
}
