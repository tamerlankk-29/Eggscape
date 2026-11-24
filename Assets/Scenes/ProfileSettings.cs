using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Настройки в меню. Хранятся в SaveSystem.Data.settings (save.json).
// Методы меняют только память; файл пишется, когда панель закрыта (MenuButtons.Back)
// и когда игра уходит в фон (SaveKeeper).
// Неделя 12: громкость идёт в AudioMixer в децибелах (VolumeSettings), музыку играет MusicManager.
public class ProfileSettings : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_Dropdown colorDropdown;
    public Toggle fullscreenToggle;
    public Toggle musicToggle;
    public Slider volumeSlider;          // Master: общая громкость
    public Slider musicSlider;           // НОВОЕ (12): Music
    public Slider sfxSlider;             // НОВОЕ (12): SFX (эффекты и кнопки)

    GameSettings S => SaveSystem.Data.settings;   // короткое имя для настроек из save.json

    void Start()
    {
        // Показать сохранённое, не вызывая обработчики
        nameField.SetTextWithoutNotify(S.playerName);
        colorDropdown.SetValueWithoutNotify(S.colorIndex);
        fullscreenToggle.SetIsOnWithoutNotify(S.fullscreen);
        musicToggle.SetIsOnWithoutNotify(S.music);
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(S.volume);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(S.musicVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(S.sfxVolume);
    }

    public void SaveName(string value)  { S.playerName = value; }
    public void SaveColor(int index)    { S.colorIndex = index; }

    public void SetFullscreen(bool isOn)
    {
        S.fullscreen = isOn;
        Screen.fullScreen = isOn;
    }

    // Галочка Music: выкл = MusicVol -80 dB во всей игре
    public void SetMusic(bool isOn)
    {
        S.music = isOn;
        VolumeSettings.Apply();
    }

    // Слайдеры: On Value Changed -> Dynamic float. Min Value = 0.0001, Max Value = 1
    public void SetVolume(float value)
    {
        S.volume = value;
        VolumeSettings.Apply();
    }

    public void SetMusicVolume(float value)
    {
        S.musicVolume = value;
        VolumeSettings.Apply();
    }

    public void SetSfxVolume(float value)
    {
        S.sfxVolume = value;
        VolumeSettings.Apply();
    }
}
