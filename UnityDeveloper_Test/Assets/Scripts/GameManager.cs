using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerWin()
    {
        Debug.Log("YOU WIN");
    }
}