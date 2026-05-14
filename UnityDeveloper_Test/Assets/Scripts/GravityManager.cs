using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages gravity direction changes for the entire game.
/// Rotates the level and player orientation smoothly when gravity changes.
/// </summary>
public class GravityManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GravityManager.
    /// </summary>
    public static GravityManager Instance { get; private set; }

    /// <summary>
    /// Event fired whenever gravity changes.
    /// Sends the new gravity direction.
    /// </summary>
    public event Action<Vector3> OnGravityChanged;

    /// <summary>
    /// Current gravity direction of the game.
    /// </summary>
    public Vector3 CurrentGravityDirection => currentGravityDirection;

    /// <summary>
    /// Returns true while gravity transition rotation is occurring.
    /// </summary>
    public bool IsTransitioning => isTransitioning;

    [Header("References")]
    [SerializeField]
    private Transform levelRoot;

    [SerializeField]
    private PlayerController playerController;

    [Header("Settings")]
    [SerializeField]
    private float rotationDuration = 0.5f;

    private Vector3 currentGravityDirection = Vector3.down;

    private bool isTransitioning;

    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Requests a gravity change to a new gravity direction.
    /// </summary>
    /// <param name="newGravityDirection">
    /// The target gravity direction.
    /// Example: Vector3.left, Vector3.right, Vector3.up, Vector3.down
    /// </param>
    public void RequestGravityChange(Vector3 newGravityDirection)
    {
        if (isTransitioning)
        {
            return;
        }

        newGravityDirection = newGravityDirection.normalized;

        if (newGravityDirection == currentGravityDirection)
        {
            return;
        }

        StartCoroutine(RotateGravity(newGravityDirection));
    }

    /// <summary>
    /// Smoothly rotates the level and updates player orientation.
    /// </summary>
    /// <param name="newGravityDirection">The new gravity direction.</param>
    private IEnumerator RotateGravity(Vector3 newGravityDirection)
    {
        isTransitioning = true;

        Vector3 currentUp = -currentGravityDirection;
        Vector3 targetUp = -newGravityDirection;

        Quaternion startRotation = levelRoot.rotation;

        Quaternion gravityRotation =
            Quaternion.FromToRotation(currentUp, targetUp);

        Quaternion targetRotation =
            gravityRotation * levelRoot.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / rotationDuration;

            levelRoot.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        levelRoot.rotation = targetRotation;

        currentGravityDirection = newGravityDirection;

        // Notify player controller of new orientation.
        playerController.SetGravityDirection(-currentGravityDirection);

        // Fire gravity changed event.
        OnGravityChanged?.Invoke(currentGravityDirection);

        isTransitioning = false;
    }
}