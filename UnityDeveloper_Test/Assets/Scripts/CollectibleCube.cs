using System.Collections;
using UnityEngine;

/// <summary>
/// Handles collectible cube behavior,
/// collection detection, and destruction animation.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CollectibleCube : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Minimum scale value used when shrinking the cube.
    /// </summary>
    private static readonly Vector3 MinScale = Vector3.zero;

    [Header("Collection Animation")]

    /// <summary>
    /// Duration of the scale-down collection animation.
    /// </summary>
    [SerializeField]
    private float _collectAnimationDuration = 0.2f;

    /// <summary>
    /// Cached collider reference.
    /// </summary>
    private Collider _cubeCollider;

    /// <summary>
    /// Tracks whether the cube has already been collected.
    /// Prevents duplicate collection events.
    /// </summary>
    private bool _isCollected;

    #endregion

    #region Properties

    /// <summary>
    /// Returns whether this cube has already been collected.
    /// </summary>
    public bool IsCollected => _isCollected;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes required component references.
    /// </summary>
    private void Awake()
    {
        _cubeCollider = GetComponent<Collider>();

        // Ensure the collider behaves as a trigger
        // for overlap-based collection detection.
        _cubeCollider.isTrigger = true;
    }

    /// <summary>
    /// Detects player collision and triggers collection.
    /// </summary>
    /// <param name="other">
    /// Collider that entered the trigger.
    /// </param>
    private void OnTriggerEnter(Collider other)
    {
        // Prevent duplicate collection processing.
        if (_isCollected)
        {
            return;
        }

        // Ignore collisions from non-player objects.
        if (!other.CompareTag("Player"))
        {
            return;
        }

        _isCollected = true;

        // Notify collection manager.
        if (CollectionManager.Instance != null)
        {
            CollectionManager.Instance.OnCubeCollected(this);
        }

        // Begin visual collection animation.
        StartCoroutine(PlayCollectAnimation());
    }

    #endregion

    #region Public Methods

    // Intentionally left empty.
    // Reserved for future external collectible interactions.

    #endregion

    #region Private Methods

    /// <summary>
    /// Plays a scale-down animation before destroying the cube.
    /// </summary>
    /// <returns>
    /// Coroutine enumerator.
    /// </returns>
    private IEnumerator PlayCollectAnimation()
    {
        Vector3 startScale = transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < _collectAnimationDuration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime =
                elapsedTime / _collectAnimationDuration;

            // Smoothly shrink the cube to zero scale.
            transform.localScale = Vector3.Lerp(
                startScale,
                MinScale,
                normalizedTime
            );

            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion
}