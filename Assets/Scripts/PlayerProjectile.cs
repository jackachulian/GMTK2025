using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private int bounces = 0; // starts at 0, increased after bouncing, destroy self after certain amount of bounces
    private Rigidbody2D rb;
    private float spawnedTime; // time object was spawned, will despawn if out for too long
    private Vector2 velocity;
    public Player2D player;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        spawnedTime = Time.time;
        velocity = rb.linearVelocity;
    }

    void Update()
    {
        velocity = rb.linearVelocity;

        if (Time.time - spawnedTime > 10)
        {
            player.projectilesActive -= 1;
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
        if (bounces > 3)
        {
            player.projectilesActive -= 1;
            Destroy(gameObject);
        }
    }




}
