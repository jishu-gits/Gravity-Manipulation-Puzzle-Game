using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages gravity direction changes for the game.
/// Rotates the level smoothly and updates player orientation.
/// </summary>
public class GravityManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static GravityManager Instance { get; private set; }

    /// <summary>
    /// Event triggered whenever gravity changes.
    /// </summary>
    public event Action<Vector3> OnGravityChanged;

    /// <summary>
    /// Current gravity direction.
    /// </summary>
    public Vector3 CurrentGravityDirection => currentGravityDirection;

    /// <summary>
    /// Returns true while gravity transition is occurring.
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
    /// Initializes singleton instance.
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
    /// Requests a gravity direction change.
    /// </summary>
    /// <param name="newGravityDirection">
    /// The target gravity direction.
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
    /// Smoothly rotates the level and updates player gravity orientation.
    /// </summary>
    /// <param name="newGravityDirection">
    /// The target gravity direction.
    /// </param>
    private IEnumerator RotateGravity(Vector3 newGravityDirection)
    {
        isTransitioning = true;

        Vector3 currentUp = -currentGravityDirection;

        Vector3 targetUp = -newGravityDirection;

        Quaternion startRotation = levelRoot.rotation;

        Quaternion gravityRotation =
            Quaternion.FromToRotation(
                currentUp,
                targetUp
            );

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

        if (playerController != null)
        {
            playerController.SetGravityDirection(
                -currentGravityDirection
            );
        }

        OnGravityChanged?.Invoke(currentGravityDirection);

        isTransitioning = false;
    }
}