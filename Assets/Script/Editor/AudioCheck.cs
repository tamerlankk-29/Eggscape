using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Только для редактора. Eggscape -> Audio -> Check Routing:
// находит каждый AudioSource в префабах и в открытой сцене и ругается на те, у которых пустой Output.
public static class AudioCheck
{
    [MenuItem("Eggscape/Audio/Check Routing")]
    static void Check()
    {
        int total = 0, bad = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".prefab")) continue;               // модели FBX пропускаем
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            foreach (AudioSource s in prefab.GetComponentsInChildren<AudioSource>(true))
            {
                total++;
                if (s.outputAudioMixerGroup != null) continue;
                bad++;
                Debug.LogWarning("No Output: prefab " + path + " -> " + s.name, prefab);
            }
        }

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (AudioSource s in root.GetComponentsInChildren<AudioSource>(true))
                {
                    total++;
                    if (s.outputAudioMixerGroup != null) continue;
                    bad++;
                    Debug.LogWarning("No Output: scene " + scene.name + " -> " + s.name, s);
                }
        }

        if (bad == 0) Debug.Log("Audio routing OK: " + total + " AudioSources, every one has an Output group");
        else Debug.LogWarning("Audio routing: " + bad + " of " + total + " AudioSources have no Output (click a warning to find it)");
    }
}
