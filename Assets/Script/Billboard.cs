using UnityEngine;

// Поворачивает 3D-текст ("!" или имя состояния) лицом к камере.
public class Billboard : MonoBehaviour
{
    Transform cam;
    void Start() { cam = Camera.main.transform; }
    void LateUpdate() { transform.rotation = cam.rotation; }   // после движения камеры
}