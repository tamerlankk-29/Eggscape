using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    private int coins = 0;

    void OnTriggerEnter(Collider coin)
    {
        Destroy(coin.gameObject);
        coins++;
        coinText.text = "Монеты: " + coins;
    }
}
