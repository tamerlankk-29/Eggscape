using UnityEngine;

// Один на сцену: какой трек должен играть здесь. Сам ничего не играет -
// просит MusicManager сделать кроссфейд (или ничего, если трек тот же).
public class SceneMusic : MonoBehaviour
{
    [SerializeField] AudioClip music;   // music_menu или music_level

    void Start()
    {
        MusicManager.Play(music);
    }
}
