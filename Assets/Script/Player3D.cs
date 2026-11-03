using UnityEngine;                    // основные классы Unity: MonoBehaviour, Vector3, Rigidbody, Physics
using UnityEngine.InputSystem;        // новая система ввода: тип InputValue
using UnityEngine.SceneManagement;    // SceneManager: загрузка сцен (рестарт, следующий уровень)

// Управление яйцом: движение, прыжок, монеты, финиш, урон от врагов, движущаяся платформа.
// Висит на объекте Player (вместе с Rigidbody, SphereCollider, AudioSource, PlayerInput, Shooter, Health).
public class Player3D : MonoBehaviour
{
    // ---------- Настройки (видны в Inspector) ----------
    public float force = 14f;                 // сила разгона
    public float maxSpeed = 8f;               // максимальная горизонтальная скорость
    public float jumpForce = 7f;              // сила прыжка (при массе 1 подлёт примерно на 2.5)
    public float groundCheckDistance = 0.6f;  // длина луча вниз: радиус шара 0.5 + запас 0.1
    public float brake = 4f;                  // сила торможения, когда кнопки отпущены
    public Animator animator;                 // Animator на EggHolder (параметры Speed, Grounded, Jump)
    public string nextScene = "Level2";       // какая сцена загрузится на финише (своя на каждом уровне)
    public AudioClip jumpClip, coinClip;      // звуки прыжка и монеты

    // ---------- Состояние (public, чтобы читали Hud, Shooter, EggVisual) ----------
    public int coins;                         // сколько монет собрано
    public int coinsTotal;                    // сколько монет на уровне всего
    public Vector3 aim = Vector3.forward;     // куда смотрит яйцо и куда стреляет
    public bool grounded;                     // стоим ли на земле (обновляется каждый кадр)

    // ---------- Внутренние ссылки (private) ----------
    Rigidbody rb;                             // физика яйца
    AudioSource src;                          // источник звука
    Transform cam;                            // камера: движение считается относительно неё
    Vector2 move;                             // последний ввод: x = влево/вправо, y = вперёд/назад

    // Один раз при создании объекта: получаем ссылки на компоненты
    void Awake()
    {
        rb = GetComponent<Rigidbody>();       // Rigidbody на этом же объекте
        src = GetComponent<AudioSource>();    // AudioSource на этом же объекте
        cam = Camera.main.transform;          // камера с тегом MainCamera
    }

    // Перед первым кадром: считаем все монеты на уровне по тегу Coin
    void Start()
    {
        coinsTotal = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    // Вызывает PlayerInput (Send Messages) при изменении действия Move (стик, WASD)
    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();          // только запоминаем, двигаем в FixedUpdate
    }

    // Вызывает PlayerInput на действие Jump (и при нажатии, и при отпускании)
    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())  // только нажатие (не отпускание) и только с земли
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // мгновенный толчок вверх
            animator.SetTrigger("Jump");      // включаем анимацию прыжка (Any State -> Egg_Jump)
            src.PlayOneShot(jumpClip);        // звук прыжка
        }
    }

    // Проверка земли: луч из центра яйца вниз
    bool IsGrounded()
    {
        // ~0 = все слои; триггеры (монеты, шипы) не считаются землёй.
        // Луч начинается внутри своего коллайдера, поэтому сам себя не задевает.
        return Physics.Raycast(transform.position, Vector3.down,
            groundCheckDistance, ~0, QueryTriggerInteraction.Ignore);
    }

    // Каждый кадр: падение в пропасть, проверка земли, данные для Animator
    void Update()
    {
        if (transform.position.y < -10f)      // упали в пропасть
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // рестарт уровня

        grounded = IsGrounded();              // сохраняем, чтобы читал EggVisual

        Vector3 flat = rb.linearVelocity;     // скорость шара
        flat.y = 0f;                          // берём только горизонтальную часть
        animator.SetFloat("Speed", flat.magnitude); // Idle <-> Run по порогу 0.3
        animator.SetBool("Grounded", grounded);     // Jump -> Idle при приземлении
    }

    // Каждый шаг физики (50 раз в секунду): силы к Rigidbody добавляются здесь
    void FixedUpdate()
    {
        // Направления камеры по земле: убираем наклон по Y и делаем длину 1
        Vector3 f = cam.forward; f.y = 0f; f.Normalize();
        Vector3 r = cam.right;   r.y = 0f; r.Normalize();
        Vector3 dir = f * move.y + r * move.x;      // "вверх на стике" = "от камеры"
        if (dir.sqrMagnitude > 1f) dir.Normalize(); // по диагонали не быстрее

        rb.AddForce(dir * force);             // толкаем шар, он катится за счёт трения

        if (dir.sqrMagnitude > 0.01f)         // игрок что-то нажимает
        {
            aim = dir.normalized;             // запоминаем направление взгляда и выстрела
        }
        else if (grounded)                    // кнопки отпущены и мы на земле: тормозим
        {
            Vector3 vel = rb.linearVelocity;
            rb.AddForce(new Vector3(-vel.x, 0f, -vel.z) * brake, ForceMode.Acceleration); // сила против движения
            rb.angularVelocity *= 0.9f;       // гасим вращение шара
        }

        // Ограничение горизонтальной скорости (вертикальную не трогаем, чтобы падать и прыгать)
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);
        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
        }
    }

    // Вошли в триггер (Is Trigger): монета или финиш
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))         // монета
        {
            Destroy(other.gameObject);        // убираем монету
            coins++;                          // +1 к счётчику
            src.PlayOneShot(coinClip);        // звук монеты
        }

        if (other.CompareTag("Exit"))         // флаг финиша
        {
            if (coins >= coinsTotal)          // собраны все монеты
                SceneManager.LoadScene(nextScene);   // следующий уровень
            else
                Debug.Log("Collect all coins first! " + coins + " / " + coinsTotal);
        }
    }

    // Физическое столкновение: враг отнимает жизнь
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))   // лошадь или корова
            GetComponent<Health>().TakeDamage(1);   // связь со скриптом Health через GetComponent
    }

    // Пока касаемся платформы: яйцо едет вместе с движущейся платформой
    void OnCollisionStay(Collision c)
    {
        if (c.gameObject.CompareTag("Platform"))
        {
            // скрипт может висеть на родителе (касаемся дочернего блока Ground)
            MovingPlatform mp = c.gameObject.GetComponentInParent<MovingPlatform>();
            if (mp != null) rb.position += mp.delta; // сдвигаемся на столько же, на сколько сдвинулась платформа
        }
    }
}
