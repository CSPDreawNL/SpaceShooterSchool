using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour
{
	[Header("Movement:")]
	public float             moveSpeed                   = 10.0f; // The players movement speed

	[Header("Weapons:")]
	public GameObject        playerBullet;                        // Reference to the players bullet prefab
	public GameObject        startWeapon;                         // The players initial 'turret' gameobject
	public List<GameObject>  tripleShotTurrets;                   //
	public List<GameObject>  wideShotTurrets;                     // References to the upgrade weapon turrets
	public List<GameObject>  scatterShotTurrets;                  //
	public List<GameObject>  activePlayerTurrets;                 //
	public float             scatterShotTurretReloadTime = 2.0f;  // Reload time for the scatter shot turret!

	[Header("Effects:")]
	public GameObject        explosion;                           // Reference to the Explosion prefab
	public ParticleSystem    playerThrust;                        // The particle effect for the ships thruster

	[Header("Debug:")]
	public bool              godMode                     = false; // Set to true to enable god mode (no game over)
	public int               upgradeState                = 0;     // A reference to the upgrade state of the player
	
	// private stuff
	private Rigidbody2D      playerRigidbody;                     // The players rigidbody: Required to apply directional force to move the player
	private Renderer         playerRenderer;                      // The Renderer for the players ship sprite
	private CircleCollider2D playerCollider;                      // The Players ship collider
	private AudioSource      shootSoundFX;                        // The player shooting sound effect

	SimplePool.ObjectPool bulletPool;

	// haalt alle componenten op, en begint de lijst met actieve turrets. Checked ook gelijk of de pool aan staat
	// en voegt deze toe voor de player bullets.
	void Start()
	{
		playerCollider      = gameObject.GetComponent<CircleCollider2D>();
		playerRenderer      = gameObject.GetComponent<Renderer>();
        activePlayerTurrets = new List<GameObject>{ startWeapon };
        shootSoundFX        = gameObject.GetComponent<AudioSource>();
		playerRigidbody     = GetComponent<Rigidbody2D>();

		if (GameManager.Instance.useObjectPool)
        {
			bulletPool = gameObject.AddComponent<SimplePool.ObjectPool>() as SimplePool.ObjectPool;
			bulletPool.pooledObject = playerBullet;
			bulletPool.autoExpand = true;
		}
	}

	// Voor schieten, bewegen, en er voor zorgen dat hij niet uit de boundaries kan.
    private void Update()
    {
		// Schoot
		if (Input.GetKeyDown("space"))
        {
			Shoot(activePlayerTurrets);
        }

		// Movement rigidbody using forces
		float xDir = Input.GetAxis("Horizontal");
		float yDir = Input.GetAxis("Vertical");
		playerRigidbody.velocity = new Vector2(xDir * moveSpeed, yDir * moveSpeed);

		// Clamp ridigbody position between boundaries
		playerRigidbody.position = BoundaryManager.Instance.Clamp(playerRigidbody.position);
	}

	// Controleerd eerst of die de power up collider in gaat, en triggered dat dat hij collected wordt, en upgrade de
	// wapens. Daarna checked de speler als die in een collider gaat, of het die van de enemies of van hun bullets is.
	// In beide gevalen checked die of god mode aan staat. Zo niet, dan wordt de game over getriggered.
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.GetComponent<PowerupTag>())
        {
			CollectPowerup powerup = collision.gameObject.GetComponent<CollectPowerup>();
			powerup.PowerupCollected();

			UpgradeWeapons();
        }
        
		else if (collision.gameObject.GetComponent<EnemySmallBulletTag>() ||
				 collision.gameObject.GetComponent<EnemyType1Tag>() ||
				 collision.gameObject.GetComponent<EnemyType2Tag>())
		{
		     if (!godMode)
			 {
				ActivateGameOver();
			 }
		}
    }

	// Upgrade wapens naarmate je power ups op pakt. Pretty straight forward. Voegt eerst toe, en gooit vervolgends het
	// nummer omhoog.
	void UpgradeWeapons()
    {
		if (upgradeState == 0)
        {
			foreach (GameObject turret in tripleShotTurrets)
            {
				activePlayerTurrets.Add(turret);
            }
        }
		else if (upgradeState == 1)
		{
			foreach (GameObject turret in wideShotTurrets)
			{
				activePlayerTurrets.Add(turret);
			}
		}
		else if (upgradeState == 2)
		{
			StartCoroutine(ActivateScatterShotTurret());
		}
		else
        {
			return;
        }
		upgradeState++;
	}

	// Activeerd de Scatter Shot Turret, en zrgt ervoor dat die om de paar seconde automatisch schiet, naarmate de reload
	// tijd.
	IEnumerator ActivateScatterShotTurret()
    {
		while (true)
        {
            Shoot(scatterShotTurrets);
			yield return new WaitForSeconds(scatterShotTurretReloadTime);
        }
    }

	// De Shoot functie voor als je spatie in drukt. Checked eerst hoeveel actieve turrets je hebt, en zorgt ervoor dat hij 
	// met alle actieve turrets schiet zodra je spatie een keer indrukt. Speelt ook een geluidje af per keer dat je schiet.
	void Shoot(List<GameObject> turrets)
    {
		foreach (GameObject turret in turrets)
        {
			if (GameManager.Instance.useObjectPool)
			{
				bulletPool.GetPooledObject(turret.transform.position, turret.transform.rotation);
			}
			else
			{
				Instantiate(playerBullet, turret.transform.position, turret.transform.rotation);
			}
		}
		shootSoundFX.Play();
    }

	// Activeerd de game over, en zet componented van de speled uit, zorgt voor een explosie op de speler, en destroyed hem
	// nadat de explosies klaar zijn.
    void ActivateGameOver()
	{
		GameManager.Instance.ShowGameOver();  // If the player is hit by an enemy ship or laser it's game over.
		playerRenderer.enabled = false;       // We can't destroy the player game object straight away or any code from this point on will not be executed
		playerCollider.enabled = false;       // We turn off the players renderer so the player is not longer displayed and turn off the players collider
		playerThrust.Stop();
		Instantiate(explosion, transform.position, transform.rotation);   // Then we Instantiate the explosions... one at the centre and some additional around the players location for a bigger bang!
		for (int i = 0; i < 8; i++)
		{
			Vector3 randomOffset = new Vector3(transform.position.x + Random.Range(-0.6f, 0.6f), transform.position.y + Random.Range(-0.6f, 0.6f), 0.0f);
			Instantiate(explosion, randomOffset, transform.rotation);
		}
		Destroy(gameObject, 1.0f); // The second parameter in Destroy is a delay to make sure we have finished exploding before we remove the player from the scene.
	}
}
