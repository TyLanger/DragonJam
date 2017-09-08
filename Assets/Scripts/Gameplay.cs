using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour {

	public enum WinState {Start, Elimination, aToB, Survival, End};


	// This struct may be entirely useless
	// there is almost no overlap of variables between states
	// this could all be accomplished with one WinState
	// WinState currentStage
	// And all these variables jsut become a part of Gameplay.cs once
	// Changing this would mean I would have to fill out all those fields again
	// Only thing that overlaps is the messages
	// Probably jsut keep this for now, but next time could be done much better
	[System.Serializable]
	public struct GameStage {

		public WinState winState;

		public string stateMessage;
		public string tipMessage;
		public string winMessage;

		// the game spawns enemies and you need to kill them
		// moves on when you killed them all
		public int[] enemiesToKill;

		// have to get to some point B
		// start at point A
		public Vector3 pointA;
		public Vector3 pointB;

		// need to survive for timeToSurvive seconds
		// when the time ends, need to kill the remaining enemies
		public float timeToSurvive;
		public float originalTimeToSurvive;
		public float survivalTimeBetweenSpawns;
		public AnimationCurve lightIntensity;
		int enemiesLeftToKill;

	};


		
	public GameObject player;
	public GameObject pirate;
	public GameObject alien;
	public GameObject ocean;
	// waypoint for aToB
	public GameObject wayPoint;
	public Light sunLight;
	public Text uiText;
	public TextMesh crewText;
	// waypoint for the arrows to show the way
	public Waypoint arrow;
	public Transform cameraTrans;


	float timeOfNextSpawn = 0;
	float timeBetweenSpawns = 10;

	int numSpawnedEnemies = 0;
	public int numDeadEnemies;
	bool waypointsNotSpawned = true;

	public GameStage[] gameStages;
	int currentStage = 0;
	bool tipDisplayed = false;

	bool playerDead = false;

	// Use this for initialization
	void Start () {
		numDeadEnemies = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (playerDead) {
			return;
		}
		if (gameStages[currentStage].winState != WinState.Start && gameStages[currentStage].winState != WinState.End) {
			if (Time.time > timeOfNextSpawn) {
				timeOfNextSpawn = Time.time + timeBetweenSpawns;

				// right now, elimination only works for 1 enemy type
				if ((gameStages [currentStage].winState == WinState.Elimination) && (gameStages [currentStage].enemiesToKill [0] <= numSpawnedEnemies)) {
					// all the enemies for elimination have been spawned
					if (waypointsNotSpawned) {
						// use this condition so waypoints only get spawned once
						// once all the enemies have been spwned for this stage,
						// put waypoints on all remaining boats in case the player missed one and needs to find it
						waypointsNotSpawned = false;
						putWaypointsForRemainingEnemies ();
					}
					if (numDeadEnemies == gameStages [currentStage].enemiesToKill [0]) {
						advanceLevel ();
					}
				} else {
					spawnEnemy ();
					numSpawnedEnemies++;
				}
			}
		}
		if (gameStages [currentStage].winState == WinState.Elimination) {
			if ((numDeadEnemies == (gameStages [currentStage].enemiesToKill [0] - 3)) && !tipDisplayed) {
				
				shoutMessage (gameStages [currentStage].tipMessage);
				tipDisplayed = true;
			}
		}
		if (gameStages [currentStage].winState == WinState.Survival) {
			sunLight.intensity = gameStages[currentStage].lightIntensity.Evaluate((gameStages [currentStage].timeToSurvive/gameStages[currentStage].originalTimeToSurvive));
			gameStages [currentStage].timeToSurvive -= Time.deltaTime;
			if (gameStages [currentStage].timeToSurvive < 30 && !tipDisplayed) {
				// print the tip message when only 30 seconds left
				shoutMessage (gameStages [currentStage].tipMessage);
				tipDisplayed = true;
			}
			if (gameStages [currentStage].timeToSurvive < 0) {
				advanceLevel ();
			}
		}

		if (gameStages [currentStage].winState == WinState.aToB && !tipDisplayed) {
			if (Vector3.Distance (player.transform.position, gameStages [currentStage].pointB) < 50) {
				shoutMessage (gameStages [currentStage].tipMessage);
				tipDisplayed = true;
			}
		}
	}

	void spawnEnemy ()
	{
		// x and z position are 5 to 10 units away from the player
		// either positive or negative
		float signX = Random.Range (-1, 1);
		float signZ = Random.Range (-1, 1);
		if (signX == 0 && signZ == 0) {
			// stop both signs being 0
			// if they are both 0, they will spawn at the player's position
			signZ = 1;
		}
		Vector3 randomSpawn = new Vector3 (Random.Range (15, 20) * signX, 0, Random.Range (15, 20) * signZ);
		if (currentStage > 1 && Random.Range(0, 5) == 0) {
			// only spawn aliens once the player is past the first 2 stages (start and elimination)
			// random(0,5) == 0 gives a 1/5 chance that the enemy spawned is an alien
			//spawn an alien sometimes
			var alienCopy = Instantiate(alien, player.transform.position + randomSpawn, transform.rotation);
			alienCopy.GetComponent<AlienController> ().ocean = ocean;
			alienCopy.GetComponent<Health> ().OnDeath += DeadEnemy;
		} else {
			var copy = Instantiate (pirate, player.transform.position + randomSpawn, transform.rotation);
			copy.GetComponent<BoatController> ().ocean = ocean;
			copy.GetComponent<Health> ().OnDeath += DeadEnemy;
		}

	}

	void putWaypointsForRemainingEnemies ()
	{
		
		BoatAI[] pirates = FindObjectsOfType<BoatAI> ();
		Debug.Log (string.Format("Spawning {0} arrows", pirates.Length));
		foreach (BoatAI boat in pirates) {
			var arrowCopy = Instantiate (arrow.gameObject);
			arrowCopy.GetComponent<Waypoint> ().setTarget (boat.gameObject);
			arrowCopy.GetComponent<Waypoint> ().setPlayer (player);
		}
	}

	void DeadEnemy()
	{
		numDeadEnemies++;
	}

	void printMessage(string message)
	{
		// prints the message to the screen
		// this is for game info
		uiText.text = message;
		Debug.Log(message);
	}

	void shoutMessage(string message)
	{
		// shout the message as if the crew are saying it
		// this is for more flavour text
		var textCopy = Instantiate(crewText, player.transform.position, cameraTrans.rotation);
		textCopy.text = message;
	}

	public void advanceLevel()
	{
		shoutMessage (gameStages [currentStage].winMessage);
		currentStage++;
		printMessage (gameStages [currentStage].stateMessage);
		initWinState ();
	}

	public void resetLevel()
	{
		playerDead = true;

		// kill all current enemies and restart the level
		BoatAI[] enemyPirates = FindObjectsOfType<BoatAI> ();
		if (enemyPirates != null) {
			foreach (var boat in enemyPirates) {
				boat.dieFromReset ();
			}
		}
		AlienController[] aliens = FindObjectsOfType<AlienController>();
		if (aliens != null) {
			foreach (var a in aliens) {
				a.dieFromReset ();
			}
		}
		Island spawnedIsland = FindObjectOfType<Island> ();
		if (spawnedIsland != null) {
			spawnedIsland.resetIsland ();
		}

		// reset variables
		waypointsNotSpawned = true;
		initWinState ();
	}
		
	public void playerRevived()
	{
		// called by the player when they have revived
		playerDead = false;
	}

	void initWinState()
	{
		WinState currentWinState = gameStages [currentStage].winState;
		tipDisplayed = false;

		switch (currentWinState) {

		case WinState.Elimination:
			// reset the number of spawned enemies so it doens't interfere
			numSpawnedEnemies = 0;
			numDeadEnemies = 0;
			break;

		case WinState.aToB:
			// set point a to the player's position
			// Don't really use this for anything... but it is useful for debugging
			gameStages [currentStage].pointA = player.transform.position;
			// create a waypoint in the world at the specified point relative to the player's current position
			var wayClone = Instantiate (wayPoint, player.transform.position + gameStages [currentStage].pointB, transform.rotation);
			wayClone.GetComponent<Island> ().ocean = ocean;
			wayClone.GetComponent<Island> ().gameplay = this;
			Waypoint arrowClone = Instantiate (arrow);
			arrowClone.setPlayer (player);
			arrowClone.setTarget (wayClone);

			break;

		case WinState.Survival:
			gameStages [currentStage].timeToSurvive = gameStages [currentStage].originalTimeToSurvive;
			timeBetweenSpawns = gameStages [currentStage].survivalTimeBetweenSpawns;
			gameStages [currentStage].originalTimeToSurvive = gameStages[currentStage].timeToSurvive;
			break;

		case WinState.End:
			// spawn the end message/credits
			break;
		}
	}

}
