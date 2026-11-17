using UnityEngine;

// "Глаза" врага: дальность -> угол -> стены (Raycast). Дальность, угол и высота глаз - в EnemyStats.
public class EnemySight : MonoBehaviour
{
    const float CheckInterval = 0.15f;            // техника: смотрим не каждый кадр, а ~7 раз в секунду
    const float ScreenMargin = 0.05f;             // техника: отступ от края экрана для IsOnScreen

    [SerializeField] EnemyStats stats;            // НОВОЕ: Horse_Stats / Llama_Stats / Pig_Stats
    [SerializeField] LayerMask obstacleMask;      // ТОЛЬКО слой Walls: через что не видно

    public Transform Player { get; private set; }
    public Health PlayerHealth { get; private set; }
    public bool Sees { get; private set; }

    float checkTimer;
    Camera cam;

    void Awake()
    {
        Player3D p = FindFirstObjectByType<Player3D>();   // ищем ОДИН раз, не в Update
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
        checkTimer = CheckInterval;
        Sees = CanSeePlayer();
    }

    bool CanSeePlayer()
    {
        if (Player == null) return false;
        Vector3 eye = transform.position + Vector3.up * stats.eyeHeight;
        Vector3 toPlayer = Player.position - eye;

        // 1. Дальность
        if (toPlayer.magnitude > stats.viewRange) return false;

        // 2. Угол (по горизонтали)
        Vector3 flat = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (Vector3.Angle(transform.forward, flat) > stats.fov / 2f) return false;

        // 3. Стены: луч от глаз до игрока только по слою Walls
        bool blocked = Physics.Raycast(eye, toPlayer.normalized, toPlayer.magnitude,
                                       obstacleMask, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(eye, toPlayer, blocked ? Color.red : Color.green, CheckInterval);
        return !blocked;
    }

    public float DistanceToPlayer()
    {
        if (Player == null) return Mathf.Infinity;
        Vector3 d = Player.position - transform.position;
        d.y = 0f;
        return d.magnitude;
    }

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
        Vector3 v = cam.WorldToViewportPoint(transform.position + Vector3.up * stats.eyeHeight);
        return v.z > 0f && v.x > ScreenMargin && v.x < 1f - ScreenMargin
                        && v.y > ScreenMargin && v.y < 1f - ScreenMargin;
    }

    public void FacePlayer(float turnSpeed)
    {
        Vector3 d = DirectionToPlayer();
        if (d.sqrMagnitude < 0.01f) return;
        Quaternion look = Quaternion.LookRotation(d);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null) return;                // ассет ещё не перетащили - не рисуем
        Vector3 eye = transform.position + Vector3.up * stats.eyeHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.viewRange);
        Vector3 left = Quaternion.Euler(0f, -stats.fov / 2f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, stats.fov / 2f, 0f) * transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(eye, left * stats.viewRange);
        Gizmos.DrawRay(eye, right * stats.viewRange);
    }
}
