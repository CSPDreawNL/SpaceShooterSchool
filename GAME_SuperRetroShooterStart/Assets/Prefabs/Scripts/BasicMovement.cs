using UnityEngine;

public class BasicMovement : MonoBehaviour
{
	public  float speed;

    // Zorgt ervoor dat de eerste enemy de basis movement naar benenen kan gaan, zelfde voor de bullets en de power ups.
    private void OnEnable()
    {
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.up * speed;
    }
}
