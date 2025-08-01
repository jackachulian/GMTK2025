using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollapsibleTile : MonoBehaviour
{
    public int _fallDelayMilliseconds = 1000;
    public float fallGravity = 10f;
    public float respawnTime = 5f;
    public float shakeStrength = 0.25f;
    public Collider2D collider;
    public SpriteRenderer spriteRenderer;
    
    private bool _willCollapse = false;
    private bool _collapsing = false;
    private Vector3 _currentVel = Vector3.zero;
    private float _respawnTimer;
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = collider.transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player2D>(out var player))
        {
            CollapseAfterDelay();
        }
    }

    async void CollapseAfterDelay()
    {
        if (_willCollapse) return;
        
        Debug.Log("Collapsing after delay", this);
        _willCollapse = true;
        
        await Task.Delay(_fallDelayMilliseconds);

        Debug.Log("Collapsing now", this);
        _willCollapse = false;
        _collapsing = true;
        _currentVel = Vector3.zero;
        _respawnTimer = 0f;
        spriteRenderer.transform.localPosition = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (_willCollapse)
        {
            if (spriteRenderer) spriteRenderer.transform.localPosition = Random.insideUnitCircle * shakeStrength;
        }
        
        if (_collapsing)
        {
            _respawnTimer += Time.fixedDeltaTime;
            if (_respawnTimer > respawnTime)
            {
                Respawn();
                return;
            }
        
            _currentVel += Vector3.down * (fallGravity * Time.fixedDeltaTime);
            collider.transform.localPosition += _currentVel * Time.fixedDeltaTime;
        }
    }

    void Respawn()
    {
        Debug.Log("Respawned", this);
        _willCollapse = false;
        _collapsing = false;
        _respawnTimer = 0f;
        _currentVel = Vector3.zero;
        collider.transform.position = _initialPosition;
    }
}