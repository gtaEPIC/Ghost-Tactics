using System;
using UnityEngine;
using GhostTacticsNS;

public class PlayerController2 : MonoBehaviour
{
    [Header("Character Controller")]
    private CharacterController _controller;

    public GhostTactics _inputs;

    [Header("Movement")] private Vector2 _move;
    [SerializeField] private float _speed = 8.0f;
    private float _speedOld;
    [SerializeField] private float _acceleration = 1.0f;
    [SerializeField] private float _deceleration = 1.0f;
    //[SerializeField] private float _strafeModifier = 0.5f;
    [ShowOnly] [SerializeField] private float speedmodifier = 0;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _jumpHeight = 6.0f;
    [ShowOnly] [SerializeField] private Vector3 _velocity = new Vector3(0, 0, 0);
    

    [Header("Mouse Settings")]
    private Vector2 _mouse;
    private Camera _camera;
    [SerializeField] public float mouseSensX = 30f;
    [SerializeField] public float mouseSensY = 30f;
    private float _xRotation = 0f;
    private float _yRotation = 0f; 
    [SerializeField] public float _cameraVelMod = 20.0f;
    [SerializeField] public float _cameraReturnSpeed = 0.001f;
    [SerializeField]public int _cameraSnapCapX = 50;
    [SerializeField] public int _cameraSnapCapY = 50;
    private Vector2 _cameraDesync = new Vector2(0f, 0f);    
    private Vector2 _cameraOffset = new Vector2(0f, 0f);
    private Vector2 _cameraPrev = new Vector2(0f, 0f);
    private float _yGunRotation = 0.0f;
    
    [Header("Lean Settings")]
    private Vector2 _lean;
    [SerializeField] private float _leanAngle = 15f;
    [SerializeField] private float _leanDistance = 0.5f;
    [SerializeField] private float _leanSpeed = 5f;
    private Vector3 _targetCameraPosition;
    private Quaternion _targetCameraRotation;
    private Vector3 _targetGunPosition;
    [SerializeField] private GameObject _currentGun = null;
    

    //[Header("Projectile Stuff")]
    public bool isFiring = false;

    private void Awake()
    {
        _inputs = new GhostTactics();
        _inputs.Enable();
        
        //Movement Input Binds
        _inputs.Player.Move.performed += ctx => _move = ctx.ReadValue<Vector2>();
        _inputs.Player.Move.canceled += ctx => _move = Vector2.zero;
        _inputs.Player.Jump.performed += context => Jump();
        
        //Mouse Movement Input Binds
        _inputs.Player.Look.performed += ctx => _mouse = ctx.ReadValue<Vector2>();
        _inputs.Player.Look.canceled += ctx => _mouse = Vector2.zero;
        
        // Lean Input Binds
        _inputs.Player.LeftLean.performed += ctx => _lean.x = -1;
        _inputs.Player.LeftLean.canceled += ctx => _lean.x = 0;
        _inputs.Player.RightLean.performed += ctx => _lean.x = 1;
        _inputs.Player.RightLean.canceled += ctx => _lean.x = 0;
        
        //Variable Binding
        _camera = GetComponentInChildren<Camera>();
        _controller = GetComponent<CharacterController>();
        
        //Lean Bindings
        _targetCameraPosition = _camera.transform.localPosition;
        _targetCameraRotation = _camera.transform.localRotation;
        _targetGunPosition = _currentGun.transform.localPosition;
        
        //Weapon Bindings
        _inputs.Player.Fire.performed += ctx => isFiring = true;
        _inputs.Player.Fire.canceled += ctx => isFiring = false;
    }

    void Update()
    {
        HandleMouseInput();
        MovementHandler();
        HandleLeanInput();
    }
    
