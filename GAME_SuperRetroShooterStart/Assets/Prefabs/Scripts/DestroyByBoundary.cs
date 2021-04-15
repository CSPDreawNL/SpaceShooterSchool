using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    // Zorgt ervoor dat als dit opject de collider van de boundary uit gaat, dat die terug de object pool in gaat.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Boundary>())
        {
            SimplePool.PoolItem.ReturnToPoolOrDestroy(gameObject, GameManager.Instance.useObjectPool);
        }
    }
}
