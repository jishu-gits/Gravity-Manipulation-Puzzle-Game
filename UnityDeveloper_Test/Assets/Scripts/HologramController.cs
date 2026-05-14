using UnityEngine;

/// <summary>
/// Controls the hologram preview used for gravity direction visualization.
/// Displays a ghost version of the player before gravity changes are confirmed.
/// </summary>
public class HologramController : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the HologramController.
    /// </summary>
    public static HologramController Instance { get; private set; }

    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private GameObject hologramObject;

    [Header("Settings")]
    [SerializeField]
    private float previewOffsetDistance = 2f;

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

        HideHologram();
    }

    /// <summary>
    /// Shows the hologram preview in the selected gravity direction.
    /// </summary>
    /// <param name="gravityDirection">
    /// The gravity direction being previewed.
    /// </param>
    public void ShowHologram(Vector3 gravityDirection)
    {
        if (player == null || hologramObject == null)
        {
            return;
        }

        hologramObject.SetActive(true);

        Vector3 targetUp = -gravityDirection.normalized;

        Vector3 previewPosition =
            player.position +
            gravityDirection.normalized * previewOffsetDistance;

        hologramObject.transform.position = previewPosition;

        Quaternion targetRotation =
            Quaternion.FromToRotation(Vector3.up, targetUp);

        hologramObject.transform.rotation = targetRotation;
    }

    /// <summary>
    /// Hides the hologram preview object.
    /// </summary>
    public void HideHologram()
    {
        if (hologramObject != null)
        {
            hologramObject.SetActive(false);
        }
    }
}