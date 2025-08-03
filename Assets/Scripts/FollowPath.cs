using System.Collections;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector2[] _pointOffsets;
    [SerializeField] private bool closedLoop = true;
    [SerializeField] private int _currentPointIndex = 0; 
    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 offset = _pointOffsets[_currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, initialPos + offset, _speed * Time.fixedDeltaTime);
        if (Vector2.Distance(transform.position, initialPos + offset) < 0.001f)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _pointOffsets.Length;
            if (!closedLoop && _currentPointIndex == 0 && _pointOffsets.Length > 1)
            {
                transform.position = (Vector2)initialPos + _pointOffsets[0];
                _currentPointIndex = 1;
            }
        }
    }
}
