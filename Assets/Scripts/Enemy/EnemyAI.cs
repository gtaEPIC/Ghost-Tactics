using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float patrolAreaRadius = 5f; 
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange, sightAngle, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private bool respondingToAlarm = false; 

    private EnemyHealthController healthController;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        healthController = GetComponent<EnemyHealthController>();

        if (agent == null)
            Debug.LogWarning("NavMeshAgent component is missing on " + gameObject.name);
        if (healthController == null)
            Debug.LogWarning("EnemyHealthController component is missing on " + gameObject.name);
    }

    private void Update()
    {
        if (respondingToAlarm)
        {
            RespondToAlarm();
        }
        else
        {
            
            playerInSightRange = IsPlayerInSight();
            playerInAttackRange = playerInSightRange && Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange)
                Patroling();
            else if (playerInSightRange && !playerInAttackRange)
                ChasePlayer();
            else if (playerInAttackRange && playerInSightRange)
                AttackPlayer();
        }
    }

    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        
        if (distanceToPlayer > sightRange)
            return false;

        
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > sightAngle / 2)
            return false;

        
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightRange))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPointAroundAlarm();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false; 
    }

    private void SearchWalkPointAroundAlarm()
    {
        
        float randomZ = Random.Range(-patrolAreaRadius, patrolAreaRadius);
        float randomX = Random.Range(-patrolAreaRadius, patrolAreaRadius);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void RespondToAlarm()
    {
        
        agent.SetDestination(walkPoint);
        
        if (IsPlayerInSight())
        {
            respondingToAlarm = false; 
            return; 
        }

        Vector3 distanceToAlarmPoint = transform.position - walkPoint;

        
        if (distanceToAlarmPoint.magnitude < 1f)
        {
            respondingToAlarm = false; 
            walkPointSet = false; 
            SearchWalkPointAroundAlarm(); 
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        if (!alreadyAttacked)
        {
            GameObject instantiatedProjectile = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);
            Rigidbody rb = instantiatedProjectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = transform.forward * 32f;
            }
            else
            {
                Debug.LogWarning("Projectile does not have a Rigidbody component.");
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void SetPatrolPoint(Vector3 alarmPoint)
    {
        walkPoint = alarmPoint; 
        respondingToAlarm = true; 
    }

    public void TakeDamage(int damage)
    {
        if (healthController != null)
        {
            healthController.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Draw the sight angle visualization
        Gizmos.color = Color.blue;
        Vector3 leftBoundary = Quaternion.Euler(0, -sightAngle / 2, 0) * transform.forward * sightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, sightAngle / 2, 0) * transform.forward * sightRange;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
