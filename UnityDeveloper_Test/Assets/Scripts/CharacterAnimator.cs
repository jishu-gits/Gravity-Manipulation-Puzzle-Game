using UnityEngine;

/// <summary>
/// Controls character animation states using Animator parameters.
/// Synchronizes movement, grounded state, and jump triggers
/// with the Animator Controller.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class CharacterAnimator : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Minimum movement magnitude required
    /// before the character is considered moving.
    /// </summary>
    private const float MovementThreshold = 0.01f;

    [Header("References")]

    /// <summary>
    /// Animator component responsible for playing animations.
    /// </summary>
    [SerializeField]
    private Animator _animator;

    /// <summary>
    /// Reference to the PlayerController component.
    /// Used for grounded-state queries.
    /// </summary>
    [SerializeField]
    private PlayerController _playerController;

    [Header("Animation Settings")]

    /// <summary>
    /// Speed used to smooth animation blending.
    /// Higher values produce faster transitions.
    /// </summary>
    [SerializeField]
    private float _animationSmoothness = 10f;

    /// <summary>
    /// Current smoothed movement speed value.
    /// </summary>
    private float _currentSpeed;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes required component references.
    /// </summary>
    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        if (_playerController == null)
        {
            _playerController = GetComponent<PlayerController>();
        }
    }

    /// <summary>
    /// Updates animation parameters every frame.
    /// </summary>
    private void Update()
    {
        UpdateMovementAnimation();
        UpdateGroundedAnimation();
        HandleJumpAnimation();
    }

    #endregion

    #region Public Methods

    // Intentionally left empty.
    // Reserved for future public animation API methods.

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates movement animation blending using
    /// WASD keyboard input magnitude.
    /// </summary>
    private void UpdateMovementAnimation()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Read horizontal movement input.
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }

        // Read vertical movement input.
        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }

        // Convert raw keyboard input into a normalized
        // movement magnitude used for animation blending.
        Vector2 movementInput =
            new Vector2(horizontalInput, verticalInput);

        float targetSpeed = movementInput.magnitude;

        // Prevent tiny floating-point fluctuations from
        // triggering movement transitions unintentionally.
        if (targetSpeed < MovementThreshold)
        {
            targetSpeed = 0f;
        }

        // Smoothly interpolate animation speed
        // for cleaner transitions between states.
        _currentSpeed = Mathf.Lerp(
            _currentSpeed,
            targetSpeed,
            _animationSmoothness * Time.deltaTime
        );

        _animator.SetFloat("Speed", _currentSpeed);
    }

    /// <summary>
    /// Updates grounded-state animation parameter.
    /// </summary>
    private void UpdateGroundedAnimation()
    {
        _animator.SetBool(
            "IsGrounded",
            _playerController.IsGrounded
        );
    }

    /// <summary>
    /// Triggers jump animation when the player
    /// presses Space while grounded.
    /// </summary>
    private void HandleJumpAnimation()
    {
        if (
            Input.GetKeyDown(KeyCode.Space) &&
            _playerController.IsGrounded
        )
        {
            _animator.SetTrigger("Jump");
        }
    }

    #endregion
}