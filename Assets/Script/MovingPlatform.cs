using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 0f, 6f);
    public float speed = 0.3f;
    public Vector3 delta;

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
