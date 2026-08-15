using UnityEngine;

public class MissTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<CubeHandling>(out var cubeHandling) || !cubeHandling.enabled)
        {
            return;
        }

        ScoreManager.Instance?.RegisterMiss();
        Destroy(other.gameObject);
    }
}