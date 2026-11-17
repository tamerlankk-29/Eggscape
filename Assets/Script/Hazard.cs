using UnityEngine;

// Всё, что летит или лежит и действует на игрока при касании.
// Наследники: Rock, BombHazard, Heart, EnemyProjectile. Числа - в HazardStats.
public class Hazard : MonoBehaviour
{
    [SerializeField] protected HazardStats stats;   // НОВОЕ: Rock_Stats, Bomb_Stats, ...

    protected virtual void Start()
    {
        if (stats.lifeTime > 0f) Destroy(gameObject, stats.lifeTime);  // 0 = лежит, пока не взяли
    }

    protected virtual void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.CompareTag("Player")) return;
        OnHitPlayer(c.gameObject.GetComponent<Health>());
        Destroy(gameObject);
    }

    protected virtual void OnHitPlayer(Health player)
    {
        player.TakeDamage(stats.amount);             // НОВОЕ: было TakeDamage(1)
    }
}
