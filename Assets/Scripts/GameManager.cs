using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public AudioManager audioManager;

    private static bool _isWarping;
    public static bool IsWarping => _isWarping;

    public static event Action<bool> OnIsWarpingChanged;

    public static InputSystemActions actions;
    

    public int CurrentLevelIndex { get; private set; } = 0;

    public SceneAsset[] levels;

    public event Action OnLevelChanged;

    private static Level _currentLevel;
    public static Level currentLevel
    {
        get {return _currentLevel;}
        set
        {
            _currentLevel = value;
            Debug.Log("Change Level");
            Instance.OnLevelChanged?.Invoke();
        }
    }

    public Texture2D cursorLocked, cursorShoot;
    public GameObject pauseMenu;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        CurrentLevelIndex = 0;

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

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LevelSelect(int level)
    {
        CurrentLevelIndex = level;
        SceneManager.LoadScene(levels[CurrentLevelIndex].name);
    }

    public void EndLevel()
    {
        CurrentLevelIndex++;
        Debug.Log(CurrentLevelIndex);
        if (CurrentLevelIndex >= levels.Length) SceneManager.LoadScene("End");
        else SceneManager.LoadScene(levels[CurrentLevelIndex].name);

    }

    public void ClosePauseMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
