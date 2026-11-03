using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float lifeTime = 6f;

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.CompareTag("Player")) return;

        OnHitPlayer(c.gameObject.GetComponent<Health>());
        Destroy(gameObject);
    }

    protected virtual void OnHitPlayer(Health player)
    {
        player.TakeDamage(1);
    }
}
