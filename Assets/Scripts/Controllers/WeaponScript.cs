using UnityEngine;
using GhostTacticsNS;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] private string _playerControllerName = "Player";
    [Header("Projectile Settings")]
    [SerializeField] private float _projectileSpeed = 10.0f;
    [SerializeField] private float _projectileLifeTime = 5.0f;
    [SerializeField] private float _projectileFireRate = 0.5f;
    private PlayerController2 _playerController;
    private bool _canShoot = true;
    private float _fireTimer = 0.0f;
    private float _fireTimerMax = 0.0f;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _projectileSpawnPoint;
    private bool _isShooting = false;
    
    // Sound Effects
    [Header("Sound Effects")]
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private AudioClip _reloadSound; // Not implemented yet
    private AudioSource _audioSource;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameObject playerObject = GameObject.Find(_playerControllerName);
        _audioSource = GetComponent<AudioSource>();
        if (playerObject != null)
        {
            _playerController = playerObject.GetComponent<PlayerController2>();
            if (_playerController == null)
            {
                Debug.LogError("PlayerController component not found on player object");
            }
        }
        else
        {
            Debug.LogError("Player object not found in scene");
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_playerController.isFiring)
        {
            ItsFridayInCalifornia();
        }
    }

    private void ItsFridayInCalifornia()
    {
        if (_canShoot)
        {
            _canShoot = false;
            _fireTimer = 0.0f;
            _fireTimerMax = _projectileFireRate;

            GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.transform.position, _projectileSpawnPoint.transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.linearVelocity = _projectileSpawnPoint.transform.forward * _projectileSpeed;
            }
            Destroy(projectile, _projectileLifeTime);
            
            // Play sound effect
            if (_fireSound != null)
            {
                _audioSource.PlayOneShot(_fireSound);
            }
        }
        else
        {
            _fireTimer += Time.deltaTime;
            if (_fireTimer >= _fireTimerMax)
            {
                _canShoot = true;
            }
        }

        if (_isShooting)
        {
            _fireTimer = 0.0f;
            GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.transform.position, _projectileSpawnPoint.transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.linearVelocity = _projectileSpawnPoint.transform.forward * _projectileSpeed;
            }
            Destroy(projectile, _projectileLifeTime);
        }
    }
    
    
    
}
