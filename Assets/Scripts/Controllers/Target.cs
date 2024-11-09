using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 2.0f;
    [SerializeField] private float _toggleInterval = 0.1f;
    [SerializeField] private int _scoreValue = 10;
    private MeshRenderer _meshRenderer;
    private bool _isToggling = false;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void DestroyTarget()
    {
        if (!_isToggling)
        {
            _isToggling = true;
            StartCoroutine(ToggleMeshRenderer());
        }
    }

    private IEnumerator ToggleMeshRenderer()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < _destroyDelay)
        {
            _meshRenderer.enabled = !_meshRenderer.enabled;
            yield return new WaitForSeconds(_toggleInterval);
            elapsedTime += _toggleInterval;
        }

        GameManager.Instance.AddScore(_scoreValue);
        Destroy(gameObject);
    }
}