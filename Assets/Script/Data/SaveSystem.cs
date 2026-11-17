using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Прогресс и настройки в файле save.json в Application.persistentDataPath.
// JsonUtility, а не BinaryFormatter: BinaryFormatter небезопасен, а JSON можно открыть и прочитать.
public static class SaveSystem
{
    public const int CurrentVersion = 1;

    // Папка, которая переживает обновления. Никаких "C:/..." и "Assets/..."
    static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
    static string TempPath => Path.Combine(Application.persistentDataPath, "save.tmp");

    static SaveData data;   // данные в памяти (RAM)

    // В проекте выключена перезагрузка домена при Play (Enter Play Mode Options),
    // поэтому static-поля живут между запусками Play. Обнуляем их при каждом старте,
    // иначе игра не перечитывает save.json и берёт старые данные из памяти.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        data = null;
    }

    // Текущие данные. При первом обращении читаются из файла
    public static SaveData Data
    {
        get
        {
            if (data == null) data = Load();
            return data;
        }
    }

    // Безопасная загрузка: нет файла, пустой или сломанный -> значения по умолчанию
    public static SaveData Load()
    {
        if (!File.Exists(FilePath)) return new SaveData();      // первый запуск или после сброса
        try
        {
            string json = File.ReadAllText(FilePath);
            SaveData loaded = JsonUtility.FromJson<SaveData>(json); // сломанный JSON -> исключение
            return Repair(loaded);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Broken save.json, using defaults: " + e.Message);
            return new SaveData();
        }
    }

    // Чиним то, что файл мог оставить пустым или странным
    static SaveData Repair(SaveData d)
    {
        if (d == null) return new SaveData();                  // пустой файл: FromJson вернул null
        if (d.version != CurrentVersion)                         // чужой или старый формат
        {
            Debug.LogWarning("save.json version " + d.version + " is not " + CurrentVersion + ", using defaults");
            return new SaveData();
        }
        if (d.unlockedLevels == null) d.unlockedLevels = new List<int>();
        if (!d.unlockedLevels.Contains(1)) d.unlockedLevels.Add(1);   // первый уровень всегда открыт
        if (d.records == null) d.records = new List<LevelRecord>();
        if (d.settings == null) d.settings = new GameSettings();
        d.settings.volume = Mathf.Clamp01(d.settings.volume);           // громкость строго 0..1
        return d;
    }

    // Запись: сначала в save.tmp, потом подменяем save.json.
    // Села батарея посередине - сломан только tmp, старый save.json цел.
    public static void Save()
    {
        string json = JsonUtility.ToJson(Data, true);   // true = с переносами строк, читаемо
        try
        {
            File.WriteAllText(TempPath, json);
            if (File.Exists(FilePath)) File.Replace(TempPath, FilePath, null);
            else File.Move(TempPath, FilePath);              // самое первое сохранение
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Safe save failed (" + e.Message + "), writing save.json directly");
            try { File.WriteAllText(FilePath, json); }
            catch (System.Exception e2) { Debug.LogError("Save failed: " + e2.Message); }
        }
    }

    // Момент сохранения 1: уровень пройден. Вызывает LevelLoader.Complete
    public static void LevelCompleted(int level, int score, float time)
    {
        LevelRecord r = Data.GetRecord(level);
        if (r == null)
        {
            r = new LevelRecord { level = level, bestScore = score, bestTime = time };
            Data.records.Add(r);
        }
        else
        {
            r.bestScore = Mathf.Max(r.bestScore, score);
            r.bestTime = Mathf.Min(r.bestTime, time);
        }
        if (!Data.unlockedLevels.Contains(level + 1)) Data.unlockedLevels.Add(level + 1);
        Save();
    }

    // Сброс прогресса: файл и данные в памяти.
    // Если оставить старые данные в памяти, следующая пауза запишет их обратно.
    public static void DeleteAll()
    {
        if (File.Exists(FilePath)) File.Delete(FilePath);
        if (File.Exists(TempPath)) File.Delete(TempPath);
        data = new SaveData();
    }

    public static string Folder => Application.persistentDataPath;
}
