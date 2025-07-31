using System;
using UnityEngine;
 
public class WarpWall : MonoBehaviour
{
    [SerializeField] private GameObject wall;

    private Vector3 _currentTargetLocalPosition;

    private void OnEnable()
    {
        GameManager.OnIsWarpingChanged += OnIsWarpingChanged;
    }
        
    private void OnDisable()
    {
        GameManager.OnIsWarpingChanged -= OnIsWarpingChanged;
    }
    
    public void OnIsWarpingChanged(bool isWarping)
    {
        if (!gameObject || !wall) return;
        
        if (isWarping)
            DisableWall();
        else
            EnableWall();
    }

    public void EnableWall()
    {
        wall.SetActive(true);
    }
        
    public void DisableWall()
    {
        wall.SetActive(false);
    }
}