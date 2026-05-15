using UnityEngine;

/// <summary>
/// Controls character animation states using Animator parameters.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class CharacterAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private PlayerController playerController;

    [Header("Settings")]
    [SerializeField]
    private float animationSmoothness = 10f;

    private float currentSpeed;

    /// <summary>
    /// Initializes references.
    /// </summary>
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
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

    /// <summary>
    /// Updates movement animation using WASD input magnitude.
    /// </summary>
    private void UpdateMovementAnimation()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            vertical = -1f;
        }

        Vector2 movementInput =
            new Vector2(horizontal, vertical);

        float targetSpeed =
            movementInput.magnitude;

        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed,
            animationSmoothness * Time.deltaTime
        );

        animator.SetFloat("Speed", currentSpeed);
    }

    /// <summary>
    /// Updates grounded animation state.
    /// </summary>
    private void UpdateGroundedAnimation()
    {
        animator.SetBool(
            "IsGrounded",
            playerController.IsGrounded
        );
    }

    /// <summary>
    /// Triggers jump animation.
    /// </summary>
    private void HandleJumpAnimation()
    {
        if (
            Input.GetKeyDown(KeyCode.Space) &&
            playerController.IsGrounded
        )
        {
            animator.SetTrigger("Jump");
        }
    }
}