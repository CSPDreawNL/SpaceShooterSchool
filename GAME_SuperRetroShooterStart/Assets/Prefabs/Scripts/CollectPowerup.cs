using UnityEngine;

public class CollectPowerup : MonoBehaviour
{
	private AudioSource      powerupAudio;
	private CircleCollider2D powerupCollider;
	private Renderer         powerupRenderer;

	// Haalt de componenten van de power up op en zorgt ervoor dat het script ze kan gebruiken als nodig.
	void Start()
	{
		powerupAudio    = gameObject.GetComponent<AudioSource>();
		powerupCollider = gameObject.GetComponent<CircleCollider2D>();
		powerupRenderer = gameObject.GetComponent<Renderer>();
	}

	// manipuleerd de componenten van de power up om hem zo onzichtbaar te maken en het geluid af te kunnen laten spelen
	// en daarna de power up zelf kapot te maken.
	public void PowerupCollected()
	{
		powerupCollider.enabled = false;
		powerupRenderer.enabled = false;
		powerupAudio.Play();
		Destroy(gameObject, powerupAudio.clip.length);
	}
}
