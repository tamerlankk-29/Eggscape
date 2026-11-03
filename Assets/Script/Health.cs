using System.Collections;             // IEnumerator для корутин
using UnityEngine;
using UnityEngine.SceneManagement;    // SceneManager для рестарта уровня

// Жизни героя: урон, лечение, неуязвимость с миганием, рестарт при смерти.
// Висит на объекте Player.
public class Health : MonoBehaviour
{
    public int hp = 3;                // текущие жизни
    public int maxHp = 3;             // максимум жизней
    public AudioClip hurtClip;        // звук урона
    public AudioClip healClip;        // звук лечения

    bool invincible;                  // неуязвим ли сейчас (после удара)
    Renderer[] rends;                 // все видимые части яйца: скорлупа, глаза, ручки, ножки
    AudioSource src;                  // источник звука

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>(); // все Renderer на себе и на всех детях
        src = GetComponent<AudioSource>();
    }

    // Получить урон. Вызывают: Player3D (враги), Hazard (камни, бомбы, помидоры), DamageZone (шипы)
    public void TakeDamage(int dmg)
    {
        if (invincible) return;       // во время неуязвимости урон игнорируется
        hp -= dmg;                    // отнимаем жизни
        src.PlayOneShot(hurtClip);    // звук урона

        if (hp <= 0)                  // <=, потому что бомба отнимает 2
        {
            GameOver();               // жизни кончились: рестарт уровня
            return;
        }
        StartCoroutine(Invincibility()); // иначе 2 секунды неуязвимости с миганием
    }

    // Лечение. Вызывает сердечко (Heart)
    public void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, maxHp); // не больше максимума
        src.PlayOneShot(healClip);
    }

    // Показать или скрыть все части яйца
    void SetVisible(bool v)
    {
        foreach (Renderer r in rends) r.enabled = v;
    }

    // Корутина неуязвимости: 10 переключений по 0.2 секунды = 2 секунды мигания
    IEnumerator Invincibility()
    {
        invincible = true;
        bool visible = true;
        for (int i = 0; i < 10; i++)
        {
            visible = !visible;                    // переключаем видимость
            SetVisible(visible);                   // яйцо мигает
            yield return new WaitForSeconds(0.2f); // ждём 0.2 секунды, игра при этом продолжается
        }
        SetVisible(true);             // в конце точно видимы
        invincible = false;           // снова можно получить урон
    }

    // Смерть: перезагружаем текущую сцену
    void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
