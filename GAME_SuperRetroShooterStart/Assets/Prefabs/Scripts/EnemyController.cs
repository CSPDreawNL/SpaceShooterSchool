using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public GameObject powerUp;
	public GameObject explosion;
	public GameObject bullet;
	public float minReloadTime = 1.0f;
	public float maxReloadTime = 2.0f;

    SimplePool.ObjectPool bulletPool;

    // Zorgt ervoor dat de enemies kunnen beginnen met schieten, en checked of de pools aan staan zodat een enemy bullet
    // pool geregeld kan worden.
    private void Start()
    {
        StartCoroutine(Shoot());

        if (GameManager.Instance.useObjectPool)
        {
            bulletPool = gameObject.AddComponent<SimplePool.ObjectPool>() as SimplePool.ObjectPool;
            bulletPool.pooledObject = bullet;
            bulletPool.autoExpand = true;
        }
    }

    // Zorgt ervoor dat enemies kunnen schieten, en dat die wacht op een reload tijd binnen de 1 en 2 seconden.
    // Gebruikt de pool met enemy bullets
    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(Random.Range(minReloadTime, maxReloadTime));
        while (true)
        {
            if (GameManager.Instance.useObjectPool)
            {
                bulletPool.GetPooledObject(gameObject.transform.position, gameObject.transform.rotation);
                yield return new WaitForSeconds(Random.Range(minReloadTime, maxReloadTime));
            }
        }
    }

    // Checked of hij uit de collider van de boundaries gaat (behalve de bovenste) en geeft daarna door aan de functie
    // die de enemy kapot maakt.
    private void OnTriggerExit2D(Collider2D collision)
    {
        Boundary boundary = collision.gameObject.GetComponent<Boundary>() as Boundary;
        if (boundary != null && boundary.location != Boundary.BoundaryLocation.TOP)
        {
            DestroyEnemy();
        }
    }

    // Checked of die de villder van de player bullet in gaat. Als dit zo is geeft hij door dat de ScoreManager een punt
    // erbij mag zetten, en doet hij een chance roll met 10% kans om een power up te spawnen. Explodeerd daarna en
    // returned eerst de player bullet naar zijn pool, om vervolgens zelf "kapot" verklaard te worden.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBulletTag>())
        {
            ScoreManager.Instance.AddToScore();

            float randomNumber = Random.Range(0.0f, 10.0f);
            if (randomNumber > 9.0f)
            {
                Instantiate(powerUp, gameObject.transform.position, gameObject.transform.rotation);
            }

            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);

            SimplePool.PoolItem.ReturnToPoolOrDestroy(collision.gameObject, GameManager.Instance.useObjectPool);
            DestroyEnemy();
        }
    }

    // Geeft door aan het poolscript om deze enemy terug in de pool te gooien.
    private void DestroyEnemy()
    {
        SimplePool.PoolItem.ReturnToPoolOrDestroy(gameObject, GameManager.Instance.useObjectPool);
    }
}
