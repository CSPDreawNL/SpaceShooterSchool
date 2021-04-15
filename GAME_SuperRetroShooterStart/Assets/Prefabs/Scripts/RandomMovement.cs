using UnityEngine;

public class RandomMovement : MonoBehaviour
{
	public float speed = 2.0f;
	private float tChange = 0.0f;
	private float randomX;
	private float randomY;

    // Zorgt ervoor dat enemy 2 op random door het scherm heen beweegt, en niet gewoon normaal naar beneden komt vallen.
    // Deze kan alleen binnen de boundaries terecht komen.
    private void Update()
    {
        if (Time.time >= tChange)
        {
            randomX = Random.Range(-2.0f, 2.0f);
            randomY = Random.Range(-2.0f, 2.0f);

            tChange = Time.time + Random.Range(0.5f, 1.5f);
        }
        Vector3 newPosition = new Vector3(randomX, randomY, 0f);
        transform.Translate(newPosition * speed * Time.deltaTime);

        if (!BoundaryManager.Instance.WithinBoundaryX(transform.position.x))
        {
            randomX = -randomX;
        }
        if (!BoundaryManager.Instance.WithinBoundaryY(transform.position.y))
        {
            randomY = -randomY;
        }

        transform.position = BoundaryManager.Instance.Clamp(transform.position);
    }
}