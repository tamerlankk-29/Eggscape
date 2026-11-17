using UnityEngine;

// Бомба: бьёт при касании (урон из Bomb_Stats), а упав на землю - взрывается через fuseTime.
public class BombHazard : Hazard
{
    const float ExplosionFxTime = 0.6f;   // только картинка: сколько виден взрыв

    public GameObject explosionPrefab;
    bool armed;

    protected override void OnCollisionEnter(Collision c)
    {
        base.OnCollisionEnter(c);
        if (!armed && !c.gameObject.CompareTag("Player"))
        {
            armed = true;
            Invoke(nameof(Explode), stats.fuseTime);   // НОВОЕ: было 2f
        }
    }

    void Explode()
    {
        GameObject fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(fx, ExplosionFxTime);
        Destroy(gameObject);
    }
}
