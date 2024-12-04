using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI2 : MonoBehaviour
{
    [Header("General Variables")]
    private StateMachine _state;
    private NavMeshAgent _agent;
    private Transform _player;
    private Sight _sight;
    private float _preAttackStoppingDistance;
    
    // Anti-Stuck Variables
    private Vector3 _myPosition;
    private float _stuckTimer;
    private float _stuckTime = 5f;
    
    // Distance to a goal before switching goals
    [SerializeField] private float closeEnough = 1f;
    
    // Patrolling variables
    [Header("Patrolling Variables")]
    [ShowOnly, SerializeField] private Vector3 walkPoint;
    [SerializeField] private float walkPointRange = 10f;
    
    // Attacking variables
    [Header("Attacking Variables")]
    [SerializeField] private float attackCooldown = 2f;
    [ShowOnly, SerializeField] private float currentCooldown;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackDistance = 5f;
    public GameObject projectile;
    
    // Alarm variables
    [Header("Alarm Variables")]
    [ShowOnly, SerializeField] private bool alarmSounded;
    [ShowOnly, SerializeField] private Vector3 alarmPosition;
    
    // Idle variables
    [Header("Idle Variables")]
    [SerializeField] private float idleTime = 2f;
    [ShowOnly, SerializeField] private float currentIdleTime;
    
    // Epic Debug Variables
    [Header("Debug Variables")] 
    [ShowOnly, SerializeField] private string _currentState;
    
    // State Machine States
    private StateMachine.State _patrol;
    private StateMachine.State _attack;
    private StateMachine.State _chase;
    private StateMachine.State _investigate;
    private StateMachine.State _idle;
    

    // Epic Cool Functions

    private float DistanceToGoal(Vector3 goal)
    {
        return Vector3.Distance(transform.position, goal);
    }
    
    private bool AtGoal(Vector3 goal)
    {
        return DistanceToGoal(goal) < closeEnough;
    }
    
    private Vector3 RandomPoint(Vector3 origin, float range)
    {
        Vector3 randomPoint = origin + UnityEngine.Random.insideUnitSphere * range;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, range, 1);
        Debug.Log("New walk point: " + hit.position);
        return hit.position;
    }
    
    private bool IsPlayerInSight()
    {
        return _sight.detected != null;
    }
    
    private bool IsPlayerInAttackRange()
    {
        return DistanceToGoal(_player.position) < attackRange;
    }

    private void CheckStuck()
    {
        if (_myPosition == transform.position)
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= _stuckTime)
            {
                // Pick a new walk point
                Debug.Log("I got stuck! Picking a new walk point.");
                walkPoint = RandomPoint(transform.position, walkPointRange);
                _stuckTimer = 0;
            }
        }
        else
        {
            _myPosition = transform.position;
            _stuckTimer = 0;
        }
    }
    
    
    // State On Frames

    private void Patrol()
    {
        CheckStuck();
        if (AtGoal(walkPoint))
        {
            // Idle for a bit
            _state.TransitionTo(_idle);
        }
        else
        {
            // Move to the walk point
            _agent.SetDestination(walkPoint);
        }
    }

    private void StartAttack()
    {
        _preAttackStoppingDistance = _agent.stoppingDistance;
        _agent.stoppingDistance = attackDistance;
    }
    
    private void Attack()
    {
        alarmSounded = false; // If the player is in sight, the alarm is no longer relevant
        _agent.SetDestination(_player.position);
        
        // Make sure the enemy is facing the player
        Vector3 directionToPlayer = (_player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
        
        // Attempt to shoot the player
        if (currentCooldown <= 0)
        {
            // Shoot the player
            currentCooldown = attackCooldown;
            
            GameObject bullet = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Projectile does not have a Rigidbody component.");
            }
        }
        
        if (!IsPlayerInSight())
        {
            // Stop chasing
            _state.TransitionTo(_patrol);
        }
    }
    
    private void EndAttack()
    {
        _agent.stoppingDistance = _preAttackStoppingDistance;
    }
    
    private void Chase()
    {
        alarmSounded = false; // If the player is in sight, the alarm is no longer relevant
        _agent.SetDestination(_player.position);
        
        if (!IsPlayerInSight())
        {
            // Stop chasing
            _state.TransitionTo(_patrol);
        }
    }
    
    private void Investigate()
    {
        _agent.SetDestination(alarmPosition);

        if (AtGoal(alarmPosition))
        {
            // Stop investigating
            _state.TransitionTo(_patrol);
        }
    }

    private void StartIdle()
    {
        currentIdleTime = idleTime;
    }

    private void Idle()
    {
        if (currentIdleTime <= 0)
        {
            // Stop idling
            _state.TransitionTo(_patrol);
        }
        else
        {
            currentIdleTime -= Time.deltaTime;
        }
    }

    private void EndIdle()
    {
        // Set a new walk point
        walkPoint = RandomPoint(transform.position, walkPointRange);
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        _sight = GetComponent<Sight>();
        
        // Pick a random walk point
        walkPoint = RandomPoint(transform.position, walkPointRange);
        
        // State Machine (my beloved)
        _state = new StateMachine();
        
        // States
        
        // Patrol
        _patrol = _state.CreateState("Patrol");
        // Attack
        _attack = _state.CreateState("Attack");
        // Chase
        _chase = _state.CreateState("Chase");
        // Investigate (Responding to an alarm)
        _investigate = _state.CreateState("Investigate");
        // Idle
        _idle = _state.CreateState("Idle");
        
        // OnFrames
        _patrol.OnFrame = Patrol;
        _attack.OnFrame = Attack;
        _chase.OnFrame = Chase;
        _investigate.OnFrame = Investigate;
        _idle.OnFrame = Idle;
        
        // OnEnter
        _attack.OnEnter = StartAttack;
        _idle.OnEnter = StartIdle;
        
        // OnExit
        _attack.OnExit = EndAttack;
        _idle.OnExit = EndIdle;
    }

    private void Update()
    {
        _currentState = _state?.currentState?.ToString();
        currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0) currentCooldown = 0;
        
        // If the player is in sight, in any state, chase the player
        if (IsPlayerInSight() && IsPlayerInAttackRange())
        {
            _state.TransitionTo(_attack);
        }
        else if (IsPlayerInSight())
        {
            _state.TransitionTo(_chase);
        }
        
        _state.Update();
    }
    
    private void OnDrawGizmos()
    {
        // Draw the walk point
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(walkPoint, 1f);
        
        // Draw the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
    }
}
