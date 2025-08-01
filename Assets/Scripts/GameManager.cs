using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    public bool IsScrollingLevel {get; private set;}

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

        IsScrollingLevel = GameObject.Find("Main Camera").GetComponent<CameraController>() != null;
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
        StartCoroutine(NextLevel());
    }

    private IEnumerator NextLevel()
    {
        var map = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        AudioManager.Instance.PlaySfx("Transition");
        int count = map.size.x * map.size.y / 4;
        // randomize tiles
        for(int i = 0; i < count; i++)
        {
            ReplaceRandomTile(map);
        }
        yield return new WaitForSeconds(0.25f);
        // randomize tiles
        for(int i = 0; i < count; i++)
        {
            ReplaceRandomTile(map);
        }
        yield return new WaitForSeconds(0.25f);
        // randomize tile
        for(int i = 0; i < count; i++)
        {
            ReplaceRandomTile(map);
        }
        yield return new WaitForSeconds(0.25f);
        // randomize tile
        for(int i = 0; i < count; i++)
        {
            ReplaceRandomTile(map);
        }
        if (CurrentLevelIndex >= levels.Length) SceneManager.LoadScene("End");
        else SceneManager.LoadScene(levels[CurrentLevelIndex].name);
    }

    private void ReplaceRandomTile(Tilemap map)
    {
        map.SetTile(
            new Vector3Int(UnityEngine.Random.Range(0, map.size.x), UnityEngine.Random.Range(0, map.size.y)), 
            null
        );
    }

    public void ClosePauseMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
