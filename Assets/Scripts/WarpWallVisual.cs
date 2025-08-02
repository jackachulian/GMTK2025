using UnityEngine;

public class WarpWallVisual : MonoBehaviour
{
    [SerializeField] private float _fadeTime = 0.1f;
    [SerializeField] private float _fadeToAlpha = 0.3f;
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
        // This script has been repeatidly stuggling to keep references to these for their whole development, i dont get it 
        spriteLeft = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteBottom = transform.GetChild(1).GetComponent<SpriteRenderer>();
        spriteTop = transform.GetChild(2).GetComponent<SpriteRenderer>();
        spriteRight = transform.GetChild(3).GetComponent<SpriteRenderer>();
        
        Setup();
        _camera = Camera.main;
    }

    void Setup()
    {
        // subscribe to events
        Debug.Log(spriteLeft);
        GameManager.OnIsWarpingChanged += OnWarpChanged;
    }

    public void OnWarpChanged(bool b)
    {
        targetAlpha = b ? _fadeToAlpha : 0f;
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
