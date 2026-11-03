using UnityEngine;

// Внешний вид яйца. Физический шар (Player) крутится, а яйцо с лицом и ножками должно стоять ровно.
// На скорости ручки и ножки прячутся, и скорлупа вращается вместе с шаром (яйцо катится).
// Висит на EggHolder (дочерний объект Player). Иерархия: Player -> EggHolder -> EggMesh, Limbs.
public class EggVisual : MonoBehaviour
{
    public Transform mesh;          // EggMesh: скорлупа и глаза
    public Transform limbs;         // Limbs: ручки и ножки
    public float standHeight = 0.3f;// на сколько поднять яйцо, когда стоит на ножках
    public float rollOn = 4.5f;     // выше этой скорости начинаем катиться
    public float rollOff = 2.5f;    // ниже этой скорости встаём на ножки
    public float blendSpeed = 6f;   // скорость перехода между режимами (весь переход ~0.17 с)

    Transform body;                 // Player: физический шар
    Rigidbody rb;                   // его Rigidbody
    Player3D player;                // его скрипт управления (grounded, aim)
    float blend;                    // 0 = стоит, 1 = катится шаром, между ними плавный переход
    bool rolling;                   // текущий режим

    void Start()
    {
        body = transform.parent;                // родитель EggHolder = Player
        rb = body.GetComponent<Rigidbody>();    // компоненты берём у родителя
        player = body.GetComponent<Player3D>();
    }

    // LateUpdate идёт после всех Update и после Animator, поэтому последнее слово за этим скриптом
    void LateUpdate()
    {
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        float speed = v.magnitude;              // горизонтальная скорость шара

        // Начинаем катиться только на земле, а встаём всегда, как только замедлились.
        // Два разных порога (гистерезис), чтобы режим не мигал около одной скорости.
        if (!rolling && speed > rollOn && player.grounded) rolling = true;
        if (rolling && speed < rollOff) rolling = false;

        // Плавно двигаем blend к 1 (катимся) или к 0 (стоим)
        blend = Mathf.MoveTowards(blend, rolling ? 1f : 0f, blendSpeed * Time.deltaTime);

        Quaternion upright = Quaternion.LookRotation(player.aim);   // ровно, лицом по ходу движения
        transform.rotation = upright;                                // EggHolder всегда стоит ровно
        // Подъём вверх ПО МИРУ (position), а не по крутящемуся шару (localPosition), иначе яйцо уходило в пол
        transform.position = body.position + Vector3.up * standHeight * (1f - blend);
        // Скорлупа: blend=0 стоит ровно, blend=1 вращается как физический шар, между ними плавно
        mesh.rotation = Quaternion.Slerp(upright, body.rotation, blend);
        // Ручки и ножки: при стоянии масштаб 1, при качении 0 (спрятаны)
        limbs.localScale = Vector3.one * (1f - blend);
    }
}
