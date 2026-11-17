using UnityEngine;

// Шипы: пока яйцо в зоне, отнимаем жизни (неуязвимость в Health не даст бить каждый кадр)
public class DamageZone : MonoBehaviour
{
    [SerializeField] HazardStats stats;   // НОВОЕ: Spikes_Stats

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Health h = other.GetComponent<Health>();
        if (h != null) h.TakeDamage(stats.amount);   // НОВОЕ: было TakeDamage(1)
    }
}
