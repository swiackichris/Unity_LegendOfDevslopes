using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	[SerializeField] GameObject player;
	[SerializeField] GameObject[] spawnPoints;
	[SerializeField] GameObject[] powerUpSpawns;
	[SerializeField] GameObject tanker;
	[SerializeField] GameObject ranger;
	[SerializeField] GameObject soldier;
	[SerializeField] GameObject arrow;
	[SerializeField] GameObject healthPowerUp;
	[SerializeField] GameObject speedPowerUp;
	[SerializeField] Text levelText;
	[SerializeField] Text endGameText;
	[SerializeField] int maxPowerUps = 4;
	[SerializeField] int finalLevel = 20;

	private bool gameOver = false;
	private int currentLevel;
	private float generatedSpawnTime = 1;
	private float currentSpawnTime = 0;
	private float powerUpSpawnTime = 60;
	private float currentPowerUpSpawnTime = 0;	
	private GameObject newEnemy;
	private int powerups = 0;
	private GameObject newPowerup;

	private List<EnemyHealth> enemies = new List<EnemyHealth>();
	private List<EnemyHealth> killedEnemies = new List<EnemyHealth>();

	public void RegisterEnemy(EnemyHealth enemy)
	{
		enemies.Add(enemy);
	}

	public void KilledEnemy(EnemyHealth enemy)
	{
		killedEnemies.Add(enemy);
	}

	public void RegisterPowerUp()
	{
		powerups++;
	}

	public bool GameOver
	{
		get {return gameOver; }
	}

		public GameObject Player
	{
		get {return player; }
	}

		public GameObject Arrow
	{
		get {return arrow; }
	}


	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		endGameText.GetComponent<Text>().enabled = false;
		StartCoroutine(spawn());
		StartCoroutine(powerUpSpawn());
		currentLevel = 1;
	}
	
	// Update is called once per frame
	void Update () {
		currentSpawnTime += Time.deltaTime;
		currentPowerUpSpawnTime += Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadScene("GameMenu");
		}
	}

	public void PlayerHit(int currentHP)
	{
		if (currentHP > 0)
		{
			gameOver = false;
		}
		else
		{
			gameOver = true;
			StartCoroutine(endGame("Defeat"));
		}
	}


	IEnumerator spawn()
	{
		// check that spawn time is greater than the current
		if(currentSpawnTime > generatedSpawnTime)
			currentSpawnTime = 0;
			// if there are less enemies on screen than the current level, randomly select a spawn point
			{
				if(enemies.Count < currentLevel)
				{
					int randomNumber = Random.Range(0, spawnPoints.Length - 1);
					GameObject spawnLocation = spawnPoints[randomNumber];
					int randomEnemy = Random.Range (0, 3);
					if(randomEnemy == 0) // Could do the same with switch and case.
					{
						newEnemy = Instantiate(soldier) as GameObject;
					}
					else if(randomEnemy == 1)
					{
						newEnemy = Instantiate(ranger) as GameObject;
					}
					else if(randomEnemy == 2)
					{
						newEnemy = Instantiate(tanker) as GameObject;
					}

					newEnemy.transform.position = spawnLocation.transform.position;
				}
				if(killedEnemies.Count == currentLevel && currentLevel != finalLevel)
				{
					enemies.Clear();
					killedEnemies.Clear();

					yield return new WaitForSeconds (3f);
					currentLevel++;
					levelText.text = "Level " + currentLevel;
				}
				if(killedEnemies.Count == finalLevel)
				{
					StartCoroutine(endGame ("Victory!"));
				}
			}

			yield return null;
			StartCoroutine(spawn());
			// and spawn a random enemy.
		// if we have killed the same number of enemies as the current level, clear out the enemies and
		// killed enemies arrays, increment the current level by 1, and start over.
	}

	IEnumerator powerUpSpawn()
	{
		if (currentPowerUpSpawnTime > powerUpSpawnTime)
		{
			currentPowerUpSpawnTime = 0;
			if(powerups < maxPowerUps)
			{
				int randomNumber = Random.Range(0, powerUpSpawns.Length - 1);
				GameObject spawnLocation = powerUpSpawns [randomNumber];
				int randomPowerUp = Random.Range(0, 2);
				if (randomPowerUp == 0)
				{
					newPowerup = Instantiate(healthPowerUp) as GameObject;
				}
				else if (randomPowerUp == 1)
				{
					newPowerup = Instantiate(speedPowerUp) as GameObject;
				}

				newPowerup.transform.position = spawnLocation.transform.position;
			}
		}
		yield return null;
		StartCoroutine(powerUpSpawn());
	}

	IEnumerator endGame(string outcome)
	{
		endGameText.text = outcome;
		endGameText.GetComponent<Text>().enabled = true;
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene("GameMenu");
	}
}
