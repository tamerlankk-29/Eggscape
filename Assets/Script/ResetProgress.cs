using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Сброс прогресса. Одно нажатие ничего не стирает: сначала окно подтверждения.
public class ResetProgress : MonoBehaviour
{
    [SerializeField] GameObject confirmPanel;   // тёмная панель на весь экран, скрыта
    [SerializeField] TMP_Text message;          // что именно будет потеряно

    // Кнопка Reset progress
    public void AskReset()
    {
        SaveData d = SaveSystem.Data;
        int best = 0;
        foreach (LevelRecord r in d.records) best = Mathf.Max(best, r.bestScore);
        message.text = "Delete all progress?\n"
                     + d.records.Count + " finished level(s), best score " + best + ",\n"
                     + "unlocked levels and settings will be lost.";
        confirmPanel.SetActive(true);
    }

    // Кнопка Cancel: ничего не меняет
    public void Cancel()
    {
        confirmPanel.SetActive(false);
    }

    // Кнопка Delete
    public void ConfirmReset()
    {
        SaveSystem.DeleteAll();                  // и файл, и данные в памяти
        SaveSystem.Data.settings.Apply();        // громкость и экран по умолчанию
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   // перезагрузить меню
    }
}
