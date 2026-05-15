using UnityEngine;

/// <summary>
/// Controls game state and win/loss conditions.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGameOver => isGameOver;

    private bool isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TriggerGameOver(GameOverReason reason)
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        string reasonText = "";

        switch (reason)
        {
            case GameOverReason.TimeUp:
                reasonText = "Time's Up!";
                break;

            case GameOverReason.Fell:
                reasonText = "You Fell!";
                break;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(reasonText);
        }

        Time.timeScale = 0f;
    }

    public void TriggerWin()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowWin();
        }

        Time.timeScale = 0f;
    }
}