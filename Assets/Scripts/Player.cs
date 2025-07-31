using System;
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

    void Start()
    {
        GetComponent<PlayerInput>().enabled = true;
        spawnPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && shootingUnlocked)
        {
            Shoot();
        }
    }
    
    private static readonly int Running = Animator.StringToHash("running");
    private static readonly int Grounded = Animator.StringToHash("grounded");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");

    void FixedUpdate()
    {
        grounded = GroundCheck();
        bool inWallslide = WallslideCheck() && !grounded;

        _wallJumpTimer -= Time.deltaTime;

        // Player movement
        Vector2 delta = Move(_moveInputDir, _lastDelta + applyForce);
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
                ApplyForce(new Vector2(-_moveInputDir.x * _wallJumpLateralForce, 0));
            }
        }
        else if (grounded)
        {
            delta.y = 0f;
        }

        delta.y -= _gravity;

        // Apply gravity
        groundedLastTick = grounded;

        // Cap fall speed
        delta.y = Math.Max(delta.y, inWallslide ? _wallSlideSpeed : _maxFallSpeed);

        if (delta.y > 0 && !jumpHeld) delta.y *= _jumpControl;

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
        return GroundMove(inputDir, currentVel, (inputDir.x * currentVel.x < 0 && !useGroundPhys) ? 0.25f : 1f);
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
        // _rigidbody.enabled = false;
        transform.position = spawnPosition;
        respawnedThisTick = true;

        // _rigidbody.enabled = true;
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
        float dist = _collider.size.y * 0.56f;
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
            Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(_moveInputDir.x), dist, _groundMask);
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


    }




}