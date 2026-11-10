using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Свинья-таран. Ждёт, замахивается (дрожит + красная полоса), несётся по прямой, потом оглушена.
// Состояния: Idle -> WindUp -> Dash -> Stunned -> Idle
// Проверяет навык игрока: тайминг (уйти с полосы в конце замаха, бить, пока оглушена).
[RequireComponent(typeof(NavMeshAgent), typeof(EnemySight))]
public class ChargerAI : MonoBehaviour
{
    enum State { Idle, WindUp, Dash, Stunned }

    [Header("Замах")]
    [SerializeField] float triggerRange = 7f;     // м, с какого расстояния начинает замах
    [SerializeField] float windUp = 0.7f;         // с, предупреждение
    [SerializeField] float lockTime = 0.2f;       // с до конца замаха, когда направление фиксируется
    [Header("Рывок")]
    [SerializeField] float dashSpeed = 12f;       // м/с
    [SerializeField] float dashDistance = 9f;     // м
    [SerializeField] float hitRadius = 2.0f;      // м между центрами: полсвиньи ~1.3 + яйцо 0.5 + запас
    [SerializeField] float knockback = 4f;        // сила толчка яйца (больше - может сбросить с арены)
    [SerializeField] int damage = 1;
    [Header("После атаки")]
    [SerializeField] float stunTime = 1.2f;       // с, окно для ответа игрока
    [SerializeField] float cooldown = 1.5f;       // с в Idle до следующего замаха
    [SerializeField] float walkSpeed = 2f;        // м/с, возвращение на место
    [Header("Предупреждение и отладка")]
    [SerializeField] LineRenderer lane;           // красная полоса на земле - куда побежит
    [SerializeField] Transform model;             // дочерняя модель - её трясём
    [SerializeField] AudioClip windUpClip, hitClip;
    [SerializeField] Animator anim;
    [SerializeField] TMP_Text stateLabel;

    State state;
    float timer;          // секунд в текущем состоянии
    float cooldownLeft;   // сколько ещё нельзя атаковать
    bool hitDone;         // уже ударили в этом рывке (один удар за рывок)
    Vector3 home;         // точка, куда возвращается
    Vector3 dashDir;      // направление рывка
    Vector3 modelPos;     // обычное положение модели (для дрожи)
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
        home = transform.position;
        modelPos = model.localPosition;
        agent.speed = walkSpeed;
        dashDir = transform.forward;
        SetState(State.Idle);
    }

    void Update()
    {
        timer += Time.deltaTime;
        cooldownLeft -= Time.deltaTime;
        switch (state)
        {
            case State.Idle:
                if (agent.hasPath && !agent.pathPending && agent.remainingDistance < 0.2f)
                {
                    agent.ResetPath();                         // дошла домой
                    if (anim != null) anim.CrossFade("Idle", 0.1f);
                }
                if (sight.Sees && !agent.hasPath) sight.FacePlayer(180f);   // следит глазами
                if (sight.Sees && cooldownLeft <= 0f &&
                    sight.DistanceToPlayer() < triggerRange && sight.IsOnScreen())
                    SetState(State.WindUp);
                break;

            case State.WindUp:                                 // стоит, дрожит, показывает полосу
                if (timer < windUp - lockTime)
                {
                    sight.FacePlayer(360f);                    // пока ещё поворачивается за игроком
                    dashDir = transform.forward;
                }                                              // последние lockTime секунд - направление замерло
                ShowLane();
                model.localPosition = modelPos + Vector3.right * Mathf.Sin(timer * 60f) * 0.05f; // дрожь
                if (timer >= windUp) SetState(State.Dash);
                break;

            case State.Dash:
                agent.Move(dashDir * dashSpeed * Time.deltaTime);   // по прямой, но не слетает с NavMesh
                if (!hitDone && sight.DistanceToPlayer() < hitRadius) HitPlayer();
                if (timer >= dashDistance / dashSpeed) SetState(State.Stunned);
                break;

            case State.Stunned:                                // окно игрока: стоит, её можно бить
                if (timer >= stunTime) SetState(State.Idle);
                break;
        }
    }

    // Красная полоса на земле от свиньи на всю длину рывка
    void ShowLane()
    {
        Vector3 a = transform.position + Vector3.up * 0.05f;   // чуть выше пола, чтобы не мерцала
        lane.SetPosition(0, a);
        lane.SetPosition(1, a + dashDir * dashDistance);
    }

    void HitPlayer()
    {
        hitDone = true;
        sight.PlayerHealth.TakeDamage(damage);
        Rigidbody rb = sight.Player.GetComponent<Rigidbody>();
        if (rb != null) rb.AddForce(dashDir * knockback + Vector3.up * 2f, ForceMode.Impulse); // толчок
        if (hitClip != null) src.PlayOneShot(hitClip);
    }

    void SetState(State next)
    {
        state = next;
        timer = 0f;
        model.localPosition = modelPos;           // перестать дрожать
        lane.enabled = next == State.WindUp;      // полоса только во время замаха
        agent.ResetPath();                        // замах, рывок и оглушение - без пути агента

        if (next == State.WindUp && windUpClip != null) src.PlayOneShot(windUpClip);
        if (next == State.Dash) hitDone = false;
        bool goingHome = false;
        if (next == State.Idle)
        {
            cooldownLeft = cooldown;
            goingHome = Vector3.Distance(transform.position, home) > 0.5f;
            if (goingHome) agent.SetDestination(home);   // идёт назад на своё место
        }
        if (anim != null)
            anim.CrossFade(next == State.Dash ? "Run" : goingHome ? "Walk" : "Idle", 0.1f);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }
}