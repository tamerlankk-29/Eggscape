using UnityEditor;
using UnityEngine;

// Только для редактора (папка Editor не попадает в APK).
// В верхнем меню Unity появится Eggscape -> Open Save Folder.
public static class SaveMenu
{
    [MenuItem("Eggscape/Open Save Folder")]
    static void OpenFolder()
    {
        string file = System.IO.Path.Combine(SaveSystem.Folder, "save.json");
        // есть файл - откроется папка с выделенным save.json; нет - просто папка
        EditorUtility.RevealInFinder(System.IO.File.Exists(file) ? file : SaveSystem.Folder);
    }
}
