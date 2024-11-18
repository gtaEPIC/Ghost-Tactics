using UnityEngine;

public class MineController : MonoBehaviour
{
    public float explosionRadius;
    public int damage;
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    
    private System.Collections.IEnumerator Explode()
    {
       
        
        // Get all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerHealthController>().TakeDamage(damage);
                
            }
            else if (collider.CompareTag("Enemy"))
            {
                collider.GetComponent<EnemyHealthController>().TakeDamage(damage);
            }
        }

        Destroy(gameObject); 
        yield return null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Explode());
        }
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(Explode());
        }
    }
}
