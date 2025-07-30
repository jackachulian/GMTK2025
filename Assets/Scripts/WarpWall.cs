using System;
using UnityEngine;

public class WarpWall : MonoBehaviour
{
    [SerializeField] private Transform wall;
    [SerializeField] private Vector2 disableOffset;
    [SerializeField] private float moveSpeed = 10f;

    private Vector3 _currentTargetLocalPosition;

    private void OnEnable()
    {
        GameManager.OnIsWarpingChanged += OnIsWarpingChanged;
    }
        
    private void OnDisable()
    {
        GameManager.OnIsWarpingChanged += OnIsWarpingChanged;
    }

    private void FixedUpdate()
    {
        if (_currentTargetLocalPosition == wall.transform.localPosition) return;
        Vector2 newPos = Vector2.MoveTowards(wall.transform.localPosition, _currentTargetLocalPosition, moveSpeed * Time.deltaTime);
        wall.transform.localPosition = newPos;
    }

    public void OnIsWarpingChanged(bool isWarping)
    {
        if (isWarping)
            DisableWall();
        else
            EnableWall();
    }

    public void EnableWall()
    {
        _currentTargetLocalPosition = Vector2.zero;
    }
        
    public void DisableWall()
    {
        _currentTargetLocalPosition = disableOffset;
    }
}