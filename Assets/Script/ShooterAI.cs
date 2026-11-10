using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Лама-стрелок. Держит дистанцию, целится красным лазером, плюётся, перезаряжается.
// Состояния: Idle -> KeepDistance -> Aim -> Cooldown -> KeepDistance ...
// Проверяет навык игрока: реакцию (увидел лазер - сдвинься в сторону).
[RequireComponent(typeof(NavMeshAgent), typeof(EnemySight))]
public class ShooterAI : MonoBehaviour
{
    enum State { Idle, KeepDistance, Aim, Cooldown }

    [Header("Дистанция")]
    [SerializeField] float preferredMin = 4f;     // м, ближе - отходит
    [SerializeField] float preferredMax = 8f;     // м, дальше - подходит
    [SerializeField] float moveSpeed = 2.5f;      // м/с
    [SerializeField] float turnSpeed = 240f;      // градусов в секунду
    [SerializeField] float loseTime = 3f;         // с без видимости -> Idle
    [Header("Выстрел")]
    [SerializeField] float aimTime = 0.6f;        // с, лазер перед выстрелом (предупреждение)
    [SerializeField] float shotSpeed = 6f;        // м/с, медленный - можно увернуться
    [SerializeField] float cooldown = 1.5f;       // с между выстрелами
    [SerializeField] float laserLength = 12f;     // м, длина лазера
    [SerializeField] GameObject projectilePrefab; // префаб Spit (EnemyProjectile)
    [SerializeField] Transform muzzle;            // точка у рта, откуда летит снаряд
    [SerializeField] LineRenderer laser;          // красная линия прицела
    [Header("Звук и отладка")]
    [SerializeField] AudioClip aimClip;           // звук начала прицела
    [SerializeField] AudioClip shootClip;         // звук плевка
    [SerializeField] Animator anim;
    [SerializeField] TMP_Text stateLabel;

    State state;
    float timer;          // секунд в текущем состоянии
    float lostTimer;      // секунд без видимости игрока
    float repath;         // секунд до следующего выбора точки
    Vector3 aimDir;       // куда полетит снаряд
    NavMeshAgent agent;
    EnemySight sight;
    AudioSource src;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
        src = GetComponent<AudioSource>();
        agent.updateRotation = false;   // поворачиваем сами: лама всегда смотрит на игрока, даже когда пятится
    }

    void Start()
    {
        agent.speed = moveSpeed;
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
                sight.FacePlayer(turnSpeed);
                KeepDistance();
                bool inRange = sight.DistanceToPlayer() <= preferredMax + 1f;
                if (sight.Sees && inRange && sight.IsOnScreen()) SetState(State.Aim); // не из-за кадра
                else if (lostTimer > loseTime) SetState(State.Idle);
                break;

            case State.Aim:                                   // стоит и целится - это предупреждение
                sight.FacePlayer(turnSpeed);
                aimDir = (sight.Player.position - muzzle.position).normalized;
                laser.SetPosition(0, muzzle.position);
                laser.SetPosition(1, muzzle.position + aimDir * laserLength);
                if (timer >= aimTime)
                {
                    Fire();
                    SetState(State.Cooldown);
                }
                break;

            case State.Cooldown:                              // перезарядка - окно игрока
                sight.FacePlayer(turnSpeed);
                if (timer >= cooldown) SetState(State.KeepDistance);
                break;
        }
    }

    // Раз в 0.3 с выбираем, куда идти: отойти, подойти или стоять
    void KeepDistance()
    {
        repath -= Time.deltaTime;
        if (repath > 0f) return;
        repath = 0.3f;
        float d = sight.DistanceToPlayer();
        if (d < preferredMin)
            agent.SetDestination(transform.position - sight.DirectionToPlayer() * 3f); // назад на 3 м
        else if (d > preferredMax)
            agent.SetDestination(sight.Player.position);                              // к игроку
        else
            agent.ResetPath();                                                        // стоять
    }

    void Fire()
    {
        GameObject p = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(aimDir));
        p.GetComponent<Rigidbody>().linearVelocity = aimDir * shotSpeed;   // как помидоры у Boss
        if (shootClip != null) src.PlayOneShot(shootClip);
    }

    void SetState(State next)
    {
        state = next;
        timer = 0f;
        lostTimer = 0f;
        repath = 0f;

        if (next != State.KeepDistance) agent.ResetPath();  // целится и перезаряжается стоя
        laser.enabled = next == State.Aim;                  // лазер только во время прицела
        if (next == State.Aim && aimClip != null) src.PlayOneShot(aimClip);
        if (anim != null) anim.CrossFade(next == State.KeepDistance ? "Walk" : "Idle", 0.15f);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }
}