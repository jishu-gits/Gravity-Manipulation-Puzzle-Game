using UnityEngine;

/// <summary>
/// Manages collectible cube tracking,
/// collection progress, and win conditions.
/// </summary>
public class CollectionManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Singleton instance of the CollectionManager.
    /// </summary>
    public static CollectionManager Instance { get; private set; }

    [Header("Debug")]

    /// <summary>
    /// Enables collection progress debug logging.
    /// </summary>
    [SerializeField]
    private bool _enableDebugLogs = true;

    /// <summary>
    /// Total number of collectible cubes in the scene.
    /// </summary>
    private int _totalCubes;

    /// <summary>
    /// Number of cubes collected by the player.
    /// </summary>
    private int _collectedCubes;

    /// <summary>
    /// Cached array of collectible cubes discovered in the scene.
    /// </summary>
    private CollectibleCube[] _collectibleCubes;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the total number of collectible cubes in the scene.
    /// </summary>
    public int TotalCubes => _totalCubes;

    /// <summary>
    /// Gets the number of cubes collected by the player.
    /// </summary>
    public int CollectedCubes => _collectedCubes;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes singleton instance and discovers collectible cubes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Automatically discover all collectible cubes
        // currently active in the scene.
        _collectibleCubes =
            FindObjectsOfType<CollectibleCube>();

        _totalCubes = _collectibleCubes.Length;

        _collectedCubes = 0;
    }

    /// <summary>
    /// Initializes HUD values after scene load.
    /// </summary>
    private void Start()
    {
        UpdateCubeCountUI();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Handles collectible cube collection events.
    /// </summary>
    /// <param name="cube">
    /// The cube that was collected.
    /// </param>
    public void OnCubeCollected(CollectibleCube cube)
    {
        if (cube == null)
        {
            return;
        }

        _collectedCubes++;

        if (_enableDebugLogs)
        {
            Debug.Log(
                "Collected Cubes: " +
                _collectedCubes +
                " / " +
                _totalCubes
            );
        }

        UpdateCubeCountUI();

        // Trigger game win when all cubes are collected.
        if (AreAllCubesCollected())
        {
            if (_enableDebugLogs)
            {
                Debug.Log("All cubes collected!");
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerWin();
            }
        }
    }

    /// <summary>
    /// Returns whether all collectible cubes
    /// in the scene have been collected.
    /// </summary>
    /// <returns>
    /// True if all cubes are collected;
    /// otherwise false.
    /// </returns>
    public bool AreAllCubesCollected()
    {
        return _collectedCubes >= _totalCubes;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the cube collection count on the HUD.
    /// </summary>
    private void UpdateCubeCountUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCubeCount(
                _collectedCubes,
                _totalCubes
            );
        }
    }

    #endregion
}