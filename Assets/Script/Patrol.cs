using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Vector3 offset = new Vector3(5f, 0f, 0f);
    public float speed = 0.4f;

    Vector3 start, last;

    void Start()
    {
        start = transform.position;
        last = start;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 p = Vector3.Lerp(start, start + offset, t);
        Vector3 d = p - last;
        if (d.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(d);
        last = p;
        transform.position = p;
    }
}
