using UnityEngine;

// Дождь опасностей вокруг яйца. Что, как часто и где - берёт из LevelData.
// Запускает LevelLoader методом Begin.
public class Spawner : MonoBehaviour
{
    public Transform player;      // яйцо (уже перетащено в сценах)
    LevelData level;

    public void Begin(LevelData data)
    {
        level = data;
        if (level.hazardPrefabs == null || level.hazardPrefabs.Length == 0) return;   // на уровне ничего не падает
        InvokeRepeating(nameof(Spawn), level.hazardFirstDelay, level.hazardInterval);
    }

    void Spawn()
    {
        if (player.position.z > level.hazardStopZ) return;   // на арене с врагами ничего не роняем

        float x = player.position.x + Random.Range(-level.hazardRange, level.hazardRange);
        float z = player.position.z + Random.Range(-level.hazardRange, level.hazardRange);
        Hazard pick = level.hazardPrefabs[Random.Range(0, level.hazardPrefabs.Length)];
        Instantiate(pick, new Vector3(x, player.position.y + level.hazardHeight, z), Quaternion.identity);
    }
}
