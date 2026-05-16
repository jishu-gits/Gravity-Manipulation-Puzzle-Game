using UnityEngine;

public class GravityInputHandler : MonoBehaviour
{
    private Vector3 previewDirection;

    private void Update()
    {
        if (GravityManager.Instance.IsTransitioning)
        {
            return;
        }

        HandlePreview();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            GravityManager.Instance
                .RequestGravityChange(previewDirection);

            HologramController.Instance.HideHologram();
        }
    }

    private void HandlePreview()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            SetPreview(-transform.forward);
            return;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            SetPreview(transform.forward);
            return;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            SetPreview(-transform.right);
            return;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            SetPreview(transform.right);
            return;
        }

        HologramController.Instance.HideHologram();
    }

    private void SetPreview(Vector3 direction)
    {
        previewDirection = direction.normalized;

        HologramController.Instance
            .ShowHologram(previewDirection);
    }
}