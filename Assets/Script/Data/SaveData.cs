using System.Collections.Generic;
using UnityEngine;

// Рекорд одного уровня
[System.Serializable]
public class LevelRecord
{
    public int level;        // номер уровня 1..3
    public int bestScore;    // лучший счёт
    public float bestTime;   // лучшее время прохождения, с
}

// Настройки игрока (раньше были в PlayerPrefs)
[System.Serializable]
public class GameSettings
{
    public float volume = 1f;        // 0..1, общая громкость (MasterVol в микшере)
    public float musicVolume = 0.4f; // НОВОЕ (12): 0..1, музыка (MusicVol). 0.4 = -8 dB: музыка под эффектами
    public float sfxVolume = 0.7f;   // НОВОЕ (12): 0..1, эффекты (SFXVol). 0.7 = -3 dB
    public bool music = true;        // галочка Music: выкл = музыка молчит во всей игре
    public bool fullscreen = true;
    public string playerName = "";
    public int colorIndex = 0;

    // Применить к игре (при запуске и после сброса)
    public void Apply()
    {
        AudioListener.volume = 1f;       // НОВОЕ (12): громкость теперь в микшере, не в AudioListener
        Screen.fullScreen = fullscreen;
        VolumeSettings.Apply();          // три громкости в dB -> AudioMixer
    }
}

// Весь файл save.json. Значения после = - это и есть "значения по умолчанию":
// их получает новый игрок и игрок со сломанным файлом.
[System.Serializable]
public class SaveData
{
    public int version = 1;                                   // номер формата: поднимем, если изменим поля
    public List<int> unlockedLevels = new List<int> { 1 };    // первый уровень открыт всегда
    public List<LevelRecord> records = new List<LevelRecord>();
    public GameSettings settings = new GameSettings();

    public bool IsUnlocked(int level) { return unlockedLevels.Contains(level); }

    // Рекорд уровня или null, если уровень ещё не пройден
    public LevelRecord GetRecord(int level) { return records.Find(r => r.level == level); }
}
