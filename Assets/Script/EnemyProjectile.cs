using UnityEngine;

// Плевок ламы. Наследует Hazard: время жизни и урон игроку уже есть там.
// Отличие: исчезает при ЛЮБОМ столкновении (стена, земля), а не только с игроком.
public class EnemyProjectile : Hazard
{
    protected override void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Player"))
            OnHitPlayer(c.gameObject.GetComponent<Health>());   // метод из Hazard: -1 жизнь
        Destroy(gameObject);
    }
}