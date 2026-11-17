using UnityEngine;

// Старая лошадь: ходит туда-обратно, бьёт касанием (тег Enemy -> Player3D).
public class Patrol : MonoBehaviour
{
    public EnemyStats stats;     // НОВОЕ: OldHorse_Stats, нужна только moveSpeed
    public Vector3 offset;       // куда ходит: раскладка уровня, значение стоит на объекте в сцене

    Vector3 start, last;

    void Start()
    {
        start = transform.position;
        last = start;
    }

    void Update()
    {
        float length = offset.magnitude;
        if (length < 0.01f) return;                                   // некуда идти
        float t = Mathf.PingPong(Time.time * stats.moveSpeed, length) / length; // 0..1..0, скорость в м/с
        Vector3 p = Vector3.Lerp(start, start + offset, t);
        Vector3 d = p - last;
        if (d.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(d);
        last = p;
        transform.position = p;
    }
}
