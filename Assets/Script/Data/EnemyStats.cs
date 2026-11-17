using UnityEngine;

// Числа, которые есть у любого врага. Наследники добавляют своё:
// PatrollerStats (лошадь), ShooterStats (лама), ChargerStats (свинья).
// Сам этот тип нужен старой лошади в Level2 (скрипт Patrol): ей нужна только скорость.
[CreateAssetMenu(fileName = "Enemy_Stats", menuName = "Eggscape/Enemy Stats (basic)")]
public class EnemyStats : ScriptableObject
{
    [Header("Общее")]
    public int maxHp = 3;            // попаданий пулей до смерти
    public float moveSpeed = 2f;     // м/с, спокойная ходьба (патруль, возврат домой)
    public float turnSpeed = 360f;   // градусов в секунду, поворот к игроку

    [Header("Зрение")]
    public float viewRange = 8f;     // м, дальность
    public float fov = 110f;         // градусов, ширина конуса
    public float eyeHeight = 1f;     // м, глаза над ногами (ниже укрытий 1.8 м)
}
