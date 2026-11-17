using UnityEngine;

// Создаётся сам до первой сцены и не уничтожается при смене сцен.
// 1) применяет сохранённые настройки, 2) сохраняет, когда игра уходит в фон.
public class SaveKeeper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        GameObject go = new GameObject("SaveKeeper");
        DontDestroyOnLoad(go);
        go.AddComponent<SaveKeeper>();
        SaveSystem.Data.settings.Apply();                  // громкость и экран из save.json
        Debug.Log("Save folder: " + SaveSystem.Folder);
    }

    // Момент сохранения 3: Home, звонок, смена приложения.
    // После этого телефон может убить игру без предупреждения.
    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.Save();
    }

    // Запасной вариант: игра потеряла фокус. Дважды записать маленький файл не страшно
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveSystem.Save();
    }
}
