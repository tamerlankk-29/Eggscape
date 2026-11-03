using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -7f);
    public float smooth = 8f;

    void LateUpdate()
    {
        Vector3 want = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, want, smooth * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}
