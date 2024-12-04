using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Sight: Sense
{
    [FormerlySerializedAs("FieldOfView")] public float fieldOfView = 45;
    [FormerlySerializedAs("ViewDistance")] public float viewDistance = 100;
    private Transform playerTransform;
    private Vector3 rayDirection;

    [ReadOnly] public GameObject detected { get; private set; }
        
    protected override void Initialize()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
        
    protected override void UpdateSense()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= detectionRate)
        {
            DetectAspect();
            elapsedTime = 0.0f;
        }
    }
        
    public void DetectAspect()
    {
        RaycastHit hit;
        rayDirection = playerTransform.position - transform.position;
        if (Vector3.Angle(rayDirection, transform.forward) < fieldOfView)
        {
            if (Physics.Raycast(transform.position, rayDirection, out hit, viewDistance))
            {
                // Aspect aspect = hit.collider.GetComponent<Aspect>();
                string tag = hit.collider.tag;
                if (targetTag == tag)
                {
                    detected = hit.collider.gameObject;
                    return;
                }
            }
        }
        detected = null;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isEditor || playerTransform == null)
            return;
            
        // Draw view distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        
        // Draw field of view
        Gizmos.color = Color.blue;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * viewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * viewDistance;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
        
        // Draw line to player
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, playerTransform.position);
    }
}