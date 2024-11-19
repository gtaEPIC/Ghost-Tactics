using UnityEngine;

public class MineController : MonoBehaviour
{
    public float explosionRadius;
    public int damage;
    public AudioClip explosionSound;
    private AudioSource audioSource;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private System.Collections.IEnumerator Explode()
    {
        // Play the explosion sound
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        //Disable the mesh renderer
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

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

        //Wait for the sound clip to finish playing
        yield return new WaitForSeconds(explosionSound.length);
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            StartCoroutine(Explode());
        }
    }
}