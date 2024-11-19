using UnityEngine;

public class Alarm : MonoBehaviour
{
    public float alertRadius = 20f; 
    public LayerMask enemyLayer; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AlertNearbyEnemies();
        }
    }

    private void AlertNearbyEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, alertRadius, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.SetPatrolPoint(transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}