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
    public float volume = 1f;        // 0..1, общая громкость игры
    public bool music = true;        // музыка в меню
    public bool fullscreen = true;
    public string playerName = "";
    public int colorIndex = 0;

    // Применить к игре (при запуске и после сброса)
    public void Apply()
    {
        AudioListener.volume = volume;
        Screen.fullScreen = fullscreen;
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
