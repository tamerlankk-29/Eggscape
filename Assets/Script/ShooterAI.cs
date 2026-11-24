using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Лама-стрелок. Состояния: Idle -> KeepDistance -> Aim -> Cooldown -> KeepDistance ...
// Все числа баланса - в ShooterStats (Llama_Stats). Урон плевка - в Spit_Stats.
[RequireComponent(typeof(NavMeshAgent), typeof(EnemySight))]
public class ShooterAI : MonoBehaviour
{
    enum State { Idle, KeepDistance, Aim, Cooldown }

    const float RepathInterval = 0.3f;    // техника: как часто выбираем, куда идти

    [SerializeField] ShooterStats stats;           // НОВОЕ: Llama_Stats
    [Header("Ссылки")]
    [SerializeField] GameObject projectilePrefab;  // префаб Spit
    [SerializeField] Transform muzzle;             // точка у рта
    [SerializeField] LineRenderer laser;           // красная линия прицела
    [Header("Звук и отладка")]
    [SerializeField] AudioClip aimClip;
    [SerializeField] AudioClip shootClip;
    [SerializeField] Animator anim;
    [SerializeField] TMP_Text stateLabel;

    State state;
    float timer;
    float lostTimer;
    float repath;
    Vector3 aimDir;
    NavMeshAgent agent;
    EnemySight sight;
    AudioSource src;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
        src = GetComponent<AudioSource>();
        agent.updateRotation = false;   // поворачиваем сами: лама смотрит на игрока, даже когда пятится
    }

    void Start()
    {
        SetState(State.Idle);
    }

    void Update()
    {
        timer += Time.deltaTime;
        switch (state)
        {
            case State.Idle:
                if (sight.Sees) SetState(State.KeepDistance);
                break;

            case State.KeepDistance:
                lostTimer = sight.Sees ? 0f : lostTimer + Time.deltaTime;
                sight.FacePlayer(stats.turnSpeed);
                KeepDistance();
                bool inRange = sight.DistanceToPlayer() <= stats.aimRange;
                if (sight.Sees && inRange && sight.IsOnScreen()) SetState(State.Aim);
                else if (lostTimer > stats.loseTime) SetState(State.Idle);
                break;

            case State.Aim:                                   // стоит и целится - это предупреждение
                sight.FacePlayer(stats.turnSpeed);
                aimDir = (sight.Player.position - muzzle.position).normalized;
                laser.SetPosition(0, muzzle.position);
                laser.SetPosition(1, muzzle.position + aimDir * stats.laserLength);
                if (timer >= stats.aimTime)
                {
                    Fire();
                    SetState(State.Cooldown);
                }
                break;

            case State.Cooldown:                              // перезарядка - окно игрока
                sight.FacePlayer(stats.turnSpeed);
                if (timer >= stats.cooldown) SetState(State.KeepDistance);
                break;
        }
    }

    void KeepDistance()
    {
        agent.speed = stats.moveSpeed;
        repath -= Time.deltaTime;
        if (repath > 0f) return;
        repath = RepathInterval;
        float d = sight.DistanceToPlayer();
        if (d < stats.preferredMin)
            agent.SetDestination(transform.position - sight.DirectionToPlayer() * stats.backStep);
        else if (d > stats.preferredMax)
            agent.SetDestination(sight.Player.position);
        else
            agent.ResetPath();
    }

    void Fire()
    {
        GameObject p = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(aimDir));
        p.GetComponent<Rigidbody>().linearVelocity = aimDir * stats.shotSpeed;
        if (shootClip != null) src.PlayVaried(shootClip);
    }

    void SetState(State next)
    {
        state = next;
        timer = 0f;
        lostTimer = 0f;
        repath = 0f;

        if (next != State.KeepDistance) agent.ResetPath();
        laser.enabled = next == State.Aim;
        if (next == State.Aim && aimClip != null) src.PlayVaried(aimClip);
        if (anim != null) anim.CrossFade(next == State.KeepDistance ? "Walk" : "Idle", 0.15f);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }
}
