using UnityEngine;

// "Глаза" врага: дальность -> угол -> стены (Raycast).
// Висит на каждом враге. PatrollerAI, ShooterAI, ChargerAI читают отсюда Sees.
public class EnemySight : MonoBehaviour
{
    [SerializeField] float viewRange = 8f;        // дальность зрения, м
    [SerializeField] float fov = 110f;            // ширина конуса, градусы (55 в каждую сторону)
    [SerializeField] float eyeHeight = 1f;        // высота глаз над ногами, м
    [SerializeField] float checkInterval = 0.15f; // как часто смотреть, с (не каждый кадр - дешевле)
    [SerializeField] LayerMask obstacleMask;      // ТОЛЬКО слой Walls: через что не видно

    public Transform Player { get; private set; }    // яйцо
    public Health PlayerHealth { get; private set; } // чтобы наносить урон
    public bool Sees { get; private set; }           // видит ли игрока сейчас

    float checkTimer;   // секунд до следующей проверки
    Camera cam;

    void Awake()
    {
        Player3D p = FindFirstObjectByType<Player3D>(); // ищем ОДИН раз, не в Update
        if (p != null)
        {
            Player = p.transform;
            PlayerHealth = p.GetComponent<Health>();
        }
        cam = Camera.main;
    }

    void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer > 0f) return;
        checkTimer = checkInterval;
        Sees = CanSeePlayer();
    }

    bool CanSeePlayer()
    {
        if (Player == null) return false;
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 toPlayer = Player.position - eye;

        // 1. Дальность: дальше viewRange - не видит
        if (toPlayer.magnitude > viewRange) return false;

        // 2. Угол: внутри конуса? Считаем по горизонтали, чтобы высота не мешала
        Vector3 flat = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (Vector3.Angle(transform.forward, flat) > fov / 2f) return false;

        // 3. Стены: луч от глаз до игрока. Попал в Walls раньше - не видит
        bool blocked = Physics.Raycast(eye, toPlayer.normalized, toPlayer.magnitude,
                                       obstacleMask, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(eye, toPlayer, blocked ? Color.red : Color.green, checkInterval); // отладка
        return !blocked;
    }

    // Расстояние до игрока по земле (без высоты)
    public float DistanceToPlayer()
    {
        if (Player == null) return Mathf.Infinity;
        Vector3 d = Player.position - transform.position;
        d.y = 0f;
        return d.magnitude;
    }

    // Направление на игрока по земле, длина 1
    public Vector3 DirectionToPlayer()
    {
        if (Player == null) return transform.forward;
        Vector3 d = Player.position - transform.position;
        d.y = 0f;
        return d.normalized;
    }

    // Виден ли сам враг на экране. Атаковать из-за кадра нельзя
    public bool IsOnScreen()
    {
        Vector3 v = cam.WorldToViewportPoint(transform.position + Vector3.up * eyeHeight);
        return v.z > 0f && v.x > 0.05f && v.x < 0.95f && v.y > 0.05f && v.y < 0.95f;
    }

    // Плавно повернуться к игроку (только по горизонтали)
    public void FacePlayer(float turnSpeed)   // turnSpeed - градусов в секунду
    {
        Vector3 d = DirectionToPlayer();
        if (d.sqrMagnitude < 0.01f) return;
        Quaternion look = Quaternion.LookRotation(d);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
    }

    // Конус зрения в Scene, когда враг выделен
    void OnDrawGizmosSelected()
    {
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);
        Vector3 left = Quaternion.Euler(0f, -fov / 2f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, fov / 2f, 0f) * transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(eye, left * viewRange);
        Gizmos.DrawRay(eye, right * viewRange);
    }
}