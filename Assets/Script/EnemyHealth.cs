using UnityEngine;

// Жизни врага. Пуля игрока (Bullet) вызывает TakeHit.
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int hp = 3;               // сколько попаданий выдерживает
    [SerializeField] GameObject deathEffect;   // необязательно: префаб Explosion
    [SerializeField] float effectLifeTime = 2f; // секунд, потом взрыв удаляется из сцены

    public void TakeHit()
    {
        hp--;
        if (hp > 0) return;
        if (deathEffect != null)
        {
            GameObject fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(fx, effectLifeTime);       // иначе взрыв остаётся в сцене навсегда
        }
        Destroy(gameObject);
    }
}