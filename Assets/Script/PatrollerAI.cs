using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Лошадь-патрульный. Состояния: Patrol -> Chase -> Attack (замах) -> Recover -> Chase ...
// Все числа баланса - в PatrollerStats (Horse_Stats). Маршрут - в LevelData.
[RequireComponent(typeof(NavMeshAgent), typeof(EnemySight))]
public class PatrollerAI : MonoBehaviour
{
    enum State { Patrol, Chase, Attack, Recover }

    // Техника, не баланс
    const float RepathInterval = 0.2f;   // с, пересчёт пути в погоне (5 раз в секунду)
    const float ArriveDistance = 0.3f;   // м, «дошла до точки маршрута»
    const float PatrolStopping = 0.2f;   // м, stoppingDistance на маршруте
    const float ChaseStopGap = 0.4f;     // м, в погоне останавливается чуть раньше attackRange

    [SerializeField] PatrollerStats stats;   // НОВОЕ: Horse_Stats
    [Header("Предупреждение и отладка")]
    [SerializeField] GameObject warning;     // красный "!" над головой
    [SerializeField] AudioClip windUpClip;
    [SerializeField] Animator anim;
    [SerializeField] TMP_Text stateLabel;

    Vector3[] route = new Vector3[0];        // НОВОЕ: точки маршрута из LevelData
    State state;
    float timer;
    float lostTimer;
    float repath;
    int point;
    NavMeshAgent agent;
    EnemySight sight;
    AudioSource src;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
        src = GetComponent<AudioSource>();
    }

    // НОВОЕ: вызывает LevelLoader сразу после создания лошади (до Start)
    public void SetRoute(Vector3[] points)
    {
        route = points != null ? points : new Vector3[0];
    }

    void Start()
    {
        SetState(State.Patrol);
    }

    void Update()
    {
        timer += Time.deltaTime;
        switch (state)
        {
            case State.Patrol:
                agent.speed = stats.moveSpeed;          // читаем каждый кадр: правка ассета видна сразу
                Patrol();
                if (sight.Sees) SetState(State.Chase);
                break;

            case State.Chase:
                agent.speed = stats.chaseSpeed;
                lostTimer = sight.Sees ? 0f : lostTimer + Time.deltaTime;
                repath -= Time.deltaTime;
                if (repath <= 0f)
                {
                    repath = RepathInterval;
                    agent.SetDestination(sight.Player.position);
                }
                if (sight.DistanceToPlayer() < stats.attackRange) SetState(State.Attack);
                else if (lostTimer > stats.loseTime) SetState(State.Patrol);
                break;

            case State.Attack:                             // стоит и замахивается
                sight.FacePlayer(stats.turnSpeed);
                if (timer >= stats.windUp)
                {
                    if (sight.DistanceToPlayer() < stats.hitReach)
                        sight.PlayerHealth.TakeDamage(stats.damage);
                    SetState(State.Recover);
                }
                break;

            case State.Recover:                            // окно игрока
                if (timer >= stats.recoverTime) SetState(State.Chase);
                break;
        }
    }

    void Patrol()
    {
        if (route.Length == 0) return;
        if (!agent.pathPending && agent.remainingDistance < ArriveDistance)
        {
            point = (point + 1) % route.Length;
            agent.SetDestination(route[point]);
        }
    }

    // Единственное место, где меняется состояние
    void SetState(State next)
    {
        state = next;
        timer = 0f;
        lostTimer = 0f;
        repath = 0f;

        bool standing = next == State.Attack || next == State.Recover;
        agent.isStopped = standing;
        agent.stoppingDistance = next == State.Patrol ? PatrolStopping : stats.attackRange - ChaseStopGap;
        if (next == State.Patrol && route.Length > 0)
            agent.SetDestination(route[point]);

        if (warning != null) warning.SetActive(next == State.Attack);
        if (next == State.Attack && windUpClip != null) src.PlayVaried(windUpClip);
        if (anim != null)
            anim.CrossFade(next == State.Patrol ? "Walk" : next == State.Chase ? "Run" : "Idle", 0.15f);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }
}
