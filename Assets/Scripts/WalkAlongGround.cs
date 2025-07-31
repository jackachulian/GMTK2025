using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class WalkAlongGround : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] private float _wallLookDist;
    [SerializeField] private float _groundLookDist;
    [SerializeField] bool _isVertical = false;
    [SerializeField] bool _verticalFlip = false;

    private Vector3 _groundDirection;
    private Vector3 _wallDirection;

    void Start()
    {
        _groundDirection = (_isVertical ? Vector2.left : Vector2.down) * (_verticalFlip ? -1 : 1);
        _wallDirection = _isVertical ? Vector2.up : Vector2.right;

    }

    void Update()
    {
        transform.position += _speed * Time.deltaTime * _wallDirection;
        // check if wall exists in front
        bool wall = Physics2D.Raycast(transform.position, _wallDirection * Mathf.Sign(_speed), _wallLookDist, _layerMask);
        // check if ground exists
        Vector2 frontPosition = LevelWrap.WrapPosition(transform.position + _wallLookDist * Mathf.Sign(_speed) * _wallDirection);
        bool floor = Physics2D.Raycast(frontPosition, _groundDirection, _groundLookDist, _layerMask);

        // turn around
        if (wall || !floor)
        {
            _speed *= -1;
        }
    }


}