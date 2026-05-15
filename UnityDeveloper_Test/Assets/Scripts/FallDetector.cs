using UnityEngine;

/// <summary>
/// Detects dangerous free-fall situations
/// and triggers a game over when the player
/// falls for too long and too far.
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class FallDetector : MonoBehaviour
{
    #region Fields

    [Header("Fall Detection")]

    /// <summary>
    /// Maximum amount of continuous free-fall time
    /// allowed before triggering a fail condition.
    /// </summary>
    [SerializeField]
    private float _maxFallDuration = 3f;

    /// <summary>
    /// Minimum distance the player must fall
    /// before a game over can occur.
    /// </summary>
    [SerializeField]
    private float _minimumFallDistance = 15f;

    /// <summary>
    /// Cached reference to the PlayerController component.
    /// </summary>
    private PlayerController _playerController;

    /// <summary>
    /// Tracks how long the player has been falling continuously.
    /// </summary>
    private float _fallTimer;

    /// <summary>
    /// Stores the player's last grounded position.
    /// Used to measure total fall distance.
    /// </summary>
    private Vector3 _lastGroundedPosition;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes required component references.
    /// </summary>
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    /// <summary>
    /// Stores the player's initial grounded position.
    /// </summary>
    private void Start()
    {
        _lastGroundedPosition = transform.position;
    }

    /// <summary>
    /// Monitors player free-fall state each frame.
    /// </summary>
    private void Update()
    {
        // Stop processing if the game is already over.
        if (
            GameManager.Instance != null &&
            GameManager.Instance.IsGameOver
        )
        {
            return;
        }

        HandleFallDetection();
    }

    #endregion

    #region Public Methods

    // Intentionally left empty.
    // Reserved for future public fall-detection APIs.

    #endregion

    #region Private Methods

    /// <summary>
    /// Tracks player grounded state,
    /// free-fall duration, and fall distance.
    /// </summary>
    private void HandleFallDetection()
    {
        // Reset fall tracking when the player touches ground.
        if (_playerController.IsGrounded)
        {
            _fallTimer = 0f;

            _lastGroundedPosition = transform.position;

            return;
        }

        // Increase continuous free-fall timer.
        _fallTimer += Time.deltaTime;

        // Measure total distance from the last grounded point.
        float fallDistance =
            Vector3.Distance(
                transform.position,
                _lastGroundedPosition
            );

        // Trigger fail condition only when both:
        // 1. Fall duration exceeds threshold
        // 2. Fall distance exceeds threshold
        if (
            _fallTimer >= _maxFallDuration &&
            fallDistance >= _minimumFallDistance
        )
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver(
                    GameOverReason.Fell
                );
            }
        }
    }

    #endregion
}