using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    private static bool _isWarping;
    public static bool IsWarping => _isWarping;
    
    public static event Action<bool> OnIsWarpingChanged;

    public static InputSystemActions actions;

    private void Awake()
    {
        Instance = this;

        actions = new InputSystemActions();
        actions.Player.Enable();

        actions.Player.Warp.performed += OnWarpPerformed;
        actions.Player.Warp.canceled += OnWarpCanceled;
    }

    public void OnWarpPerformed(InputAction.CallbackContext ctx)
    {
        if (_isWarping) return;
        Debug.Log("Warp enabled");
        _isWarping = true;
        OnIsWarpingChanged?.Invoke(_isWarping);
    }

    public void OnWarpCanceled(InputAction.CallbackContext ctx)
    {
        if (!_isWarping) return;
        Debug.Log("Warp disabled");
        _isWarping = false;
        OnIsWarpingChanged?.Invoke(_isWarping);
    }
}
