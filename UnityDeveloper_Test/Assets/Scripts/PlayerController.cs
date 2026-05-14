using UnityEngine;

/// <summary>
/// Controls player movement for a gravity manipulation puzzle game.
/// Uses a CharacterController instead of Rigidbody physics.
/// Supports dynamic gravity directions and movement relative to the player's current up direction.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Movement speed of the player.
    /// </summary>
    public float MoveSpeed = 5f;

    /// <summary>
    /// Jump height applied opposite to gravity.
    /// </summary>
    public float JumpHeight = 8f;

    /// <summary>
    /// Manual gravity strength applied every frame.
    /// </summary>
    public float GravityStrength = -20f;

    /// <summary>
    /// Radius used for ground detection sphere cast.
    /// </summary>
    public float GroundCheckRadius = 0.3f;

    /// <summary>
    /// Distance used for ground detection sphere cast.
    /// </summary>
    public float GroundCheckDistance = 0.4f;

    /// <summary>
    /// Layers considered valid ground.
    /// </summary>
    public LayerMask GroundMask;

    private CharacterController characterController;

    // Current velocity affected by gravity.
    private Vector3 velocity;

    // Current "up" direction for the player.
    private Vector3 currentUp = Vector3.up;

    // Current gravity direction.
    private Vector3 gravityDirection = Vector3.down;

    // Tracks whether player is grounded.
    private bool isGrounded;

    /// <summary>
    /// Initializes required components.
    /// </summary>
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Handles movement, gravity, jumping, and rotation every frame.
    /// </summary>
    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        AlignToGravity();
    }

    /// <summary>
    /// Sets a new gravity orientation for the player.
    /// The provided vector represents the new "up" direction.
    /// </summary>
    /// <param name="newUp">The new up direction.</param>
    public void SetGravityDirection(Vector3 newUp)
    {
        currentUp = newUp.normalized;
        gravityDirection = -currentUp;

        // Remove any velocity component that conflicts with the new gravity.
        velocity = Vector3.ProjectOnPlane(velocity, currentUp);
    }

    /// <summary>
    /// Handles WASD movement relative to the current gravity orientation.
    /// </summary>
    private void HandleMovement()
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

        // Camera-relative movement projected onto movement plane.
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Remove components aligned with gravity.
        cameraForward = Vector3.ProjectOnPlane(cameraForward, currentUp).normalized;
        cameraRight = Vector3.ProjectOnPlane(cameraRight, currentUp).normalized;

        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        Vector3 movement = moveDirection * MoveSpeed;

        characterController.Move(movement * Time.deltaTime);
    }

    /// <summary>
    /// Applies jump force opposite to gravity direction.
    /// </summary>
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity = currentUp * JumpHeight;
        }
    }

    /// <summary>
    /// Applies manual gravity force over time.
    /// </summary>
    private void ApplyGravity()
    {
        // Small downward force keeps CharacterController grounded properly.
        if (isGrounded && Vector3.Dot(velocity, gravityDirection) > -2f)
        {
            velocity = gravityDirection * 2f;
        }

        velocity += gravityDirection * Mathf.Abs(GravityStrength) * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Detects whether the player is grounded.
    /// </summary>
    private void HandleGroundCheck()
    {
        Vector3 castOrigin =
            transform.position + currentUp * 0.5f;

        Vector3 castDirection = -currentUp;

        isGrounded = Physics.SphereCast(
            castOrigin,
            0.4f,
            castDirection,
            out RaycastHit hit,
            0.8f,
            GroundMask
        );

        Debug.DrawRay(
            castOrigin,
            castDirection * 0.8f,
            isGrounded ? Color.green : Color.red
        );
    }
    /// <summary>
    /// Smoothly rotates the player so its up direction aligns
    /// with the current gravity orientation.
    /// </summary>
    private void AlignToGravity()
    {
        Quaternion targetRotation =
            Quaternion.FromToRotation(transform.up, currentUp) * transform.rotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
    }

    /// <summary>
    /// Draws the ground check sphere cast gizmo in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 upDirection =
            currentUp == Vector3.zero ? Vector3.up : currentUp;

        Vector3 downDirection = -upDirection;

        Vector3 sphereOrigin =
            transform.position +
            upDirection * 0.1f;

        Gizmos.DrawWireSphere(
            sphereOrigin + downDirection * GroundCheckDistance,
            GroundCheckRadius
        );
    }
}