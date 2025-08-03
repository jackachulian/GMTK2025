using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player2D : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] BoxCollider2D _collider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private bool jumpHeld = false;
    private bool jumpedThisInput = false;
    private bool groundedLastTick = false;
    private bool grounded = false;

    public bool shootingUnlocked; // is set to true if you are allowed to shoot
    [SerializeField] private float projectileSpeed;
    [SerializeField] private Rigidbody2D projectile;
    public int projectilesActive; // how many projectiles you currently have spawned
    [SerializeField] private int maxProjectilesActive; // the max of how many projectiles you are allowed to have spawned at once

    [SerializeField] private float _groundAcceleration;
    [SerializeField] private float _groundMaxVel;
    [SerializeField] private float _friction;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpVel;
    [SerializeField] private float _jumpControl;

    // time of reduced air control after walljump
    [SerializeField] private float _wallJumpRecoveryTime;
    [SerializeField] private float _wallJumpLateralForce;
    [SerializeField] private float _wallSlideSpeed;

    private float _wallJumpTimer;
    // private bool _inWallslide;

    [SerializeField] private LayerMask _groundMask;

    private Vector2 _moveInputDir = Vector3.zero;
    private Vector2 lateralVector = new(1, 0);

    // Extra force caused by other objects, to be applied next tick
    private Vector2 applyForce = Vector3.zero;

    private Vector2 spawnPosition;

    private bool respawnedThisTick;

    private Vector2 _lastDelta;

    private bool bouncing = false;

    private AudioSource _wallSlideAudioSource;

    private PlayerInput _playerInput;

    [SerializeField] private ParticleSystem _deathParticleSystem;

    void Start()
    {
        GetComponent<PlayerInput>().enabled = true;
        spawnPosition = transform.position;

        if (shootingUnlocked) Cursor.SetCursor(GameManager.Instance.cursorShoot, new Vector2(0, 0), CursorMode.Auto);
        else Cursor.SetCursor(GameManager.Instance.cursorLocked, new Vector2(0, 0), CursorMode.Auto);

        _wallSlideAudioSource = GetComponent<AudioSource>();

        _playerInput = GetComponent<PlayerInput>();
        _playerInput.enabled = true;

        GameManager.Instance.player = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && shootingUnlocked && maxProjectilesActive > projectilesActive)
        {
            if(GameManager.Instance.winMenuOpen == false) Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.winMenuOpen == false)
            {
                GameManager.Instance.pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.R)) // reset scene
        {
            if (GameManager.Instance.winMenuOpen == false)
            {
                if (transform.parent != null)
                {
                    transform.parent.transform.DetachChildren();
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        
    }
    
    private static readonly int Running = Animator.StringToHash("running");
    private static readonly int Grounded = Animator.StringToHash("grounded");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");

    void FixedUpdate()
    {
        if (_rigidbody.bodyType == RigidbodyType2D.Static) return;

        grounded = GroundCheck();
        bool inWallslide = WallslideCheck() && !grounded;

        _animator.SetBool("wallslide", inWallslide);

        _wallSlideAudioSource.volume = inWallslide ? 0.5f : 0f;

        _wallJumpTimer -= Time.deltaTime;

        // Player movement
        Vector2 delta = Move(_moveInputDir, _lastDelta + applyForce);
        
        // If we are applying upward force, set directly to avoid gravity inconsisencies
        if (applyForce.y > 0)
        {
            bouncing = true;
            delta.y = applyForce.y;
        }
        applyForce = Vector3.zero;

        // Jump logic
        if (jumpHeld && !jumpedThisInput && (grounded || inWallslide))
        {
            // Apply vertical jump force
            delta.y = _jumpVel * Time.fixedDeltaTime;
            
            jumpedThisInput = true;

            // Apply lateral force for wall jump
            if (inWallslide)
            {
                _wallJumpTimer = _wallJumpRecoveryTime;
                ApplyForce(new Vector2((_spriteRenderer.flipX ? 1 : -1) * _wallJumpLateralForce, 0));
                AudioManager.Instance.PlaySfx("Walljump");
            }
            else
            {
                AudioManager.Instance.PlaySfx("Jump");
            }
        }
        else if (grounded && !(delta.y > 0))
        {
            delta.y = 0f;
            bouncing = false;
        }
        // Apply gravity
        delta.y -= _gravity;

        groundedLastTick = grounded;
        
        // Cap fall speed
        delta.y = Math.Max(delta.y, inWallslide ? _wallSlideSpeed : _maxFallSpeed);

        if (delta.y > 0 && !jumpHeld && !bouncing) delta.y *= _jumpControl;
        if (respawnedThisTick) delta = Vector3.zero;
        respawnedThisTick = false;

        _rigidbody.linearVelocity = delta;
        _lastDelta = delta;
        
        // Animation update
        if (_moveInputDir.x < 0)
            _spriteRenderer.flipX = true;
        else if (_moveInputDir.x > 0)
            _spriteRenderer.flipX = false;

        _animator.SetBool(Running, _moveInputDir.magnitude > 0);
        _animator.SetBool(Grounded, grounded);
        _animator.SetFloat(YVelocity, delta.y);
    }

    public void ApplyForce(Vector2 f)
    {
        applyForce += f;
    }

    /// <summary>
    /// Apply acceleration based on current velocity and desired velocity
    /// </summary>
    /// <param name="inputDir"></param>
    /// <param name="currentVel"></param>
    /// <param name="acceleration"></param>
    /// <param name="maxVel"></param>
    /// <returns></returns>
    private Vector2 AddAcceleration(Vector2 inputDir, Vector2 currentVel, float acceleration, float maxVel)
    {
        Vector2 v = new((currentVel + inputDir * acceleration).x, currentVel.y);
        // if (Vector2.Scale(v, lateralVector).magnitude > maxVel) v.x = maxVel * Mathf.Sign(v.x);

        return v;
    }

    /// <summary>
    /// Calculates current player state and returns an approprate movement vector based on state & input
    /// </summary>
    /// <param name="inputDir"></param>
    /// <param name="currentVel"></param>
    /// <returns></returns>
    private Vector2 Move(Vector2 inputDir, Vector2 currentVel)
    {
        // Determine current state movement
        bool useGroundPhys = groundedLastTick && grounded;

        // Run current state movement
        float airControl = 0.5f - Mathf.Max(_wallJumpTimer, 0) / _wallJumpRecoveryTime * 0.5f;
        return GroundMove(inputDir, currentVel, (inputDir.x * currentVel.x < 0 && !useGroundPhys) ? airControl : 1f);
    }

    private Vector2 GroundMove(Vector2 inputDir, Vector2 currentVel, float control)
    {
        // Apply friction
        Vector2 lateralVel = Vector2.Scale(currentVel, lateralVector);
        if (lateralVel.magnitude != 0)
        {
            float d = lateralVel.magnitude * _friction * Time.fixedDeltaTime;
            currentVel.x *= Mathf.Max(lateralVel.magnitude - d, 0) / lateralVel.magnitude;
        }

        return AddAcceleration(
            inputDir,
            currentVel,
            _groundAcceleration * control,
            _groundMaxVel
            );
    }

    public void OnMove(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        _moveInputDir.x = v.x;
    }

    public void OnJump(InputValue value)
    {
        jumpHeld = value.isPressed;
        jumpedThisInput = false;

    }

    public void OnRestart(InputValue value)
    {
        Respawn();
    }

    public void Respawn()
    {
        if (!_playerInput.enabled) return;
        // _rigidbody.enabled = false;
        GameManager.Instance.OnWarpCanceled(new());
        _playerInput.enabled = false;
        _lastDelta = Vector2.zero;
        _animator.SetBool("dead", true);
        AudioManager.Instance.PlaySfx("Hurt");
        StartCoroutine(RespawnHelper());
        // _rigidbody.enabled = true;
    }

    private IEnumerator RespawnHelper()
    {
        yield return new WaitForSeconds(0.25f);
        _spriteRenderer.enabled = false;
        _deathParticleSystem.Play();
        yield return new WaitForSeconds(0.25f);
        GameManager.Instance.OnWarpCanceled(new());
        transform.position = spawnPosition;
        _animator.SetBool("dead", false);
        respawnedThisTick = true;
        _spriteRenderer.enabled = true;
        _playerInput.enabled = true;
        AudioManager.Instance.PlaySfx("Step");
    }

    public void SetSpawn(Vector3 pos, Vector3 rot)
    {
        spawnPosition = pos;
    }

    public void OnPause(InputValue value)
    {

    }

    private bool GroundCheck()
    {
        float dist = _collider.size.y * 0.6f;
        Vector2 origin = transform.position;
        Vector2 offset = transform.right * _collider.size.x / 2f;
        return
            Physics2D.Raycast(origin, Vector2.down, dist, _groundMask)
            || Physics2D.Raycast(origin + offset, Vector2.down, dist, _groundMask)
            || Physics2D.Raycast(origin - offset, Vector2.down, dist, _groundMask);
    }

    private bool WallslideCheck()
    {
        float dist = _collider.size.x * 0.5f + 0.1f;
        return
            Physics2D.Raycast(transform.position, Vector2.right * (_spriteRenderer.flipX ? -1 : 1), dist, _groundMask);
    }

    private void Shoot()
    {

        // setting direction
        Vector3 shootDirection;
        shootDirection = Input.mousePosition;
        shootDirection.z = 0f;
        shootDirection = Camera.main.ScreenToWorldPoint(shootDirection);
        shootDirection -= transform.position;

        // spawn projectile
        Rigidbody2D projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);
        projectileInstance.linearVelocity = new Vector2(shootDirection.x, shootDirection.y).normalized * projectileSpeed;
        projectileInstance.gameObject.GetComponent<PlayerProjectile>().player = this;
        projectilesActive++;
        AudioManager.Instance.PlaySfx("Throw");


    }

}