using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private Transform _target = null;
    private Vector3 _lastPos = Vector3.zero;
    private new Collider2D collider;

    void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        
        // if(collision.gameObject.CompareTag("Player")) collision.gameObject.transform.parent = transform;
        float bottom = collision.transform.position.y - (collision.collider.bounds.max.y - collision.collider.bounds.min.y) / 2f;
        float top = transform.position.y + (collider.bounds.max.y - collider.bounds.min.y) / 2f;

        if(collision.gameObject.CompareTag("Player") && bottom > top) 
            _target = collision.transform;
        
        _lastPos = transform.position;
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
       _target = null;
    }

    public void FixedUpdate()
    {
        if (_target) _target.position += transform.position - _lastPos;

        _lastPos = transform.position;
    }


}
