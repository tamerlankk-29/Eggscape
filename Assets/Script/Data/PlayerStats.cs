using UnityEngine;

// Баланс яйца. Один ассет: Assets/Data/Player_Stats.
// Читают: Player3D, Health, Shooter (и через него Bullet).
[CreateAssetMenu(fileName = "Player_Stats", menuName = "Eggscape/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Движение")]
    public float force = 14f;          // сила разгона
    public float maxSpeed = 8f;        // м/с, максимальная скорость по земле
    public float jumpForce = 7f;       // импульс прыжка (при массе 1 подлёт ~2.5 м)
    public float brake = 4f;           // торможение, когда кнопки отпущены

    [Header("Жизни")]
    public int maxHp = 3;              // жизней на старте уровня
    public float invincibleTime = 2f;  // с неуязвимости после удара
    public int contactDamage = 1;      // сколько отнимает касание объекта с тегом Enemy

    [Header("Стрельба")]
    public float shootCooldown = 0.3f; // с между выстрелами
    public float bulletSpeed = 12f;    // м/с
    public float bulletLifeTime = 3f;  // с, потом пуля исчезает
}
