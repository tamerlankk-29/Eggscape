using System.Collections;             // IEnumerator для корутин
using UnityEngine;
using UnityEngine.SceneManagement;    // загрузка сцены Win
using UnityEngine.UI;                 // Slider (полоса HP)

// Босс-корова: смотрит на игрока, атакует веерами помидоров с предупреждением,
// во второй фазе атакует чаще и шире, получает урон от пуль, при смерти загружает Win.
// Висит на объекте Boss в Level3.
public class Boss : MonoBehaviour
{
    public int maxHp = 8;                 // жизни босса (в Inspector сцены стоит своё значение)
    public Transform player;              // яйцо
    public GameObject projectilePrefab;   // префаб помидора (со скриптом Rock -> наносит урон)
    public Slider hpBar;                  // полоса HP на экране
    public float projectileSpeed = 8f;    // скорость помидоров
    public string winScene = "Win";       // сцена победы

    int hp;                               // текущие жизни

    void Start()
    {
        hp = maxHp;                       // полные жизни
        hpBar.maxValue = maxHp;           // настраиваем полосу HP
        hpBar.value = hp;
        StartCoroutine(AttackLoop());     // запускаем цикл атак
    }

    // Каждый кадр поворачиваемся к игроку (только по горизонтали)
    void Update()
    {
        Vector3 look = player.position - transform.position; // направление от босса к игроку
        look.y = 0f;                                          // без наклона
        if (look.sqrMagnitude > 0.01f)                        // защита от нулевого вектора
            transform.rotation = Quaternion.LookRotation(look);
    }

    // Попадание пули. Вызывает Bullet.OnTriggerEnter
    public void TakeHit()
    {
        hp--;                             // -1 HP
        hpBar.value = hp;                 // обновляем полосу
        if (hp <= 0)
            SceneManager.LoadScene(winScene); // босс побеждён -> экран победы
    }

    // Корутина атак: предупреждение -> веер помидоров -> пауза, и так по кругу
    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(2f);  // 2 секунды в начале боя
        while (true)                          // бесконечно; игра не зависает, потому что внутри есть yield
        {
            bool phase2 = hp <= maxHp / 2;    // вторая фаза: половина HP или меньше (деление целых)

            yield return Telegraph(0.8f);     // предупреждение: босс дрожит 0.8 секунды

            int count = phase2 ? 5 : 3;           // сколько помидоров
            float spread = phase2 ? 50f : 30f;    // ширина веера в градусах в каждую сторону
            Vector3 toPlayer = player.position - transform.position; // центр веера: на игрока
            toPlayer.y = 0f;
            toPlayer.Normalize();

            for (int i = 0; i < count; i++)
            {
                // угол равномерно от -spread до +spread (например -30, 0, +30); (float) нужен для дробного деления
                float angle = Mathf.Lerp(-spread, spread, i / (float)(count - 1));
                Vector3 dir = Quaternion.Euler(0f, angle, 0f) * toPlayer;      // поворачиваем направление на этот угол вокруг Y
                Vector3 pos = transform.position + dir * 2f + Vector3.up * 1f; // точка появления перед коровой, чтобы не застрять в ней
                GameObject p = Instantiate(projectilePrefab, pos, Quaternion.identity); // создаём помидор из префаба
                p.GetComponent<Rigidbody>().linearVelocity = dir * projectileSpeed;    // запускаем его
            }

            // Окно, чтобы подкатиться и выстрелить (во второй фазе короче)
            yield return new WaitForSeconds(phase2 ? 1.5f : 3f);
        }
    }

    // Предупреждение перед атакой: размер босса пульсирует на ±15%
    IEnumerator Telegraph(float time)
    {
        Vector3 s = transform.localScale;     // запоминаем нормальный размер
        float e = 0f;                         // прошедшее время
        while (e < time)
        {
            e += Time.deltaTime;
            transform.localScale = s * (1f + 0.15f * Mathf.Sin(e * 20f));
            yield return null;                // ждём следующий кадр
        }
        transform.localScale = s;             // возвращаем нормальный размер
    }
}
