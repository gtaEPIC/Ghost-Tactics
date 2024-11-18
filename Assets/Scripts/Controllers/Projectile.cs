using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 0.25f;
    [SerializeField] private int _damage = 5;
    private void Start()
    {
        Destroy(gameObject, _destroyDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Target target = collision.gameObject.GetComponent<Target>();
        if (target != null)
        {
            target.DestroyTarget();
        }
        Destroy(gameObject);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealthController healthController = collision.gameObject.GetComponent<EnemyHealthController>();
            if (healthController != null)
            {
                healthController.TakeDamage(_damage);
            }
            Destroy(this.gameObject);
        }
    }
}
