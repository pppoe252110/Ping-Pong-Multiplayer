using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallController : NetworkBehaviour
{
    private AudioSource audioSource;
    public float startDelay = 1f;
    public float speed = 3f;
    public float startAngle = 45f;
    private Rigidbody2D rb;
    private Vector2 direction;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(WaitForSpawn());
    }

    IEnumerator WaitForSpawn()
    {
        rb.velocity = Vector2.zero;
        rb.position = Vector2.zero;
        yield return new WaitForSeconds(startDelay);

        var dir = math.remap(0, 1, 0, 180, Random.Range(0, 2));
        var dir1 = math.remap(0, 1, -startAngle, startAngle, Random.value);

        direction = Quaternion.Euler(0, 0, dir1 + dir) * Vector2.up;
        UpdateRBSpeed();

    }

    public void UpdateRBSpeed()
    {
        rb.velocity = direction * speed;

    }

    private void Update()
    {
        if(rb.velocity.magnitude>0)
        UpdateRBSpeed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.TryGetComponent(out PaddleController paddle))
        {
            Vector2 dir = transform.position - paddle.transform.position;
            dir.Normalize();

            direction = dir;
            UpdateRBSpeed();
            RPC_OnHit();
        }
        else if (collision.transform.TryGetComponent(out AddPoints addPoints))
        {
            addPoints.onHit?.Invoke();

            StartCoroutine(WaitForSpawn());
        }
        else
        {
            Vector2 collideNormal = collision.contacts[0].normal;

            direction = Vector2.Reflect(direction, collideNormal);
            UpdateRBSpeed();
            RPC_OnHit();
        }

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnHit()
    {
        audioSource.pitch = 1f + Random.value * 0.1f;
        audioSource.Play();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        Gizmos.DrawLine(transform.position, Quaternion.Euler(0, 0, startAngle) * Vector2.up);
        Gizmos.DrawLine(transform.position, Quaternion.Euler(0, 0, -startAngle) * Vector2.up);
        Gizmos.DrawLine(transform.position, Quaternion.Euler(0, 0, -startAngle) * Vector2.down);
        Gizmos.DrawLine(transform.position, Quaternion.Euler(0, 0, startAngle) * Vector2.down);
    }
}
