using UnityEngine;

// Жизни врага. Пуля игрока (Bullet) вызывает TakeHit.
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int hp = 3;               // сколько попаданий выдерживает
    [SerializeField] GameObject deathEffect;   // необязательно: префаб Explosion

    public void TakeHit()
    {
        hp--;
        if (hp > 0) return;
        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}