using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents possible reasons for a game over state.
/// </summary>
public enum GameOverReason
{
    /// <summary>
    /// The player ran out of time.
    /// </summary>
    TimeUp,

    /// <summary>
    /// The player fell out of the playable area.
    /// </summary>
    Fell
}

/// <summary>
/// Controls overall game flow,
/// including win conditions, game over handling,
/// scene restarting, and optional scene progression.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Default scene index increment used
    /// when loading the next scene.
    /// </summary>
    private const int NextSceneOffset = 1;

    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Returns whether the game has ended.
    /// </summary>
    public bool IsGameOver { get; private set; }

    [Header("References")]

    /// <summary>
    /// Reference to the PlayerController component.
    /// Used for enabling/disabling player input.
    /// </summary>
    [SerializeField]
    private PlayerController _playerController;

    [Header("Win Settings")]

    /// <summary>
    /// Determines whether the next scene
    /// should load automatically after winning.
    /// </summary>
    [SerializeField]
    private bool _loadNextSceneOnWin = false;

    /// <summary>
    /// Delay before loading the next scene after victory.
    /// </summary>
    [SerializeField]
    private float _nextSceneDelay = 3f;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Initializes singleton instance and resets time scale.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensure normal gameplay speed when entering the scene.
        Time.timeScale = 1f;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Triggers the game over state.
    /// </summary>
    /// <param name="reason">
    /// The reason for the game over.
    /// </param>
    public void TriggerGameOver(GameOverReason reason)
    {
        // Prevent duplicate game-over execution.
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

        DisablePlayerInput();

        // Display the corresponding game-over message.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(
                reason.ToString()
            );
        }
    }

    /// <summary>
    /// Triggers the game win state.
    /// </summary>
    public void TriggerWin()
    {
        // Prevent duplicate win execution.
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

        DisablePlayerInput();

        // Display victory UI.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowWin();
        }

        // Optionally load the next scene automatically.
        if (_loadNextSceneOnWin)
        {
            StartCoroutine(LoadNextSceneRoutine());
        }
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// </summary>
    public void RestartGame()
    {
        // Reset gameplay speed before scene reload.
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Disables player input controls.
    /// </summary>
    private void DisablePlayerInput()
    {
        if (_playerController != null)
        {
            _playerController.SetInputEnabled(false);
        }
    }

    /// <summary>
    /// Loads the next scene after a short delay.
    /// </summary>
    /// <returns>
    /// Coroutine enumerator.
    /// </returns>
    private IEnumerator LoadNextSceneRoutine()
    {
        yield return new WaitForSeconds(_nextSceneDelay);

        int nextSceneIndex =
            SceneManager.GetActiveScene().buildIndex +
            NextSceneOffset;

        // Prevent attempting to load scenes
        // outside the build settings range.
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    #endregion
}