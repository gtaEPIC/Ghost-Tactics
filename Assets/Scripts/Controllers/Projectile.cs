using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 0.1f;
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
    }
}
