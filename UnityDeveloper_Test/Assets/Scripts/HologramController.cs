using UnityEngine;

public class HologramController : MonoBehaviour
{
    public static HologramController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowHologram(Vector3 direction)
    {
    }

    public void HideHologram()
    {
    }
}