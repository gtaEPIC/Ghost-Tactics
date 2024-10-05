using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using GhostTacticsNS;

public class PlayerController : MonoBehaviour
{
    private GhostTactics _inputs;
    
    [Header("Character Controller")] [SerializeField]
    private CharacterController _controller;

    [Header("Movement")] private Vector2 _move;
    [SerializeField] private float _speed;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpHeight;
    [ShowOnly] [SerializeField] private Vector3 _velocity;

    
    [Header("Mouse Settings")]
    public float mouseSensX = 30f;
    public float mouseSensY = 30f;
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    private Vector2 _mouse;
    private Vector2 _mouseDelta;
    private Camera _camera;
    [SerializeField] private Vector2 smoothing = new Vector2(3, 3);

    [Header("Projectile Stuff")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private int maxAmmo = 7;
    [ShowOnly] [SerializeField] private int currentAmmo = 0;
    [SerializeField] private float projectileLifetime = 5f;
    [ShowOnly] [SerializeField] private bool canFire = true;
    [ShowOnly] [SerializeField] private bool isShooting = false;
    [ShowOnly] [SerializeField] private bool isReloading = false;


    
    [Header("Misc")] private int _currentLevel;

    [FormerlySerializedAs("_groundCheck")] [Header("Ground Detection")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundMask;
    [ShowOnly] [SerializeField] private bool isGrounded;


    

    private void Awake()
    {
        _inputs = new GhostTactics();
        _inputs.Enable();
        _inputs.Player.Move.performed += context => _move = context.ReadValue <Vector2>();
        _inputs.Player.Move.canceled += context => _move = Vector2.zero;
        _inputs.Player.Jump.performed += context => Jump();
        _inputs.Player.Look.performed += ctx => _mouse = ctx.ReadValue<Vector2>();
        _inputs.Player.Look.canceled += ctx => _mouse = Vector2.zero;
        _currentLevel = Convert.ToInt32(SceneManager.GetActiveScene().buildIndex);
        _camera = GetComponentInChildren<Camera>();
        _inputs.Player.Fire.performed += ctx => isShooting = true;
        _inputs.Player.Fire.canceled += ctx => isShooting = false;
        _inputs.Player.Reload.performed += ctx => Reload();
        currentAmmo = maxAmmo;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        if (isGrounded && _velocity.y < 0.0f)
        {
            _velocity.y = -2.0f;
        }

        Vector3 moveDirection = new Vector3(_move.x, 0.0f, _move.y);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= _speed * Time.deltaTime;
        
        _velocity.y += -_gravity * Time.fixedDeltaTime;
        
        _controller.Move(moveDirection);
        _controller.Move(_velocity * Time.fixedDeltaTime);

    }
        
    void Update()
{
    //Set mouse movement to a vector2 and apply sensitivity
    _mouseDelta = _mouse * (mouseSensY * Time.deltaTime);
    //Apply X mouse sensitivty seperately
    _mouseDelta.x = _mouse.x * (mouseSensX * Time.deltaTime);
    
    //Up and down mouse input handling
    _yRotation -= _mouseDelta.y;
    _yRotation = Mathf.Clamp(_yRotation, -90f, 90f);
    _xRotation += _mouseDelta.x;

    //Apply X rotation to PlayerObject, and Y rotation to camera
    _controller.transform.rotation = Quaternion.Euler(0, _xRotation, 0);
    _camera.transform.localRotation = Quaternion.Euler(_yRotation, 0, 0);

    if (isShooting)
    {
        FireProjectile();
    }
}

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    private void SendMessage(InputAction action,InputAction.CallbackContext context)
    {
        Debug.Log($"Move Performed x = {context.ReadValue<Vector2>().x}, y = {context.ReadValue<Vector2>().y}");
    }

    public void Jump()
    {
        if (isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2.0f * -_gravity);
        }
    }

    public void DisableInput()
    {
        //Disable all input
        _inputs.Disable();
    }

    public void EnableInput()
    {
        //Enable all input
        _inputs.Enable();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            //if the application is focused, lock the mouse and enable input
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            EnableInput();
        }
        else
        {
            //If the application is not focused, unlock the mouse and disable input
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            DisableInput();
        }
    }

    void OnTriggerEnter(Collider Other)
    {
        Debug.Log($"Triggering with {Other.gameObject.tag}");
        if (Other.CompareTag("death"))
        {
            //Reload the entire scene for now
            SceneManager.LoadScene(_currentLevel);
        }
    }

    private void FireProjectile()
{
    if (canFire && currentAmmo > 0)
    {
        canFire = false;
        StartCoroutine(FireProjectileCoroutine());
        currentAmmo--;
    }
}

private IEnumerator FireProjectileCoroutine()
{
    Quaternion projectileRotation = Quaternion.Euler(0f, 0f, 90f);
    GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
    Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
    projectileRigidbody.linearVelocity = projectileSpawnPoint.forward * projectileSpeed;
    DestroyProjectile(projectile);
    yield return new WaitForSeconds(fireRate);
    canFire = true;
}


    private void Reload()
{
    if (!isReloading)
    {
        isReloading = true;
        StartCoroutine(ReloadCoroutine());
    }
}

    private IEnumerator ReloadCoroutine()
{
    yield return new WaitForSeconds(reloadTime);
    currentAmmo = maxAmmo;
    isReloading = false;
}

private void DestroyProjectile(GameObject projectile)
{
    StartCoroutine(DestroyProjectileCoroutine(projectile));
}

private IEnumerator DestroyProjectileCoroutine(GameObject projectile)
{
    yield return new WaitForSeconds(projectileLifetime);
    Destroy(projectile);
}


}
