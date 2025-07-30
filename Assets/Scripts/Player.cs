using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player2D : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] BoxCollider2D _collider;

    private bool jumpHeld = false;
    private bool jumpedThisInput = false;
    private bool groundedLastTick = false;
    private bool grounded = false;

    [SerializeField] private float _groundAcceleration;
    [SerializeField] private float _airAcceleration;
    [SerializeField] private float _groundMaxVel;
    [SerializeField] private float _airMaxVel;
    [SerializeField] private float _friction;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpVel;
    [SerializeField] private float _airControl;
    [SerializeField] private float _jumpControl;

    [SerializeField] private LayerMask _groundMask;

    private Vector3 _moveInputDir = Vector3.zero;
    private Vector2 lateralVector = new(1, 0);

    // Extra force caused by other objects, to be applied next tick
    private Vector2 applyForce = Vector3.zero;
    
    private Vector2 spawnPosition;

    private bool respawnedThisTick;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        // debugText = GameObject.Find("DebugText").GetComponent<TextMeshProUGUI>();
        GetComponent<PlayerInput>().enabled = true;

        // TODO: Set up camera follower
        // Camera.main.GetComponent<FollowTransform>().SetToFollow(transform);
        // Camera.main.GetComponent<FollowTransform>().enabled = true;
        spawnPosition = transform.position;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        grounded = GroundCheck();

        // Player movement
        Vector2 delta = Move(_moveInputDir, _rigidbody.linearVelocity + applyForce);
        applyForce = Vector3.zero;

        // Jump logic
        if (jumpHeld && !jumpedThisInput && grounded)
        {
            // Apply vertical jump force
            delta.y = _jumpVel * Time.fixedDeltaTime;

            jumpedThisInput = true;
            Debug.Log("Jump");
        }

        delta.y -= _gravity;

        // Apply gravity
        groundedLastTick = grounded;

        // Cap fall speed
        delta.y = Math.Max(delta.y, _maxFallSpeed);

        if (delta.y > 0 && !jumpHeld) delta.y *= _jumpControl;

        if (respawnedThisTick) delta = Vector3.zero;
        respawnedThisTick = false;

        _rigidbody.linearVelocity = delta;
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
    private Vector3 AddAcceleration(Vector3 inputDir, Vector3 currentVel, float acceleration, float maxVel)
    {
        float projectedVel = Vector2.Dot(currentVel * Time.fixedDeltaTime, inputDir);
        float accelVel = acceleration * Time.fixedDeltaTime;
        maxVel *= Time.fixedDeltaTime;

        // Cap max accel
        if (projectedVel + accelVel > maxVel)
        {
            accelVel = maxVel - projectedVel;
        }
        
        return new Vector2((currentVel * Time.fixedDeltaTime + inputDir * accelVel).x, currentVel.y);
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
        if (useGroundPhys)
        {
            return GroundMove(inputDir, currentVel);
        }
        else
        {
            return AirMove(inputDir, currentVel);
        }

    }

    private Vector2 GroundMove(Vector2 inputDir, Vector2 currentVel)
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
            _groundAcceleration,
            _groundMaxVel
            );
    }

    private Vector2 AirMove(Vector2 inputDir, Vector2 currentVel)
    {
        // Air control
        // TODO: reduce air control after walljump
        float control = _airControl;
        currentVel.x *= control;

        return AddAcceleration(
            inputDir,
            currentVel,
            _airAcceleration,
            _airMaxVel
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
        Vector3 origin = transform.position;
        Vector3 offset = transform.forward * _collider.size.x / 2f;
        return 
            Physics2D.Raycast(origin, Vector3.down, dist, _groundMask) 
            || Physics.Raycast(origin + offset, Vector3.down, dist, _groundMask)
            || Physics.Raycast(origin - offset, Vector3.down, dist, _groundMask);
    }
}