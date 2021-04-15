using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	//--------------------------------------------------------------------------------
	// Make sure this class is a singleton
	//--------------------------------------------------------------------------------
	private static GameManager instance;
	public static GameManager Instance { get => instance; }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
			Destroy(this);
	}

	private void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	//--------------------------------------------------------------------------------
	// Class implementation
	//--------------------------------------------------------------------------------
	[Header("UI:")]
	public TextMeshProUGUI   gameOverLabel;
	public Button			 restartGameButton;

	[Header("Enemy objects:")]
	public GameObject enemyType1;
	public GameObject enemyType2;

	[Header("Enemy config:")]
	public float startWait      = 1.0f;
	public float waveInterval   = 2.0f;
	public float spawnInterval  = 0.5f;
	public int   enemy1PerWave  = 5;
	public int	 enemy2PerWave  = 3;

	[Header("Debug:")]
	public bool useObjectPool = true;

	// Pools en voor spawnen enemies.
	SimplePool.ObjectPool enemyPool1;
	SimplePool.ObjectPool enemyPool2;

	private bool spawnEnemies = false;

	// Enemies aanzetten, beginnen met enemies te spawnen en pools gebruiken als deze aan staan.
	private void Start()
    {
		spawnEnemies = true;
		StartCoroutine(SpawnEnemyWaves());
		
		if (GameManager.Instance.useObjectPool)
		{
			enemyPool1 = gameObject.AddComponent<SimplePool.ObjectPool>() as SimplePool.ObjectPool;
			enemyPool2 = gameObject.AddComponent<SimplePool.ObjectPool>() as SimplePool.ObjectPool;
			enemyPool1.pooledObject = enemyType1;
			enemyPool2.pooledObject = enemyType2;
			enemyPool1.autoExpand = true;
			enemyPool2.poolSize = 3;
		}
	}

	// Spawnen van enemies, eerst ervoor zorgen dat de boundaries goed gezet zijn, en dan beide pools beginnen gebruiken
	// over tijd. De eerste for-loop is voor de eerste enemies, de 2de voor het 2de enemy (anders wordt het te druk
	// op het scherm.
	IEnumerator SpawnEnemyWaves()
    {
		if (BoundaryManager.Instance.TopLeft() == Vector3.zero || BoundaryManager.Instance.TopRight() == Vector3.zero)
        {
			yield return new WaitForSeconds(0.01f);
        }

		Vector3 topLeft = BoundaryManager.Instance.TopLeft();
		Vector3 topRight = BoundaryManager.Instance.TopRight();

		yield return new WaitForSeconds(startWait);

		while (spawnEnemies == true)
        {
			for (int i = 0 ; i < enemy1PerWave ; i++)
            {
				Vector3 spawnPosition = new Vector3(Random.Range(topLeft.x, topRight.x), topLeft.y, 0.0f);
				Quaternion spawnRotation = Quaternion.Euler(0, 0, 180);

                if (useObjectPool)
                {
                    enemyPool1.GetPooledObject(spawnPosition, spawnRotation);
				}
				yield return new WaitForSeconds(spawnInterval);
			}
			for (int i = 0 ; i < enemy2PerWave ; i++)
			{
				Vector3 spawnPosition = new Vector3(Random.Range(topLeft.x, topRight.x), topLeft.y, 0.0f);
				Quaternion spawnRotation = Quaternion.Euler(0, 0, 180);

				if (useObjectPool)
				{
					enemyPool2.GetPooledObject(spawnPosition, spawnRotation);
				}
				yield return new WaitForSeconds(spawnInterval);
			}
		}
    }

	// Als de speler dood is gegaan, wordt dit blok opgeroepen om zo enemies uit te zetten, en de game over op het scherm
	// te zetten.
    public void ShowGameOver()
	{
		spawnEnemies = false;
		gameOverLabel.rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
		restartGameButton.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -50, 0);
	}

	// Restarten als op de restasrt knop gedrukt is.
	public void RestartGame()
	{
		SceneManager.LoadScene("GameScene");
	}
}
