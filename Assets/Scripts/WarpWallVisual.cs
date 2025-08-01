using UnityEngine;

public class WarpWallVisual : MonoBehaviour
{
    [SerializeField] private float _fadeTime = 0.1f;
    private float _fadeTimer = 0f;
    private float targetAlpha = 0f;
    [SerializeField] SpriteRenderer spriteLeft;
    [SerializeField] SpriteRenderer spriteRight;
    [SerializeField] SpriteRenderer spriteTop;
    [SerializeField] SpriteRenderer spriteBottom;
    [SerializeField] AudioSource loopAudioSource;

    private Camera _camera;
    void Start()
    {
        GameManager.Instance.OnLevelChanged += Setup;
        _camera = Camera.main;
    }

    void Setup()
    {
        Debug.Log("Setup Wall Visual " + GameManager.currentLevel);
        if (GameManager.currentLevel == null) return;

        // subscribe to events
        Debug.Log(spriteLeft);
        GameManager.OnIsWarpingChanged += OnWarpChanged;
    }

    void SetPositions()
    {
       if (GameManager.currentLevel == null) return;
        var w = GameManager.currentLevel.levelSize.x;
        var h = GameManager.currentLevel.levelSize.y;

        // set up positions
        spriteLeft.sharedMaterial.mainTextureScale = new Vector2(3, h * 0.5f);
        spriteBottom.sharedMaterial.mainTextureScale = new Vector2(2, w * 0.5f);

        spriteLeft.transform.position = new Vector2(1.5f, h * 0.5f);
        spriteLeft.transform.localScale = new Vector2(3, h * 0.5f);

        spriteRight.transform.position = new Vector2(w - 1.5f, h * 0.5f);
        spriteRight.transform.localScale = new Vector2(3, h * 0.5f);

        
        spriteBottom.transform.position = new Vector2(w * 0.5f, 1f);
        spriteBottom.transform.localScale = new Vector2(2, w * 0.5f);

        spriteTop.transform.position = new Vector2(w * 0.5f, h - 1f);
        spriteTop.transform.localScale = new Vector2(2, w * 0.5f);
    }

    void SetPositionsAdaptive()
    {
        if (!GameManager.currentLevel) return;

        var w = GameManager.currentLevel.levelSize.x;
        var h = GameManager.currentLevel.levelSize.y;

        var vertExtent = 8.4375f;	
    	var horzExtent = vertExtent * Screen.width / Screen.height;

        var x = _camera.transform.position.x;
        var y = _camera.transform.position.y;

        // setup positions
        if(!spriteLeft) return;

        spriteLeft.sharedMaterial.mainTextureScale = new Vector2(3, h * 0.5f);
        spriteBottom.sharedMaterial.mainTextureScale = new Vector2(2, w * 0.5f);

        spriteLeft.transform.position = new Vector2(1.5f + x - horzExtent, y);
        spriteLeft.transform.localScale = new Vector2(3, h * 0.5f);

        spriteRight.transform.position = new Vector2(-1.5f + x + horzExtent, y);
        spriteRight.transform.localScale = new Vector2(3, h * 0.5f);
        
        spriteBottom.transform.position = new Vector2(x, 1f + y - vertExtent);
        spriteBottom.transform.localScale = new Vector2(2, w * 0.5f);

        spriteTop.transform.position = new Vector2(x, -1f + y + vertExtent);
        spriteTop.transform.localScale = new Vector2(2, w * 0.5f);
    }

    public void OnWarpChanged(bool b)
    {
        targetAlpha = b ? 0.5f : 0f;
        _fadeTimer = _fadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spriteLeft) return;

        _fadeTimer -= Time.deltaTime;
        if (_fadeTimer > -1)
        {
            float t = 1f - Mathf.Max(_fadeTimer / _fadeTime, 0);
            float a = Mathf.Lerp(spriteLeft.sharedMaterial.GetFloat("_AlphaScale"), targetAlpha, t);
            spriteLeft.sharedMaterial.SetFloat("_AlphaScale", a);
            spriteBottom.sharedMaterial.SetFloat("_AlphaScale", a);
            loopAudioSource.volume = a * 0.25f;
        }
    }
}
