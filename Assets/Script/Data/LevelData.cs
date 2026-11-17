using System.Collections.Generic;
using UnityEngine;

public enum LevelGoal { CollectCoins, DefeatBoss }

// Один враг, которого уровень создаст при старте
[System.Serializable]
public class EnemySpawn
{
    public GameObject prefab;     // Enemy_Horse / Enemy_Llama / Enemy_Pig
    public Vector3 position;      // где появится (мировые координаты)
    public float rotationY;       // куда смотрит, градусы
    public Vector3[] route;       // только для лошади: точки маршрута
}

// Всё, что делает уровень лёгким или сложным. Карта (платформы) - в сцене.
// Ассеты: Assets/Data/Levels/Level_01, Level_02, Level_03. Читает LevelLoader.
[CreateAssetMenu(fileName = "Level_01", menuName = "Eggscape/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Какой это уровень")]
    public int number = 1;                 // 1, 2, 3 - этим номером уровень записан в save.json
    public string title = "Meadow";        // название на кнопке выбора уровня
    public string sceneName = "Level1";    // сцена этого уровня
    public string nextScene = "Level2";    // что загрузить после победы

    [Header("Цель и время")]
    public LevelGoal goal = LevelGoal.CollectCoins;
    public int coinsRequired = -1;         // сколько монет нужно для финиша; -1 = все на уровне
    public float timeLimit = 90f;          // с; что осталось на финише - идёт в очки
    public float fallY = -10f;             // ниже этой высоты яйцо упало - рестарт

    [Header("Дождь опасностей (Spawner)")]
    public Hazard[] hazardPrefabs;         // что падает; пусто = ничего не падает
    public float hazardFirstDelay = 1f;    // с до первого
    public float hazardInterval = 3f;      // с между падениями
    public float hazardRange = 5f;         // м вокруг яйца
    public float hazardHeight = 10f;       // м над яйцом
    public float hazardStopZ = 1000f;      // дальше этой z ничего не падает

    [Header("Враги")]
    public List<EnemySpawn> enemies = new List<EnemySpawn>();

    [Header("Очки за уровень")]
    public int pointsPerCoin = 100;
    public int pointsPerHeart = 200;       // за каждую оставшуюся жизнь
    public int pointsPerSecondLeft = 10;   // за каждую секунду до timeLimit
}
