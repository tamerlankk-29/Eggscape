using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hud : MonoBehaviour
{
    public Image[] hearts;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI timeText;     // НОВОЕ, необязательно: секунды до конца лимита

    Health health;
    Player3D player;

    void Start()
    {
        health = FindAnyObjectByType<Health>();
        player = FindAnyObjectByType<Player3D>();
    }

    void Update()
    {
        if (health == null || player == null) return;
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < health.hp;
        bool bossLevel = LevelLoader.Current != null && LevelLoader.Current.Level.goal == LevelGoal.DefeatBoss;
        coinText.text = bossLevel ? "Defeat the cow!" : "x " + player.coins + " / " + player.coinsNeeded;  // НОВОЕ: цель из LevelData
        if (timeText != null && LevelLoader.Current != null)
            timeText.text = Mathf.CeilToInt(LevelLoader.Current.TimeLeft).ToString();
    }
}
