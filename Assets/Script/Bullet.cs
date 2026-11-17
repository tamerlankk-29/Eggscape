using UnityEngine;

// Пуля яйца. Скорость и время жизни передаёт Shooter из PlayerStats.
public class Bullet : MonoBehaviour
{
    // НОВОЕ: вместо Start с числами 12 и 3
    public void Launch(float speed, float lifeTime)
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Boss boss = other.GetComponentInParent<Boss>();
        if (boss != null)
        {
            boss.TakeHit();
            Destroy(gameObject);
            return;
        }

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeHit();
            Destroy(gameObject);
            return;
        }

        Hazard h = other.GetComponent<Hazard>();
        if (h != null && !(h is Heart))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
