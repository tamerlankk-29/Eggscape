using UnityEngine;
using UnityEngine.InputSystem;

// Стрельба яйца. Перезарядка, скорость и время жизни пули - в PlayerStats.
public class Shooter : MonoBehaviour
{
    // Геометрия: пуля появляется перед яйцом (радиус 0.5 + запас) и чуть выше центра
    const float SpawnForward = 0.9f;
    const float SpawnUp = 0.1f;

    public PlayerStats stats;            // НОВОЕ: Player_Stats
    public GameObject bulletPrefab;
    public AudioClip shootClip;

    float nextShot;
    AudioSource src;
    Player3D player;

    void Start()
    {
        src = GetComponent<AudioSource>();
        player = GetComponent<Player3D>();
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed) Shoot();
    }

    public void Shoot()
    {
        if (Time.time < nextShot) return;
        nextShot = Time.time + stats.shootCooldown;                     // НОВОЕ: stats

        Vector3 aim = player.aim;
        Vector3 spawnPos = transform.position + aim * SpawnForward + Vector3.up * SpawnUp;
        GameObject b = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(aim));
        b.GetComponent<Bullet>().Launch(stats.bulletSpeed, stats.bulletLifeTime); // НОВОЕ
        src.PlayOneShot(shootClip);
    }
}
