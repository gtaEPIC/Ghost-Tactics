using UnityEngine;


public class Sense : MonoBehaviour
{
    public bool bDebug;

    public string targetTag;
    public float detectionRate = 1.0f;
    
    protected float elapsedTime = 0.0f;
    protected virtual void Initialize() { }
    protected virtual void UpdateSense() { }
    
    private void Start()
    {
        Initialize();
    }
    
    private void Update()
    {
        UpdateSense();
    }
}
