using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hud : MonoBehaviour
{
    public Image[] hearts;
    public TextMeshProUGUI coinText;

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
        coinText.text = "x " + player.coins + " / " + player.coinsTotal;
    }
}
