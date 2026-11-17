using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Выбор уровня. Открыт уровень или нет - из save.json, название и сцена - из LevelData.
public class LevelSelect : MonoBehaviour
{
    [SerializeField] LevelData[] levels;   // Level_01, Level_02, Level_03 - по порядку
    [SerializeField] Button[] buttons;     // кнопки уровней, в том же порядке
    [SerializeField] TMP_Text[] labels;    // текст на каждой кнопке

    // Каждый раз, когда панель показывается, перечитываем сохранение
    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        SaveData save = SaveSystem.Data;
        for (int i = 0; i < levels.Length; i++)
        {
            LevelData level = levels[i];
            bool open = save.IsUnlocked(level.number);
            buttons[i].interactable = open;               // закрытую кнопку нельзя нажать, она серая

            LevelRecord record = save.GetRecord(level.number);
            if (!open)
                labels[i].text = level.number + ". LOCKED";
            else if (record == null)
                labels[i].text = level.number + ". " + level.title;
            else
                labels[i].text = level.number + ". " + level.title + "\nBest: " + record.bestScore;
        }
    }

    // Кнопка уровня: On Click -> LevelSelect.Play, в поле число 0, 1 или 2
    public void Play(int index)
    {
        if (!SaveSystem.Data.IsUnlocked(levels[index].number)) return;
        SceneManager.LoadScene(levels[index].sceneName);
    }
}
