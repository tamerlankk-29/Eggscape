using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Hazard[] prefabs;
    public Transform player;
    public float interval = 2f;
    public float range = 5f;
    public float height = 10f;
    public float stopZ = 56f;   // НОВОЕ: дальше этой z (арена) камни не падают - там работают враги

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, interval);
    }

    void Spawn()
    {
        if (player.position.z > stopZ) return;   // НОВОЕ: яйцо на арене - ничего не роняем

        float x = player.position.x + Random.Range(-range, range);
        float z = player.position.z + Random.Range(-range, range);

        Hazard pick = prefabs[Random.Range(0, prefabs.Length)];
        Instantiate(pick, new Vector3(x, player.position.y + height, z), Quaternion.identity);
    }
}