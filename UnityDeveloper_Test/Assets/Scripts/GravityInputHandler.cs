using UnityEngine;

/// <summary>
/// Handles gravity direction selection and confirmation input.
/// Arrow keys preview gravity direction.
/// Enter key applies the selected gravity direction.
/// </summary>
public class GravityInputHandler : MonoBehaviour
{
    /// <summary>
    /// Currently previewed gravity direction.
    /// </summary>
    public Vector3 CurrentPreviewDirection => currentPreviewDirection;

    private Vector3 currentPreviewDirection = Vector3.down;

    private bool hasPreviewSelection;

    /// <summary>
    /// Handles gravity preview selection and confirmation input.
    /// </summary>
    private void Update()
    {
        if (GravityManager.Instance == null)
        {
            return;
        }

        if (GravityManager.Instance.IsTransitioning)
        {
            return;
        }

        HandleDirectionSelection();
        HandleGravityConfirmation();
        HandlePreviewRelease();
    }

    /// <summary>
    /// Handles arrow key input for gravity direction preview.
    /// </summary>
    private void HandleDirectionSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetPreviewDirection(-transform.forward);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetPreviewDirection(transform.forward);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetPreviewDirection(-transform.right);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetPreviewDirection(transform.right);
        }
    }

    /// <summary>
    /// Applies the currently selected gravity direction when Enter is pressed.
    /// </summary>
    private void HandleGravityConfirmation()
    {
        if (Input.GetKeyDown(KeyCode.Return) && hasPreviewSelection)
        {
            GravityManager.Instance.RequestGravityChange(currentPreviewDirection);

            if (HologramController.Instance != null)
            {
                HologramController.Instance.HideHologram();
            }

            hasPreviewSelection = false;
        }
    }

    /// <summary>
    /// Hides the hologram preview when arrow keys are released.
    /// </summary>
    private void HandlePreviewRelease()
    {
        if (
            Input.GetKeyUp(KeyCode.UpArrow) ||
            Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow)
        )
        {
            if (HologramController.Instance != null)
            {
                HologramController.Instance.HideHologram();
            }
        }
    }

    /// <summary>
    /// Sets the currently previewed gravity direction.
    /// </summary>
    /// <param name="direction">The preview gravity direction.</param>
    private void SetPreviewDirection(Vector3 direction)
    {
        currentPreviewDirection = direction.normalized;

        hasPreviewSelection = true;

        Debug.Log("Preview Gravity Direction: " + currentPreviewDirection);

        if (HologramController.Instance != null)
        {
            HologramController.Instance.ShowHologram(currentPreviewDirection);
        }
    }
}