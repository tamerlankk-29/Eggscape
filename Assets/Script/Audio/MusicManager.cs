using System.Collections;
using UnityEngine;

// Музыка всей игры. Один экземпляр на всю игру (DontDestroyOnLoad), поэтому при смене сцены
// музыка не обрывается. Два AudioSource: один играет, второй плавно входит -> кроссфейд без щелчка.
// Если сцена просит тот же трек, что уже играет (перезапуск уровня), ничего не делаем:
// музыка не начинается заново.
public class MusicManager : MonoBehaviour
{
    public static MusicManager I { get; private set; }

    [SerializeField] AudioSource musicA;       // Output = Tracks
    [SerializeField] AudioSource musicB;       // Output = Tracks
    [SerializeField] float fadeTime = 1.5f;    // секунды кроссфейда

    AudioSource current;    // тот, что играет сейчас
    Coroutine fade;

    // Префаб Assets/Resources/Audio.prefab создаётся сам до первой сцены, как SaveKeeper.
    // Поэтому звук и микшер есть, даже если нажать Play сразу в Level2.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        GameObject prefab = Resources.Load<GameObject>("Audio");
        if (prefab == null)
        {
            Debug.LogWarning("No Assets/Resources/Audio.prefab: no music, no mixer volumes");
            return;
        }
        GameObject go = Instantiate(prefab);
        go.name = "Audio";
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }   // второй экземпляр не нужен
        I = this;
        current = musicA;
    }

    // Вызывает SceneMusic в Start каждой сцены
    public static void Play(AudioClip clip)
    {
        if (I != null) I.CrossfadeTo(clip);
    }

    void CrossfadeTo(AudioClip clip)
    {
        if (clip == null) return;
        if (current.clip == clip && current.isPlaying) return;   // тот же трек: не перезапускаем

        AudioSource next = current == musicA ? musicB : musicA;
        next.clip = clip;
        next.volume = 0f;          // новый трек начинается с тишины -> нет щелчка
        next.loop = true;
        next.Play();

        if (fade != null) StopCoroutine(fade);
        fade = StartCoroutine(Crossfade(current, next));
        current = next;
    }

    // Equal power (cos/sin): в середине кроссфейда нет "провала" громкости.
    // unscaledDeltaTime: кроссфейд идёт и в паузе, когда Time.timeScale = 0
    IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        float fromStart = from.volume;
        for (float t = 0f; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            float k = t / fadeTime;
            from.volume = fromStart * Mathf.Cos(k * Mathf.PI * 0.5f);
            to.volume = Mathf.Sin(k * Mathf.PI * 0.5f);
            yield return null;
        }
        from.volume = 0f;
        from.Stop();
        to.volume = 1f;
        fade = null;
    }
}