    private void FixedUpdate()
    {
        PhysicsHandler();
    }


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _speedOld = _speed;
    }

    //Mouse input handler
    private void HandleMouseInput()
    {
        Vector2 _mouseDelta = _mouse * Time.deltaTime;
        _xRotation -= _mouseDelta.y * mouseSensY;
        _yRotation += _mouseDelta.x * mouseSensX;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        // if (Mathf.Abs(_move.x) > 0.1f || Mathf.Abs(_velocity.y) > 0.1f)
        // {
        //     _cameraOffset.x = Mathf.MoveTowards(_cameraOffset.x, _cameraSnapCapX * _move.x, _cameraVelMod * Time.deltaTime);
        //     _cameraOffset.y = Mathf.MoveTowards(_cameraOffset.y, _velocity.y, _cameraVelMod * Time.deltaTime);
        // }
        // else
        // {
        //     _cameraOffset.x = Mathf.MoveTowards(_cameraOffset.x, 0, _cameraReturnSpeed * Time.deltaTime);
        //     _cameraOffset.y = Mathf.MoveTowards(_cameraOffset.y, 0, _cameraReturnSpeed * Time.deltaTime);
        // }
        _cameraOffset.x = Mathf.Clamp(_cameraOffset.x, -_cameraSnapCapX * 2, _cameraSnapCapX * 2);
        _cameraOffset.y = Mathf.Clamp(_cameraOffset.y, -_cameraSnapCapY, _cameraSnapCapY * 2);

        _yGunRotation = Mathf.Clamp(_xRotation, -20f, 40f);
    
        Quaternion mouseRotation = Quaternion.Euler(_xRotation + _cameraOffset.y, _cameraOffset.x, 0f);
        _camera.transform.localRotation = Quaternion.Slerp(_camera.transform.localRotation, mouseRotation, _leanSpeed * Time.deltaTime);
        _controller.transform.rotation = Quaternion.Euler(0f, _yRotation - _cameraOffset.x, 0f);
        _currentGun.transform.rotation = Quaternion.Euler(_yGunRotation, _yRotation, 0f);
    }

    //Character movement handler
    private void MovementHandler()
    {
            Vector3 move = transform.right * _move.x + transform.forward  * _move.y;
            if (Math.Abs(move.x) > 0.1f || Math.Abs(move.z) > 0.1f)
            {
                if (speedmodifier == 0.0f) { speedmodifier = 2.0f;}
                speedmodifier = speedmodifier + _acceleration * 0.01f;
                speedmodifier = Mathf.Clamp(speedmodifier, 0, _speed);
            }
            else if (_controller.isGrounded)
            {
                speedmodifier = 0;
                _velocity.x -= Mathf.MoveTowards(_velocity.x, 0, _deceleration * Time.deltaTime);
                _velocity.z -= Mathf.MoveTowards(_velocity.z, 0, _deceleration * Time.deltaTime);
            }
            _controller.Move(move * speedmodifier * Time.deltaTime);
    }
    
    private void HandleLeanInput()
    {
        float leanAngle = -_lean.x * _leanAngle;
        float leanDistance = _lean.x * _leanDistance;
        
        _targetCameraRotation = Quaternion.Euler(_xRotation + _cameraOffset.y, _cameraOffset.x, leanAngle);
        _targetCameraPosition = new Vector3(leanDistance, _camera.transform.localPosition.y, _camera.transform.localPosition.z);
        _targetGunPosition = new Vector3(leanDistance, _currentGun.transform.localPosition.y, _currentGun.transform.localPosition.z);
        
        _camera.transform.localRotation = Quaternion.Slerp(_camera.transform.localRotation, _targetCameraRotation, _leanSpeed * Time.deltaTime);
        _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, _targetCameraPosition, _leanSpeed * Time.deltaTime);
        _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _targetGunPosition, _leanSpeed * Time.deltaTime);
    }
    
    //Character physics handler
    private void PhysicsHandler()
    {
        if (!_controller.isGrounded)
        {
            _velocity.y += -_gravity * Time.smoothDeltaTime;
        }
        
        // Ensure _velocity does not contain NaN values
        if (float.IsNaN(_velocity.x) || float.IsNaN(_velocity.y) || float.IsNaN(_velocity.z))
        {
            Debug.LogError("Velocity contains NaN values: " + _velocity);
            _velocity = Vector3.zero;
        }

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        _velocity.y = (_jumpHeight);
    }
    
    
    //Mouse state handlers
    private void DisableInput()
    {
        //Disable all input
        _inputs.Disable();
    }

    private void EnableInput()
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
    
    //Reserved Area for external interactions
    
    public void ReduceSpeedByHalf()
    {
        _speed *= 0.5f;
    }
    
    public void ResetSpeed()
    {
        {
            _speed = _speedOld;
        }
    }
    
}
