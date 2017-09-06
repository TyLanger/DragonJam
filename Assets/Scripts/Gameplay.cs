using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour {

	public enum WinState {Start, Elimination, aToB, Survival, End};

	[System.Serializable]
	public struct GameStage {

		public WinState winState;

		public string stateMessage;
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
	public GameObject wayPoint;
	public Light sunLight;

	float timeOfNextSpawn = 0;
	float timeBetweenSpawns = 10;

	int numSpawnedEnemies = 0;
	public int numDeadEnemies;

	public GameStage[] gameStages;
	int currentStage = 0;

	// Use this for initialization
	void Start () {
		numDeadEnemies = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameStages[currentStage].winState != WinState.Start && gameStages[currentStage].winState != WinState.End) {
			if (Time.time > timeOfNextSpawn) {
				timeOfNextSpawn = Time.time + timeBetweenSpawns;

				// right now, elimination only works for 1 enemy type
				if ((gameStages [currentStage].winState == WinState.Elimination) && (gameStages [currentStage].enemiesToKill [0] <= numSpawnedEnemies)) {
					// all the enemies for elimination have been spawned
					if (numDeadEnemies == gameStages [currentStage].enemiesToKill [0]) {
						advanceLevel ();
					}
				} else {
					spawnEnemy ();
					numSpawnedEnemies++;
				}
			}
		}

		if (gameStages [currentStage].winState == WinState.Survival) {
			sunLight.intensity = gameStages[currentStage].lightIntensity.Evaluate((gameStages [currentStage].timeToSurvive/gameStages[currentStage].originalTimeToSurvive));
			gameStages [currentStage].timeToSurvive -= Time.deltaTime;
			if (gameStages [currentStage].timeToSurvive < 0) {
				advanceLevel ();
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

	void DeadEnemy()
	{
		numDeadEnemies++;
	}

	public void advanceLevel()
	{
		Debug.Log (gameStages [currentStage].winMessage);
		currentStage++;
		Debug.Log (gameStages [currentStage].stateMessage);
		initWinState ();
	}

	void initWinState()
	{
		WinState currentWinState = gameStages [currentStage].winState;


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

			break;

		case WinState.Survival:
			timeBetweenSpawns = gameStages [currentStage].survivalTimeBetweenSpawns;
			gameStages [currentStage].originalTimeToSurvive = gameStages[currentStage].timeToSurvive;
			break;

		case WinState.End:
			// spawn the end message/credits
			break;
		}
	}
}
