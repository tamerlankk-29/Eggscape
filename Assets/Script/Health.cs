using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Жизни героя: урон, лечение, неуязвимость с миганием, рестарт при смерти.
public class Health : MonoBehaviour
{
    const float BlinkStep = 0.2f;     // только картинка: яйцо мигает каждые 0.2 с
    const float DeathDelay = 0.7f;    // НОВОЕ (12): пауза после смерти, чтобы звук смерти был слышен

    public PlayerStats stats;         // НОВОЕ: тот же Player_Stats
    public AudioClip hurtClip;
    public AudioClip healClip;
    [HideInInspector] public int hp;  // текущие жизни: живут на яйце, а не в ассете

    bool invincible;
    bool dead;                        // НОВОЕ (12): умерли и ждём перезапуска
    Renderer[] rends;
    AudioSource src;

    void Awake()
    {
        hp = stats.maxHp;             // НОВОЕ: полные жизни из ассета
    }

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        src = GetComponent<AudioSource>();
    }

    public void TakeDamage(int dmg)
    {
        if (invincible || dead) return;
        hp -= dmg;
        if (hp <= 0)
        {
            GameOver();
            return;
        }
        src.PlayVaried(hurtClip);     // НОВОЕ (12): с вариацией высоты
        StartCoroutine(Invincibility());
    }

    public void Heal(int amount)
    {
        if (dead) return;
        hp = Mathf.Min(hp + amount, stats.maxHp);   // НОВОЕ: максимум из ассета
        src.PlayVaried(healClip);
    }

    void SetVisible(bool v)
    {
        foreach (Renderer r in rends) r.enabled = v;
    }

    // НОВОЕ: длительность из stats.invincibleTime, а не "10 раз по 0.2"
    IEnumerator Invincibility()
    {
        invincible = true;
        bool visible = true;
        for (float t = 0f; t < stats.invincibleTime; t += BlinkStep)
        {
            visible = !visible;
            SetVisible(visible);
            yield return new WaitForSeconds(BlinkStep);
        }
        SetVisible(true);
        invincible = false;
    }

    // НОВОЕ (12): звук смерти -> 0.7 с -> перезапуск через LevelLoader (там звук проигрыша)
    void GameOver()
    {
        dead = true;
        SoundManager.Death();
        StartCoroutine(RestartAfterDeath());
    }

    IEnumerator RestartAfterDeath()
    {
        yield return new WaitForSeconds(DeathDelay);
        if (LevelLoader.Current != null) LevelLoader.Current.Restart();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);   // старая сцена без LevelLoader
    }
}
