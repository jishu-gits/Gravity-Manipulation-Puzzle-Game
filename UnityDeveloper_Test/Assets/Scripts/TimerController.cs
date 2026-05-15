using UnityEngine;

/// <summary>
/// Controls the game countdown timer.
/// </summary>
public class TimerController : MonoBehaviour
{
    /// <summary>
    /// Current remaining time in seconds.
    /// </summary>
    public float TimeRemaining => timeRemaining;

    [Header("Settings")]
    [SerializeField]
    private float startTime = 120f;

    private float timeRemaining;

    private bool isTimerRunning = true;

    /// <summary>
    /// Initializes timer values.
    /// </summary>
    private void Start()
    {
        timeRemaining = startTime;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTimer(timeRemaining);
        }
    }

    /// <summary>
    /// Updates countdown timer.
    /// </summary>
    private void Update()
    {
        if (!isTimerRunning)
        {
            return;
        }

        if (
            GameManager.Instance != null &&
            GameManager.Instance.IsGameOver
        )
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        timeRemaining = Mathf.Max(timeRemaining, 0f);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTimer(timeRemaining);
        }

        if (timeRemaining <= 0f)
        {
            isTimerRunning = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver(
                    GameOverReason.TimeUp
                );
            }
        }
    }

    /// <summary>
    /// Stops the timer manually.
    /// </summary>
    public void StopTimer()
    {
        isTimerRunning = false;
    }
}