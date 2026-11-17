using UnityEngine;

// Движущаяся платформа. Путь и скорость - раскладка уровня, задаются на объекте в сцене.
public class MovingPlatform : MonoBehaviour
{
    public Vector3 offset;      // куда едет от стартовой точки
    public float speed;         // доля пути в секунду
    public Vector3 delta;       // сдвиг за последний шаг физики (читает Player3D)

    Vector3 start, last;

    void Start()
    {
        start = transform.position;
        last = start;
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 p = Vector3.Lerp(start, start + offset, t);
        delta = p - last;
        last = p;
        transform.position = p;
    }
}
