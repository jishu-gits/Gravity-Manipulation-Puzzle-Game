using System;
using System.Collections;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance;

    public event Action<Vector3> OnGravityChanged;

    [SerializeField]
    private Transform levelRoot;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private float rotationDuration = 0.5f;

    public bool IsTransitioning { get; private set; }

    private Vector3 currentUp = Vector3.up;

    private void Awake()
    {
        Instance = this;
    }

    public void RequestGravityChange(
        Vector3 targetGravity
    )
    {
        if (IsTransitioning)
        {
            return;
        }

        StartCoroutine(
            RotateWorldRoutine(
                targetGravity.normalized
            )
        );
    }

    private IEnumerator RotateWorldRoutine(
        Vector3 targetGravity
    )
    {
        IsTransitioning = true;

        Vector3 targetUp =
            -targetGravity;

        Quaternion startRotation =
            levelRoot.rotation;

        Quaternion rotationDelta =
            Quaternion.FromToRotation(
                currentUp,
                targetUp
            );

        Quaternion targetRotation =
            rotationDelta *
            levelRoot.rotation;

        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / rotationDuration);
            
            levelRoot.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        levelRoot.rotation = targetRotation;

        currentUp = targetUp;

        player.SetGravityUp(currentUp);

        OnGravityChanged?.Invoke(currentUp);

        IsTransitioning = false;
    }
}