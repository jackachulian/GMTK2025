using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] private float _gravity;
    private Rigidbody2D _target = null;

    private BoxCollider2D _collider;
    private Rigidbody2D _rigidbody;
    [SerializeField] private LayerMask _groundMask;
    private float yvel = 0;
    [SerializeField] private float _maxFallSpeed = -50f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    public void FixedUpdate()
    {
        if (GroundCheck())
            yvel = 0f;
        
        yvel -= _gravity * Time.fixedDeltaTime;
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, Mathf.Max(yvel, _maxFallSpeed));
    }

    private bool GroundCheck()
    {
        float dist = _collider.size.y * 0.52f * transform.localScale.y;
        Vector2 origin = transform.position;
        Vector2 offset = transform.right * _collider.size.x / 2f * transform.localScale.x;
        // Check normal to allow slope sliding
        return
            Physics2D.Raycast(origin, Vector2.down, dist, _groundMask).normal == Vector2.up
            || Physics2D.Raycast(origin + offset, Vector2.down, dist, _groundMask).normal == Vector2.up
            || Physics2D.Raycast(origin - offset, Vector2.down, dist, _groundMask).normal == Vector2.up;
    }


}
