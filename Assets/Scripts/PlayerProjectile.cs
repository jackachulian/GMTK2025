using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int maxBounces = 1;
    public float maxLifetime = 3f;
    
    private int bounces = 0; // starts at 0, increased after bouncing, destroy self after certain amount of bounces
    private Rigidbody2D rb;
    private float spawnedTime; // time object was spawned, will despawn if out for too long
    private Vector2 velocity;
    public Player2D player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnedTime = Time.time;
        velocity = rb.linearVelocity;
    }

    void Update()
    {
        velocity = rb.linearVelocity;

        if (Time.time - spawnedTime > maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float speed = velocity.magnitude;
        Vector3 direction = Vector3.Reflect(velocity.normalized, collision.contacts[0].normal);
        rb.linearVelocity = direction * speed;

        Debug.Log("Bounce!");
        bounces++;
        AudioManager.Instance.PlaySfx("ProjBounce");
        if (bounces >= maxBounces)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        player.projectilesActive -= 1;
    }
}
