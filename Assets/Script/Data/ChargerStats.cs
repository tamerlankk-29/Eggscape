using UnityEngine;

// Свинья-таран. Ассет: Assets/Data/Enemies/Pig_Stats
[CreateAssetMenu(fileName = "Pig_Stats", menuName = "Eggscape/Charger Stats")]
public class ChargerStats : EnemyStats
{
    [Header("Замах")]
    public float triggerRange = 7f;      // м, с какого расстояния начинает замах
    public float windUp = 0.8f;          // с, дрожь + красная полоса (было 0.7)
    public float lockTime = 0.35f;       // с до конца замаха, когда полоса замирает (было 0.2)
    public float watchTurnSpeed = 180f;  // градусов в секунду: следит глазами в Idle

    [Header("Рывок")]
    public float dashSpeed = 12f;        // м/с
    public float dashDistance = 9f;      // м
    public float hitRadius = 2f;         // м между центрами: полсвиньи + яйцо + запас
    public float knockback = 4f;         // толчок яйца вперёд (больше - может сбросить с арены)
    public float knockUp = 2f;           // толчок яйца вверх
    public int damage = 1;

    [Header("После атаки")]
    public float stunTime = 1.2f;        // с оглушения - окно игрока
    public float cooldown = 1.5f;        // с до следующего замаха
}
