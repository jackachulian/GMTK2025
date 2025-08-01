using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private Sprite[] _tileSprites;

    public bool IsScrollingLevel {get {return GameObject.Find("Main Camera").GetComponent<CameraController>() != null;}}

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
        if (Instance == this) Instance = null;
    }

    public void OnWarpPerformed(InputAction.CallbackContext ctx)
    {
        if (_isWarping) return;
        Debug.Log("Warp enabled");
        AudioManager.Instance.PlaySfx("WarpStart");
        _isWarping = true;
        OnIsWarpingChanged?.Invoke(_isWarping);
    }

    public void OnWarpCanceled(InputAction.CallbackContext ctx)
    {
        if (!_isWarping) return;
        Debug.Log("Warp disabled");
        AudioManager.Instance.PlaySfx("WarpEnd");
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
        // Replace tiles in tilemap with randomized ones for glitchy transition effect
        var map = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        AudioManager.Instance.PlaySfx("Transition");
        int count = map.size.x * map.size.y / 4;

        // create randomized list of coordinates to iterate
        List<Vector3Int> coords = new();
        for(int i = 0; i < map.size.x; i++)
        {
            for (int j = 0; j < map.size.y; j++)
            {
                coords.Add(new Vector3Int(i, j));
            }
        }
        coords = coords.OrderBy( x => UnityEngine.Random.value ).ToList();
        
        RandomizeTilesHelper(map, coords, count, 0);
        yield return new WaitForSeconds(0.25f);
        RandomizeTilesHelper(map, coords, count, count);
        yield return new WaitForSeconds(0.25f);
        RandomizeTilesHelper(map, coords, count, count * 2);
        yield return new WaitForSeconds(0.25f);
        RandomizeTilesHelper(map, coords, count, count * 3);
        if (CurrentLevelIndex >= levels.Length) SceneManager.LoadScene("End");
        else SceneManager.LoadScene(levels[CurrentLevelIndex].name);
    }

    private void RandomizeTilesHelper(Tilemap map, List<Vector3Int> coords, int count, int offset)
    {
        for (int i = 0; i < count; i++)
        {
            ReplaceTile(map, coords[i + offset]);
        }
        map.RefreshAllTiles();
    }

    private void ReplaceTile(Tilemap map, Vector3Int coord)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = _tileSprites[UnityEngine.Random.Range(0, _tileSprites.Length)];

        map.SetTile(
            coord, 
            tile
        );
    }

    public void ClosePauseMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
