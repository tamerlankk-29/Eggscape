using UnityEngine;

public class BombHazard : Hazard
{
    public GameObject explosionPrefab;
    bool armed;

    protected override void OnHitPlayer(Health player)
    {
        player.TakeDamage(2);
    }

    protected override void OnCollisionEnter(Collision c)
    {
        base.OnCollisionEnter(c);

        if (!armed && !c.gameObject.CompareTag("Player"))
        {
            armed = true;
            Invoke(nameof(Explode), 2f);
        }
    }

    void Explode()
    {
        GameObject fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(fx, 0.6f);
        Destroy(gameObject);
    }
}
