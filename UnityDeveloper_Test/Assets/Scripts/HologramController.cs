using UnityEngine;

public class HologramController : MonoBehaviour
{
    public static HologramController Instance;

    [SerializeField]
    private GameObject hologram;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float previewDistance = 3f;

    private void Awake()
    {
        Instance = this;

        hologram.SetActive(false);
    }

    public void ShowHologram(Vector3 gravityDirection)
    {
        hologram.SetActive(true);

        hologram.transform.position =
            player.position +
            gravityDirection.normalized * previewDistance;

        hologram.transform.rotation =
            Quaternion.FromToRotation(
                Vector3.up,
                -gravityDirection
            );
    }

    public void HideHologram()
    {
        hologram.SetActive(false);
    }
}