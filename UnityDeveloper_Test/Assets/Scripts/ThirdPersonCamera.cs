using System.Collections;
using UnityEngine;

/// <summary>
/// Third-person camera controller for a gravity manipulation puzzle game.
/// Smoothly follows and rotates around the player while adapting to gravity changes.
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform target;

    [Header("Follow Settings")]
    [SerializeField]
    private float followDistance = 7f;

    [SerializeField]
    private float followHeight = 2f;

    [SerializeField]
    private float followSmoothness = 5f;

    [SerializeField]
    private float rotationSmoothness = 10f;

    [Header("Orbit Settings")]
    [SerializeField]
    private float mouseSensitivity = 2f;

    [Header("Collision")]
    [SerializeField]
    private LayerMask collisionMask;

    [SerializeField]
    private float collisionBuffer = 0.2f;

    private Vector3 currentUp = Vector3.up;

    private float yaw;

    /// <summary>
    /// Subscribes to gravity change events.
    /// </summary>
    private void OnEnable()
    {
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += HandleGravityChanged;
        }
    }

    /// <summary>
    /// Unsubscribes from gravity change events.
    /// </summary>
    private void OnDisable()
    {
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= HandleGravityChanged;
        }
    }

    /// <summary>
    /// Handles camera follow and orbit logic every frame.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        HandleMouseOrbit();
        UpdateCameraPosition();
    }

    /// <summary>
    /// Handles horizontal orbit input using mouse X movement.
    /// </summary>
    private void HandleMouseOrbit()
    {
        float mouseX = Input.GetAxis("Mouse X");

        yaw += mouseX * mouseSensitivity;
    }

    /// <summary>
    /// Updates camera position, collision handling, and look direction.
    /// </summary>
    private void UpdateCameraPosition()
    {
        // Create orbit rotation around the gravity-relative up direction.
        Quaternion orbitRotation =
            Quaternion.AngleAxis(yaw, currentUp);

        // Independent orbit direction.
        // This avoids the camera rotating awkwardly with the player/world.
        Vector3 orbitDirection =
            orbitRotation * Vector3.back;

        // Camera backward offset.
        Vector3 backwardOffset =
            orbitDirection * followDistance;

        // Camera upward offset relative to current gravity orientation.
        Vector3 upwardOffset =
            currentUp * followHeight;

        // Final desired camera position.
        Vector3 desiredPosition =
            target.position +
            backwardOffset +
            upwardOffset;

        // Slightly above player center for better framing.
        Vector3 lookTarget =
            target.position + currentUp * 1.5f;

        // Collision prevention raycast.
        // If geometry blocks the camera, move camera closer.
        Vector3 rayDirection =
            desiredPosition - lookTarget;

        float rayDistance = rayDirection.magnitude;

        rayDirection.Normalize();

        if (
            Physics.Raycast(
                lookTarget,
                rayDirection,
                out RaycastHit hit,
                rayDistance,
                collisionMask
            )
        )
        {
            desiredPosition =
                hit.point - rayDirection * collisionBuffer;
        }

        // Smooth camera position movement.
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmoothness * Time.deltaTime
        );

        // Create gravity-relative camera rotation.
        Quaternion targetRotation =
            Quaternion.LookRotation(
                lookTarget - transform.position,
                currentUp
            );

        // Smoothly align camera orientation.
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothness * Time.deltaTime
        );
    }

    /// <summary>
    /// Handles gravity direction updates from GravityManager.
    /// Smoothly reorients the camera up direction.
    /// </summary>
    /// <param name="newGravityDirection">
    /// The new gravity direction.
    /// </param>
    private void HandleGravityChanged(Vector3 newGravityDirection)
    {
        StopAllCoroutines();

        StartCoroutine(
            SmoothAlignCameraUp(-newGravityDirection.normalized)
        );
    }

    /// <summary>
    /// Smoothly aligns the camera's up direction during gravity transitions.
    /// </summary>
    /// <param name="targetUp">
    /// The target up direction.
    /// </param>
    private IEnumerator SmoothAlignCameraUp(Vector3 targetUp)
    {
        Vector3 startUp = currentUp;

        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            currentUp = Vector3.Slerp(
                startUp,
                targetUp,
                t
            ).normalized;

            yield return null;
        }

        currentUp = targetUp;
    }
}