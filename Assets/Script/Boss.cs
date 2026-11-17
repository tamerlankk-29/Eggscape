using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Босс-корова: смотрит на игрока, дрожит (предупреждение), стреляет веером помидоров.
// Во второй фазе чаще и шире. Все числа - в BossStats (Cow_Stats).
public class Boss : MonoBehaviour
{
    // Картинка и геометрия, не баланс
    const float ShakeAmount = 0.15f;   // ±15% размера при дрожи
    const float ShakeSpeed = 20f;
    const float SpawnForward = 2f;     // м, помидор появляется перед коровой, а не внутри
    const float SpawnUp = 1f;

    public BossStats stats;               // НОВОЕ: Cow_Stats
    public Transform player;
    public GameObject projectilePrefab;   // Tomato
    public Slider hpBar;

    int hp;
    bool dead;

    void Start()
    {
        hp = stats.maxHp;
        hpBar.maxValue = stats.maxHp;
        hpBar.value = hp;
        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(look);
    }

    public void TakeHit()
    {
        if (dead) return;
        hp--;
        hpBar.value = hp;
        if (hp <= 0)
        {
            dead = true;
            StopAllCoroutines();
            if (LevelLoader.Current != null) LevelLoader.Current.Complete();   // НОВОЕ: очки + сохранение + Win
            else Debug.LogWarning("No LevelLoader in Level3");
        }
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(stats.startDelay);
        while (true)
        {
            bool phase2 = hp <= stats.maxHp * stats.phase2At;     // НОВОЕ: было maxHp / 2

            yield return Telegraph(stats.telegraphTime);

            int count = phase2 ? stats.phase2Count : stats.phase1Count;
            float spread = phase2 ? stats.phase2Spread : stats.phase1Spread;
            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0f;
            toPlayer.Normalize();

            for (int i = 0; i < count; i++)
            {
                float angle = count > 1 ? Mathf.Lerp(-spread, spread, i / (float)(count - 1)) : 0f;
                Vector3 dir = Quaternion.Euler(0f, angle, 0f) * toPlayer;
                Vector3 pos = transform.position + dir * SpawnForward + Vector3.up * SpawnUp;
                GameObject p = Instantiate(projectilePrefab, pos, Quaternion.identity);
                p.GetComponent<Rigidbody>().linearVelocity = dir * stats.projectileSpeed;
            }

            yield return new WaitForSeconds(phase2 ? stats.phase2Pause : stats.phase1Pause);
        }
    }

    IEnumerator Telegraph(float time)
    {
        Vector3 s = transform.localScale;
        float e = 0f;
        while (e < time)
        {
            e += Time.deltaTime;
            transform.localScale = s * (1f + ShakeAmount * Mathf.Sin(e * ShakeSpeed));
            yield return null;
        }
        transform.localScale = s;
    }
}
