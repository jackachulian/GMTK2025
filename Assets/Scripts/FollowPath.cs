using System.Collections;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector2[] _pointOffsets;
    private int _currentPointIndex = 0;
    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        Vector3 offset = _pointOffsets[_currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, initialPos + offset, _speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, initialPos + offset) < 0.01f)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _pointOffsets.Length;
        }
    }
}
