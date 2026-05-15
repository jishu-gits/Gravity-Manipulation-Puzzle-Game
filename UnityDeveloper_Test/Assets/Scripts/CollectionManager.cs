using UnityEngine;

/// <summary>
/// Manages collectible cube tracking and win conditions.
/// </summary>
public class CollectionManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the CollectionManager.
    /// </summary>
    public static CollectionManager Instance { get; private set; }

    /// <summary>
    /// Total number of collectible cubes in the scene.
    /// </summary>
    public int TotalCubes => totalCubes;

    /// <summary>
    /// Number of cubes collected by the player.
    /// </summary>
    public int CollectedCubes => collectedCubes;

    private int totalCubes;

    private int collectedCubes;

    private CollectibleCube[] collectibleCubes;

    /// <summary>
    /// Initializes singleton and discovers collectible cubes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        collectibleCubes =
            FindObjectsOfType<CollectibleCube>();

        totalCubes = collectibleCubes.Length;

        collectedCubes = 0;
    }

    /// <summary>
    /// Updates UI after scene loads.
    /// </summary>
    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCubeCount(
                collectedCubes,
                totalCubes
            );
        }
    }

    /// <summary>
    /// Handles cube collection events.
    /// </summary>
    /// <param name="cube">The collected cube.</param>
    public void OnCubeCollected(CollectibleCube cube)
    {
        collectedCubes++;

        Debug.Log(
            "Collected Cubes: " +
            collectedCubes +
            " / " +
            totalCubes
        );

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCubeCount(
                collectedCubes,
                totalCubes
            );
        }

        if (AreAllCubesCollected())
        {
            Debug.Log("All cubes collected!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerWin();
            }
        }
    }

    /// <summary>
    /// Returns whether all cubes have been collected.
    /// </summary>
    /// <returns>
    /// True if all cubes are collected.
    /// </returns>
    public bool AreAllCubesCollected()
    {
        return collectedCubes >= totalCubes;
    }
}