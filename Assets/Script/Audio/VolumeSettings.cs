using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Громкость через AudioMixer и звук паузы.
// Слайдер даёт 0.0001..1, микшер понимает децибелы: dB = Log10(v) * 20.
// 1 -> 0 dB (как есть), 0.5 -> -6 dB (вдвое тише на слух), 0.0001 -> -80 dB (тишина).
public class VolumeSettings : MonoBehaviour
{
    // Имена exposed-параметров в окне Audio Mixer - буква в букву
    public const string MasterParam = "MasterVol";
    public const string MusicParam = "MusicVol";
    public const string SfxParam = "SFXVol";

    const float MinLinear = 0.0001f;   // Log10(0) = минус бесконечность, поэтому не ниже этого
    const float SnapshotTime = 0.3f;   // секунды перехода Normal <-> Paused

    public static VolumeSettings I { get; private set; }

    [SerializeField] AudioMixer mixer;            // Assets/Audio/Main.mixer
    [SerializeField] AudioMixerSnapshot normal;   // снапшот Normal
    [SerializeField] AudioMixerSnapshot paused;   // снапшот Paused: музыка тише и глуше

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;   // снапшот работает и при timeScale = 0
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (I == this) I = null;
    }

    // В Awake SetFloat микшера ещё не срабатывает - поэтому в Start
    void Start()
    {
        Apply();
    }

    // Любая новая сцена начинается без паузы (Restart и Menu из паузы)
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPaused(false);
    }

    public static float ToDb(float linear)
    {
        return Mathf.Log10(Mathf.Max(linear, MinLinear)) * 20f;
    }

    // Значения из save.json -> микшер. Зовут: Start, слайдеры, сброс прогресса
    public static void Apply()
    {
        if (I == null) return;
        GameSettings s = SaveSystem.Data.settings;
        I.mixer.SetFloat(MasterParam, ToDb(s.volume));
        I.mixer.SetFloat(MusicParam, ToDb(s.music ? s.musicVolume : 0f));   // галочка Music выкл -> тишина
        I.mixer.SetFloat(SfxParam, ToDb(s.sfxVolume));
    }

    // PauseMenu: пауза -> Paused (тише + lowpass), продолжить -> Normal
    public static void SetPaused(bool isPaused)
    {
        if (I == null) return;
        AudioMixerSnapshot target = isPaused ? I.paused : I.normal;
        if (target != null) target.TransitionTo(SnapshotTime);
    }
}
