using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravityStrength = 20f;

    [Header("Ground Check")]
    [SerializeField] private float groundedDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;

    private Vector3 velocity;

    private Vector3 gravityUp = Vector3.up;

    private bool isGrounded;

    private bool inputEnabled = true;

    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!inputEnabled)
        {
            return;
        }

        GroundCheck();

        HandleMovement();

        HandleJump();

        ApplyGravity();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            -gravityUp,
            groundedDistance,
            groundMask
        );

        if (isGrounded && Vector3.Dot(velocity, -gravityUp) > 0f)
        {
            velocity = Vector3.zero;
        }
    }

    private void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // ONLY WASD movement.
        // Arrow keys are reserved for gravity preview.

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

        Vector3 forward =
            Vector3.Cross(
                transform.right,
                gravityUp
            ).normalized;

        Vector3 moveDirection =
            (
                transform.right * horizontal +
                forward * vertical
            ).normalized;

        controller.Move(
            moveDirection *
            moveSpeed *
            Time.deltaTime
        );
    }

    private void HandleJump()
    {
        if (
            Input.GetKeyDown(KeyCode.Space) &&
            isGrounded
        )
        {
            velocity = gravityUp * jumpForce;
        }
    }

    private void ApplyGravity()
    {
        velocity +=
            -gravityUp *
            gravityStrength *
            Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }

    public void SetGravityUp(Vector3 newUp)
    {
        gravityUp = newUp.normalized;
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }
}