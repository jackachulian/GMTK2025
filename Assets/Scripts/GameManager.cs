using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _isWarping;

    public bool IsWarping
    {
        get => _isWarping;
        set
        {
            _isWarping = value;
            OnIsWarpingChanged?.Invoke(value);
        }
    }
    public event Action<bool> OnIsWarpingChanged;
}
