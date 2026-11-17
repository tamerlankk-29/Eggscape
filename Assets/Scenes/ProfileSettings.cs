using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Настройки в меню. Раньше писались в PlayerPrefs, теперь - в SaveSystem.Data.settings (save.json).
// Методы меняют только память; файл пишется, когда панель закрыта (MenuButtons.Back).
public class ProfileSettings : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_Dropdown colorDropdown;
    public Toggle fullscreenToggle;
    public Toggle musicToggle;
    public Slider volumeSlider;          // НОВОЕ: перетащи Slider из SettingsPanel
    public AudioSource bgSound;          // музыка меню

    GameSettings S => SaveSystem.Data.settings;   // короткое имя для настроек из save.json

    void Start()
    {
        // Показать сохранённое, не вызывая обработчики
        nameField.SetTextWithoutNotify(S.playerName);
        colorDropdown.SetValueWithoutNotify(S.colorIndex);
        fullscreenToggle.SetIsOnWithoutNotify(S.fullscreen);
        musicToggle.SetIsOnWithoutNotify(S.music);
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(S.volume);
        bgSound.mute = !S.music;
    }

    public void SaveName(string value)  { S.playerName = value; }
    public void SaveColor(int index)    { S.colorIndex = index; }

    public void SetFullscreen(bool isOn)
    {
        S.fullscreen = isOn;
        Screen.fullScreen = isOn;
    }

    public void SetMusic(bool isOn)
    {
        S.music = isOn;
        bgSound.mute = !isOn;
    }

    // НОВОЕ: слайдер громкости (был в MenuButtons). Действует на все звуки игры
    public void SetVolume(float value)
    {
        S.volume = value;
        AudioListener.volume = value;
    }
}
