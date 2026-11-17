using UnityEngine;

// Лошадь-патрульный. Ассет: Assets/Data/Enemies/Horse_Stats
[CreateAssetMenu(fileName = "Horse_Stats", menuName = "Eggscape/Patroller Stats")]
public class PatrollerStats : EnemyStats
{
    [Header("Погоня")]
    public float chaseSpeed = 3.5f;   // м/с (после плейтеста 10: было 4.5)
    public float loseTime = 2f;       // с без видимости -> назад на маршрут

    [Header("Атака")]
    public float attackRange = 2.6f;  // м между центрами: с этого расстояния начинает замах
    public float windUp = 0.5f;       // с, красный "!" перед ударом
    public float hitReach = 3.2f;     // м, удар попадёт, только если игрок ещё ближе
    public float recoverTime = 0.8f;  // с отдыха после удара - окно игрока
    public int damage = 1;            // жизней за удар
}
