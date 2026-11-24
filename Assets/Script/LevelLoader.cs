using UnityEngine;
using UnityEngine.SceneManagement;

// Один и тот же скрипт в Level1, Level2, Level3. Строит уровень по ассету LevelData:
// враги и их точки, дождь опасностей, цель, время, падение, очки, сохранение при победе.
public class LevelLoader : MonoBehaviour
{
    // Текущий уровень: Player3D, Boss и Hud находят его без поиска по сцене
    public static LevelLoader Current { get; private set; }

    [SerializeField] LevelData level;     // Level_01 / Level_02 / Level_03
    [SerializeField] Spawner spawner;     // объект Spawner из сцены; в Level3 оставь пустым

    public LevelData Level => level;
    public float TimeLeft => Mathf.Max(0f, level.timeLimit - elapsed);

    Player3D player;
    Health health;
    float elapsed;        // секунд с начала уровня (пауза его останавливает: Time.deltaTime = 0)
    bool finished;        // уровень уже пройден или уже перезапускается

    void Awake()
    {
        Current = this;
        player = FindAnyObjectByType<Player3D>();
        health = player != null ? player.GetComponent<Health>() : null;
        foreach (EnemySpawn e in level.enemies) SpawnEnemy(e);
    }

    void Start()
    {
        if (spawner != null) spawner.Begin(level);
    }

    void OnDestroy()
    {
        if (Current == this) Current = null;
    }

    void Update()
    {
        if (finished) return;
        elapsed += Time.deltaTime;
        if (player != null && player.transform.position.y < level.fallY) Restart();   // упали в пропасть
    }

    // Враг из списка LevelData: создаём префаб в точке, лошади отдаём маршрут
    void SpawnEnemy(EnemySpawn e)
    {
        if (e.prefab == null) return;
        GameObject enemy = Instantiate(e.prefab, e.position, Quaternion.Euler(0f, e.rotationY, 0f));
        PatrollerAI patroller = enemy.GetComponent<PatrollerAI>();
        if (patroller != null) patroller.SetRoute(e.route);
    }

    // Сколько монет нужно для финиша: -1 в ассете = все монеты уровня
    public int CoinsNeeded(int coinsOnLevel)
    {
        if (level.coinsRequired < 0) return coinsOnLevel;
        return Mathf.Min(level.coinsRequired, coinsOnLevel);
    }

    // Уровень пройден: очки -> сохранение (момент 1) -> следующая сцена
    public void Complete()
    {
        if (finished) return;
        finished = true;
        int coins = player != null ? player.coins : 0;
        int hearts = health != null ? health.hp : 0;
        int secondsLeft = Mathf.FloorToInt(TimeLeft);
        int score = coins * level.pointsPerCoin
                  + hearts * level.pointsPerHeart
                  + secondsLeft * level.pointsPerSecondLeft;
        Debug.Log("Level " + level.number + " complete: score " + score + ", time " + elapsed.ToString("0.0") + " s");
        SaveSystem.LevelCompleted(level.number, score, elapsed);
        SoundManager.Win();                   // НОВОЕ (12): звук победы доиграет уже в следующей сцене
        SceneManager.LoadScene(level.nextScene);
    }

    // Проигрыш: упали в пропасть или кончились жизни (Health)
    public void Restart()
    {
        if (finished) return;                 // НОВОЕ (12): звук проигрыша не играет дважды
        finished = true;
        SoundManager.Lose();                  // НОВОЕ (12)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
