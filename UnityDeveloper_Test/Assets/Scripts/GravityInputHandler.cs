using UnityEngine;

/// <summary>
/// Handles gravity direction preview and confirmation input.
/// Arrow keys select a gravity direction preview,
/// while Enter applies the selected gravity change.
/// </summary>
public class GravityInputHandler : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Message prefix used for debug logging.
    /// </summary>
    private const string PreviewLogPrefix =
        "Preview Gravity Direction: ";

    /// <summary>
    /// Key used to confirm gravity selection.
    /// </summary>
    private const KeyCode ConfirmKey = KeyCode.Return;

    [Header("Debug")]

    /// <summary>
    /// Enables debug logging for gravity previews.
    /// </summary>
    [SerializeField]
    private bool _enableDebugLogs = true;

    /// <summary>
    /// Stores the currently previewed gravity direction.
    /// </summary>
    private Vector3 _currentPreviewDirection = Vector3.down;

    /// <summary>
    /// Tracks whether a gravity direction
    /// has been selected for preview.
    /// </summary>
    private bool _hasPreviewSelection;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the currently previewed gravity direction.
    /// </summary>
    public Vector3 CurrentPreviewDirection =>
        _currentPreviewDirection;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Handles gravity input every frame.
    /// </summary>
    private void Update()
    {
        // Prevent gravity input when no manager exists.
        if (GravityManager.Instance == null)
        {
            return;
        }

        // Disable input during gravity transition rotation.
        if (GravityManager.Instance.IsTransitioning)
        {
            return;
        }

        HandleDirectionSelection();
        HandleGravityConfirmation();
        HandlePreviewRelease();
    }

    #endregion

    #region Public Methods

    // Intentionally left empty.
    // Reserved for future external gravity-input APIs.

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles arrow-key input for gravity preview selection.
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
    /// Confirms and applies the selected gravity direction.
    /// </summary>
    private void HandleGravityConfirmation()
    {
        if (
            Input.GetKeyDown(ConfirmKey) &&
            _hasPreviewSelection
        )
        {
            GravityManager.Instance.RequestGravityChange(
                _currentPreviewDirection
            );

            // Hide hologram preview after gravity confirmation.
            if (HologramController.Instance != null)
            {
                HologramController.Instance.HideHologram();
            }

            _hasPreviewSelection = false;
        }
    }

    /// <summary>
    /// Hides the hologram preview when arrow keys are released.
    /// </summary>
    private void HandlePreviewRelease()
    {
        bool releasedArrowKey =
            Input.GetKeyUp(KeyCode.UpArrow) ||
            Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow);

        if (!releasedArrowKey)
        {
            return;
        }

        if (HologramController.Instance != null)
        {
            HologramController.Instance.HideHologram();
        }
    }

    /// <summary>
    /// Updates the currently previewed gravity direction.
    /// </summary>
    /// <param name="direction">
    /// Target gravity direction for preview.
    /// </param>
    private void SetPreviewDirection(Vector3 direction)
    {
        _currentPreviewDirection = direction.normalized;

        _hasPreviewSelection = true;

        // Output preview direction for debugging purposes.
        if (_enableDebugLogs)
        {
            Debug.Log(
                PreviewLogPrefix +
                _currentPreviewDirection
            );
        }

        // Display hologram preview for selected direction.
        if (HologramController.Instance != null)
        {
            HologramController.Instance.ShowHologram(
                _currentPreviewDirection
            );
        }
    }

    #endregion
}