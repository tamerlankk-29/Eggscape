using UnityEngine;
using UnityEngine.AI;   // NavMeshAgent
using TMPro;            // TMP_Text для надписи состояния

// Лошадь-патрульный. Состояния: Patrol -> Chase -> Attack (замах) -> Recover -> Chase ...
// Проверяет навык игрока: позиционирование (не попадать в конус, прятаться за заборами).
[RequireComponent(typeof(NavMeshAgent), typeof(EnemySight))]
public class PatrollerAI : MonoBehaviour
{
    enum State { Patrol, Chase, Attack, Recover }

    [Header("Маршрут")]
    [SerializeField] Transform[] waypoints;        // точки маршрута (пустые объекты в уровне)
    [SerializeField] float patrolSpeed = 2f;       // м/с
    [Header("Погоня")]
    [SerializeField] float chaseSpeed = 4.5f;      // м/с (яйцо катится до 8 м/с - можно уйти)
    [SerializeField] float loseTime = 2f;          // с без видимости -> назад на маршрут
    [Header("Атака")]
    [SerializeField] float attackRange = 2.6f;     // м между центрами: от центра лошади до носа ~1.6 + радиус яйца 0.5
    [SerializeField] float windUp = 0.5f;          // с, предупреждение перед ударом
    [SerializeField] float hitReach = 3.2f;        // м, удар попадёт, только если игрок всё ещё ближе
    [SerializeField] float recoverTime = 0.8f;     // с, окно для ответа игрока после удара
    [SerializeField] int damage = 1;               // сколько жизней отнимает
    [Header("Предупреждение и отладка")]
    [SerializeField] GameObject warning;           // красный "!" над головой, виден во время замаха
    [SerializeField] AudioClip windUpClip;         // звук замаха
    [SerializeField] Animator anim;                // необязательно: состояния Idle / Walk / Run
    [SerializeField] TMP_Text stateLabel;          // необязательно: 3D-текст с именем состояния

    State state;          // текущее состояние
    float timer;          // секунд в текущем состоянии
    float lostTimer;      // секунд с тех пор, как последний раз видела игрока
    float repath;         // секунд до следующего пересчёта пути
    int point;            // номер текущей точки маршрута
    NavMeshAgent agent;
    EnemySight sight;
    AudioSource src;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
        src = GetComponent<AudioSource>();
    }

    void Start()
    {
        SetState(State.Patrol);   // начинаем с маршрута
    }

    void Update()
    {
        timer += Time.deltaTime;
        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (sight.Sees) SetState(State.Chase);
                break;

            case State.Chase:
                lostTimer = sight.Sees ? 0f : lostTimer + Time.deltaTime;
                repath -= Time.deltaTime;
                if (repath <= 0f)                                  // новый путь 5 раз в секунду, не каждый кадр
                {
                    repath = 0.2f;
                    agent.SetDestination(sight.Player.position);
                }
                if (sight.DistanceToPlayer() < attackRange) SetState(State.Attack);
                else if (lostTimer > loseTime) SetState(State.Patrol);
                break;

            case State.Attack:                                     // стоит и замахивается - это и есть предупреждение
                sight.FacePlayer(360f);
                if (timer >= windUp)
                {
                    if (sight.DistanceToPlayer() < hitReach)       // успел откатиться - не попала
                        sight.PlayerHealth.TakeDamage(damage);
                    SetState(State.Recover);
                }
                break;

            case State.Recover:                                    // окно игрока: лошадь стоит
                if (timer >= recoverTime) SetState(State.Chase);
                break;
        }
    }

    // Идём к текущей точке; дошли - берём следующую (по кругу)
    void Patrol()
    {
        if (waypoints.Length == 0) return;
        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            point = (point + 1) % waypoints.Length;
            agent.SetDestination(waypoints[point].position);
        }
    }

    // Единственное место, где меняется состояние. Здесь вся логика "входа"
    void SetState(State next)
    {
        state = next;
        timer = 0f;
        lostTimer = 0f;
        repath = 0f;

        bool standing = next == State.Attack || next == State.Recover;
        agent.isStopped = standing;                                  // во время замаха и отдыха стоит
        agent.speed = next == State.Patrol ? patrolSpeed : chaseSpeed;
        agent.stoppingDistance = next == State.Patrol ? 0.2f : attackRange - 0.4f; // в погоне останавливается до яйца, а не толкает его
        if (next == State.Patrol && waypoints.Length > 0)
            agent.SetDestination(waypoints[point].position);

        if (warning != null) warning.SetActive(next == State.Attack); // "!" только во время замаха
        if (next == State.Attack && windUpClip != null) src.PlayOneShot(windUpClip);
        if (anim != null)
            anim.CrossFade(next == State.Patrol ? "Walk" : next == State.Chase ? "Run" : "Idle", 0.15f);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }
}