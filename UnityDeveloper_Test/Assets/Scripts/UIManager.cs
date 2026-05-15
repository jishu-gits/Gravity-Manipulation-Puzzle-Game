using TMPro;
using UnityEngine;

/// <summary>
/// Handles HUD and end-game UI.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the UIManager.
    /// </summary>
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField]
    private TMP_Text timerText;

    [SerializeField]
    private TMP_Text cubeCountText;

    [Header("Game Over")]
    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private TMP_Text gameOverReasonText;

    [Header("Win")]
    [SerializeField]
    private GameObject winPanel;

    /// <summary>
    /// Initializes singleton and hides panels.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the timer HUD text.
    /// </summary>
    /// <param name="seconds">
    /// Remaining time in seconds.
    /// </param>
    public void UpdateTimer(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);

        int remainingSeconds =
            Mathf.FloorToInt(seconds % 60f);

        timerText.text =
            string.Format("{0:00}:{1:00}",
            minutes,
            remainingSeconds);

        // Turn timer red below 30 seconds.
        if (seconds <= 30f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    /// <summary>
    /// Updates cube collection HUD text.
    /// </summary>
    /// <param name="collected">Collected cube count.</param>
    /// <param name="total">Total cube count.</param>
    public void UpdateCubeCount(int collected, int total)
    {
        cubeCountText.text =
            "Cubes: " + collected + "/" + total;
    }

    /// <summary>
    /// Displays the game over screen.
    /// </summary>
    /// <param name="reason">
    /// Reason for game over.
    /// </param>
    public void ShowGameOver(string reason)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = reason;
        }
    }

    /// <summary>
    /// Displays the win screen.
    /// </summary>
    public void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }
}