using UnityEngine;

// Босс-корова в Level3. Ассет: Assets/Data/Enemies/Cow_Stats
[CreateAssetMenu(fileName = "Cow_Stats", menuName = "Eggscape/Boss Stats")]
public class BossStats : ScriptableObject
{
    public int maxHp = 10;                 // попаданий до победы
    public float projectileSpeed = 8f;     // м/с, скорость помидора
    public float startDelay = 2f;          // с от старта до первой атаки
    public float telegraphTime = 0.8f;     // с дрожи перед залпом
    [Range(0f, 1f)]
    public float phase2At = 0.5f;          // доля жизней, с которой начинается фаза 2

    [Header("Фаза 1")]
    public int phase1Count = 3;            // помидоров в веере
    public float phase1Spread = 30f;       // градусов в каждую сторону
    public float phase1Pause = 3f;         // с паузы после залпа

    [Header("Фаза 2")]
    public int phase2Count = 5;
    public float phase2Spread = 50f;
    public float phase2Pause = 1.5f;
}
