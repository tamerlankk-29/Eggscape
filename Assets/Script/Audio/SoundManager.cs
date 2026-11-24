using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Звуки, которые должны доиграть, даже если сцена сразу меняется: победа, проигрыш,
// смерть, клик по кнопке. Живут на объекте Audio, который не уничтожается между сценами.
public class SoundManager : MonoBehaviour
{
    public static SoundManager I { get; private set; }

    [SerializeField] AudioSource sfx;   // Output = SFX
    [SerializeField] AudioSource ui;    // Output = UI
    [SerializeField] AudioClip clickClip, winClip, loseClip, deathClip;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;   // иначе после Stop останется подписка на мёртвый объект
        if (I == this) I = null;
    }

    // В редакторе у проекта выключена перезагрузка сцены при Play (Enter Play Mode Options),
    // и для первой сцены sceneLoaded не приходит. Поэтому первую сцену подключаем ещё и здесь
    void Start()
    {
        HookButtons(SceneManager.GetActiveScene());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HookButtons(scene);
    }

    // Всем кнопкам сцены (и в выключенных панелях тоже) добавляем звук клика.
    // Не надо вручную вешать звук на каждую из 20 кнопок. JUMP и FIRE - не Button, их не трогаем.
    // RemoveListener перед AddListener: если сцену подключили дважды, клик всё равно один
    void HookButtons(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Button b in root.GetComponentsInChildren<Button>(true))
            {
                b.onClick.RemoveListener(Click);
                b.onClick.AddListener(Click);
            }
    }

    public static void Click() { if (I != null) I.ui.PlayVaried(I.clickClip); }
    public static void Win()   { if (I != null) I.PlayPlain(I.winClip); }
    public static void Lose()  { if (I != null) I.PlayPlain(I.loseClip); }
    public static void Death() { if (I != null) I.PlayPlain(I.deathClip); }

    // Короткие мелодии без вариации: у победы и проигрыша высота должна быть "правильной"
    void PlayPlain(AudioClip clip)
    {
        if (clip == null) return;
        sfx.pitch = 1f;
        sfx.PlayOneShot(clip);
    }
}
