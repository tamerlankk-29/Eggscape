using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float cooldown = 0.3f;
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
        if (value.isPressed)
            Shoot();
    }

    public void Shoot()
    {
        if (Time.time < nextShot) return;
        nextShot = Time.time + cooldown;

        Vector3 aim = player.aim;
        Vector3 spawnPos = transform.position + aim * 0.9f + Vector3.up * 0.1f;
        Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(aim));
        src.PlayOneShot(shootClip);
    }
}
