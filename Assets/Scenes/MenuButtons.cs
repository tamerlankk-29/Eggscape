using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject settingsPanel;

    // Старая кнопка "играть с начала". Start в меню теперь открывает выбор уровня
    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("main menu");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting the game");
    }

    public void Settings()
    {
        settingsPanel.SetActive(true);
    }

    // НОВОЕ: момент сохранения 2 - настройки закрыты, пишем файл один раз
    public void Back()
    {
        settingsPanel.SetActive(false);
        SaveSystem.Save();
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}
