using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Свинья-таран. Состояния: Idle -> WindUp -> Dash -> Stunned -> Idle
// Все числа баланса - в ChargerStats (Pig_Stats).
[RequireComponent(typeof(NavMeshAgent), typeof(EnemySight))]
public class ChargerAI : MonoBehaviour
{
    enum State { Idle, WindUp, Dash, Stunned }

    // Картинка и техника, не баланс
    const float ShakeAmount = 0.05f;     // м, сила дрожи модели
    const float ShakeFrequency = 60f;    // скорость дрожи
    const float LaneLift = 0.05f;        // м, полоса чуть выше пола, чтобы не мерцала
    const float HomeArrive = 0.2f;       // м, «дошла домой»
    const float HomeGap = 0.5f;          // м, дальше этого от дома - идёт назад

    [SerializeField] ChargerStats stats;          // НОВОЕ: Pig_Stats
    [Header("Предупреждение и отладка")]
    [SerializeField] LineRenderer lane;
    [SerializeField] Transform model;
    [SerializeField] AudioClip windUpClip, hitClip;
    [SerializeField] Animator anim;
    [SerializeField] TMP_Text stateLabel;

    State state;
    float timer;
    float cooldownLeft;
    bool hitDone;
    Vector3 home;
    Vector3 dashDir;
    Vector3 modelPos;
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
        home = transform.position;               // дом = точка появления из LevelData
        modelPos = model.localPosition;
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
                agent.speed = stats.moveSpeed;
                if (agent.hasPath && !agent.pathPending && agent.remainingDistance < HomeArrive)
                {
                    agent.ResetPath();
                    if (anim != null) anim.CrossFade("Idle", 0.1f);
                }
                if (sight.Sees && !agent.hasPath) sight.FacePlayer(stats.watchTurnSpeed);
                if (sight.Sees && cooldownLeft <= 0f &&
                    sight.DistanceToPlayer() < stats.triggerRange && sight.IsOnScreen())
                    SetState(State.WindUp);
                break;

            case State.WindUp:
                if (timer < stats.windUp - stats.lockTime)
                {
                    sight.FacePlayer(stats.turnSpeed);
                    dashDir = transform.forward;
                }
                ShowLane();
                model.localPosition = modelPos + Vector3.right * Mathf.Sin(timer * ShakeFrequency) * ShakeAmount;
                if (timer >= stats.windUp) SetState(State.Dash);
                break;

            case State.Dash:
                agent.Move(dashDir * stats.dashSpeed * Time.deltaTime);
                if (!hitDone && sight.DistanceToPlayer() < stats.hitRadius) HitPlayer();
                if (timer >= stats.dashDistance / stats.dashSpeed) SetState(State.Stunned);
                break;

            case State.Stunned:
                if (timer >= stats.stunTime) SetState(State.Idle);
                break;
        }
    }

    void ShowLane()
    {
        Vector3 a = transform.position + Vector3.up * LaneLift;
        lane.SetPosition(0, a);
        lane.SetPosition(1, a + dashDir * stats.dashDistance);
    }

    void HitPlayer()
    {
        hitDone = true;
        sight.PlayerHealth.TakeDamage(stats.damage);
        Rigidbody rb = sight.Player.GetComponent<Rigidbody>();
        if (rb != null) rb.AddForce(dashDir * stats.knockback + Vector3.up * stats.knockUp, ForceMode.Impulse);
        if (hitClip != null) src.PlayVaried(hitClip);
    }

    void SetState(State next)
    {
        state = next;
        timer = 0f;
        model.localPosition = modelPos;
        lane.enabled = next == State.WindUp;
        agent.ResetPath();

        if (next == State.WindUp && windUpClip != null) src.PlayVaried(windUpClip);
        if (next == State.Dash) hitDone = false;
        bool goingHome = false;
        if (next == State.Idle)
        {
            cooldownLeft = stats.cooldown;
            goingHome = Vector3.Distance(transform.position, home) > HomeGap;
            if (goingHome) agent.SetDestination(home);
        }
        if (anim != null)
            anim.CrossFade(next == State.Dash ? "Run" : goingHome ? "Walk" : "Idle", 0.1f);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }
}
