using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 0.25f;
    [SerializeField] private int _damage = 5;
    [SerializeField] private Vector3 _force = new Vector3(0, 0, 10);

    private void Start()
    {
        Destroy(gameObject, _destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        Target target = other.gameObject.GetComponent<Target>();
        if (target != null)
        {
            target.DestroyTarget();
        }

        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(_force, ForceMode.Impulse);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHealthController healthController = other.gameObject.GetComponent<EnemyHealthController>();
            if (healthController != null)
            {
                healthController.TakeDamage(_damage);
            }
        }

        Destroy(gameObject);
    }
}