using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float destroyDelay = 1.5f; 

    private void Start()
    {
        
        StartCoroutine(DestroyAfterDelay());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthController healthController = other.gameObject.GetComponent<PlayerHealthController>();
            if (healthController != null)
            {
                healthController.TakeDamage(damage);
            }
            Destroy(this.gameObject);
        }
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(this.gameObject);
    }
}