using UnityEngine;                    // MonoBehaviour, Vector3, Rigidbody, Physics
using UnityEngine.InputSystem;        // InputValue

// Управление яйцом: движение, прыжок, монеты, финиш, урон от касания врага, движущаяся платформа.
// Все числа баланса - в ассете PlayerStats (Assets/Data/Player_Stats).
public class Player3D : MonoBehaviour
{
    // Геометрия и ощущение, не баланс:
    const float GroundCheckDistance = 0.6f;   // луч вниз: радиус шара 0.5 + запас 0.1
    const float SpinDamping = 0.9f;           // как быстро гаснет вращение при торможении

    public PlayerStats stats;                 // НОВОЕ: перетащи Assets/Data/Player_Stats
    public Animator animator;                 // Animator на EggHolder (Speed, Grounded, Jump)
    public AudioClip jumpClip, coinClip;

    // Состояние (читают Hud, Shooter, EggVisual, LevelLoader)
    public int coins;                         // сколько монет собрано
    public int coinsTotal;                    // сколько монет на уровне всего
    public int coinsNeeded;                   // НОВОЕ: сколько нужно для финиша (из LevelData)
    public Vector3 aim = Vector3.forward;     // куда смотрит и стреляет
    public bool grounded;

    Rigidbody rb;
    AudioSource src;
    Transform cam;
    Vector2 move;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        src = GetComponent<AudioSource>();
        cam = Camera.main.transform;
    }

    void Start()
    {
        coinsTotal = GameObject.FindGameObjectsWithTag("Coin").Length;
        // НОВОЕ: цель уровня берём из LevelData через LevelLoader
        coinsNeeded = LevelLoader.Current != null ? LevelLoader.Current.CoinsNeeded(coinsTotal) : coinsTotal;
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())
        {
            rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.Impulse);   // НОВОЕ: stats
            animator.SetTrigger("Jump");
            src.PlayVaried(jumpClip);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down,
            GroundCheckDistance, ~0, QueryTriggerInteraction.Ignore);
    }

    // НОВОЕ: проверка падения ушла в LevelLoader (высота теперь в LevelData.fallY)
    void Update()
    {
        grounded = IsGrounded();
        Vector3 flat = rb.linearVelocity;
        flat.y = 0f;
        animator.SetFloat("Speed", flat.magnitude);
        animator.SetBool("Grounded", grounded);
    }

    void FixedUpdate()
    {
        Vector3 f = cam.forward; f.y = 0f; f.Normalize();
        Vector3 r = cam.right;   r.y = 0f; r.Normalize();
        Vector3 dir = f * move.y + r * move.x;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        rb.AddForce(dir * stats.force);                                  // НОВОЕ: stats

        if (dir.sqrMagnitude > 0.01f)
        {
            aim = dir.normalized;
        }
        else if (grounded)
        {
            Vector3 vel = rb.linearVelocity;
            rb.AddForce(new Vector3(-vel.x, 0f, -vel.z) * stats.brake, ForceMode.Acceleration); // НОВОЕ: stats
            rb.angularVelocity *= SpinDamping;
        }

        Vector3 v = rb.linearVelocity;
        Vector3 horizontal = new Vector3(v.x, 0f, v.z);
        if (horizontal.magnitude > stats.maxSpeed)                      // НОВОЕ: stats
        {
            horizontal = horizontal.normalized * stats.maxSpeed;
            rb.linearVelocity = new Vector3(horizontal.x, v.y, horizontal.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coins++;
            src.PlayVaried(coinClip);
        }

        if (other.CompareTag("Exit"))
        {
            if (LevelLoader.Current != null && LevelLoader.Current.Level.goal != LevelGoal.CollectCoins)
                Debug.Log("This level is won by beating the boss, not by the flag");  // НОВОЕ: цель из LevelData
            else if (coins < coinsNeeded)
                Debug.Log("Collect coins first! " + coins + " / " + coinsNeeded);
            else if (LevelLoader.Current != null)
                LevelLoader.Current.Complete();          // НОВОЕ: очки, сохранение, следующая сцена
            else
                Debug.LogWarning("No LevelLoader in this scene");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            GetComponent<Health>().TakeDamage(stats.contactDamage);   // НОВОЕ: stats
    }

    void OnCollisionStay(Collision c)
    {
        if (c.gameObject.CompareTag("Platform"))
        {
            MovingPlatform mp = c.gameObject.GetComponentInParent<MovingPlatform>();
            if (mp != null) rb.position += mp.delta;
        }
    }
}
