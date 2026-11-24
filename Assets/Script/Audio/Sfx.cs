using UnityEngine;

// Вариация звука: вместо src.PlayOneShot(clip) пишем src.PlayVaried(clip).
// Каждый раз высота (pitch) немного другая: 0.92..1.08. Двадцатый прыжок подряд
// не звучит "как пулемёт", а клип при этом один.
public static class Sfx
{
    public const float MinPitch = 0.92f;
    public const float MaxPitch = 1.08f;

    public static void PlayVaried(this AudioSource src, AudioClip clip)
    {
        if (src == null || clip == null) return;   // забыли клип в Inspector - просто тишина, без ошибки
        src.pitch = Random.Range(MinPitch, MaxPitch);
        src.PlayOneShot(clip);
    }
}
