using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    private static bool _isWarping;
    public static bool IsWarping => _isWarping;
    
    public static event Action<bool> OnIsWarpingChanged;

    public static InputSystemActions actions;

    public static int CurrentLevelIndex {get; private set;} = 0;

    public SceneAsset[] levels;

    public static Level currentLevel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        actions = new InputSystemActions();
        actions.Player.Enable();
    }

    private void OnEnable()
    {
        actions.Player.Warp.performed += OnWarpPerformed;
        actions.Player.Warp.canceled += OnWarpCanceled;
    }
    
    private void OnDisable()
    {
        actions.Player.Warp.performed -= OnWarpPerformed;
        actions.Player.Warp.canceled -= OnWarpCanceled;
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

    public void StartGame()
    {
        CurrentLevelIndex = 0;
        SceneManager.LoadScene(levels[0].name);
    }

    public void EndLevel()
    {
        actions.Player.Disable();
        CurrentLevelIndex++;
        Debug.Log(CurrentLevelIndex);
        if (CurrentLevelIndex >= levels.Length) SceneManager.LoadScene("End");
        else SceneManager.LoadScene(levels[CurrentLevelIndex].name);
        
    }
}
