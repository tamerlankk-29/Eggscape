using UnityEngine;

// Лама-стрелок. Ассет: Assets/Data/Enemies/Llama_Stats
// Урон плевка - в ассете Spit_Stats (его читает сам плевок).
[CreateAssetMenu(fileName = "Llama_Stats", menuName = "Eggscape/Shooter Stats")]
public class ShooterStats : EnemyStats
{
    [Header("Дистанция")]
    public float preferredMin = 4f;   // м, ближе - пятится
    public float preferredMax = 8f;   // м, дальше - идёт навстречу
    public float backStep = 3f;       // м, на сколько отходит за раз
    public float aimRange = 9f;       // м, с какого расстояния начинает целиться
    public float loseTime = 3f;       // с без видимости -> Idle

    [Header("Выстрел")]
    public float aimTime = 0.6f;      // с, красный лазер перед выстрелом
    public float shotSpeed = 5f;      // м/с (после плейтеста 10: было 6)
    public float cooldown = 2f;       // с между выстрелами (было 1.5)
    public float laserLength = 12f;   // м, длина лазера
}
