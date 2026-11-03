using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;

    void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        Destroy(gameObject, 3f);
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

        Hazard h = other.GetComponent<Hazard>();
        if (h != null && !(h is Heart))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
