using UnityEngine;

// Жизни врага. Максимум - в ассете, текущие - на самом враге.
// Пуля игрока (Bullet) вызывает TakeHit.
public class EnemyHealth : MonoBehaviour
{
    const float EffectLifeTime = 2f;           // только картинка: сколько живёт взрыв

    [SerializeField] EnemyStats stats;         // НОВОЕ: тот же ассет, что у AI этого врага
    [SerializeField] GameObject deathEffect;   // необязательно: префаб Explosion

    int hp;                                    // текущие жизни ИМЕННО ЭТОГО врага

    void Start()
    {
        hp = stats.maxHp;                      // НОВОЕ: было поле hp = 3
    }

    public void TakeHit()
    {
        hp--;                                  // никогда не stats.maxHp-- : это ранило бы всех таких врагов
        if (hp > 0) return;
        if (deathEffect != null)
        {
            GameObject fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(fx, EffectLifeTime);
        }
        Destroy(gameObject);
    }
}
