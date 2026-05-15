using System.Collections;
using UnityEngine;

/// <summary>
/// Handles collectible cube behavior.
/// Detects player collection and plays collection animation.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CollectibleCube : MonoBehaviour
{
    /// <summary>
    /// Returns whether this cube has already been collected.
    /// </summary>
    public bool IsCollected => isCollected;

    [Header("Animation")]
    [SerializeField]
    private float collectAnimationDuration = 0.2f;

    private bool isCollected;

    private Collider cubeCollider;

    /// <summary>
    /// Initializes component references.
    /// </summary>
    private void Awake()
    {
        cubeCollider = GetComponent<Collider>();

        cubeCollider.isTrigger = true;
    }

    /// <summary>
    /// Detects player collection.
    /// </summary>
    /// <param name="other">The colliding object.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        isCollected = true;

        CollectionManager.Instance.OnCubeCollected(this);

        StartCoroutine(PlayCollectAnimation());
    }

    /// <summary>
    /// Plays a simple scale-down animation before destroying the cube.
    /// </summary>
    private IEnumerator PlayCollectAnimation()
    {
        Vector3 startScale = transform.localScale;

        Vector3 targetScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < collectAnimationDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / collectAnimationDuration;

            transform.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                t
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}