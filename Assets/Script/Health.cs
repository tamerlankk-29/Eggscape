using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Жизни героя: урон, лечение, неуязвимость с миганием, рестарт при смерти.
public class Health : MonoBehaviour
{
    const float BlinkStep = 0.2f;     // только картинка: яйцо мигает каждые 0.2 с

    public PlayerStats stats;         // НОВОЕ: тот же Player_Stats
    public AudioClip hurtClip;
    public AudioClip healClip;
    [HideInInspector] public int hp;  // текущие жизни: живут на яйце, а не в ассете

    bool invincible;
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
        if (invincible) return;
        hp -= dmg;
        src.PlayOneShot(hurtClip);
        if (hp <= 0)
        {
            GameOver();
            return;
        }
        StartCoroutine(Invincibility());
    }

    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, stats.maxHp);   // НОВОЕ: максимум из ассета
        src.PlayOneShot(healClip);
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

    void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
