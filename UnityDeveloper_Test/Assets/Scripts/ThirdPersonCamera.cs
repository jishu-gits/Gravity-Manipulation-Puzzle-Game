using UnityEngine;

/// <summary>
/// Third-person camera that orbits the player.
/// On gravity change, smoothly rolls (Z) or pitches (X)
/// around the player pivot to match the new up direction.
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Orbit")]
    [SerializeField] private float distance     = 6f;
    [SerializeField] private float height       = 2.5f;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Smoothing")]
    [SerializeField] private float followSpeed  = 8f;
    [SerializeField] private float gravityRollSpeed = 5f;   // Z / X roll speed

    // ── state ──────────────────────────────────────────────
    private float   _yaw             = 0f;

    // Target up is set instantly on gravity change.
    // Smoothed up interpolates toward it — drives the roll/pitch.
    private Vector3 _targetGravityUp = Vector3.up;
    private Vector3 _smoothedGravityUp = Vector3.up;

    // ── lifecycle ──────────────────────────────────────────
    private void Start()
    {
        if (GravityManager.Instance != null)
            GravityManager.Instance.OnGravityChanged += OnGravityChanged;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void OnDestroy()
    {
        if (GravityManager.Instance != null)
            GravityManager.Instance.OnGravityChanged -= OnGravityChanged;
    }

    /// <summary>
    /// Receives the new UP vector from GravityManager.
    /// GravityManager.OnGravityChanged fires currentUp (not down).
    /// </summary>
    private void OnGravityChanged(Vector3 newUp)
    {
        _targetGravityUp = newUp;         // store; smoothing happens in LateUpdate
    }

    // ── camera update ──────────────────────────────────────
    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Smoothly slerp the gravity up — this drives the roll / pitch.
        //    Slerp on the unit sphere naturally rolls Z for left/right
        //    and pitches X for forward/back gravity.
        _smoothedGravityUp = Vector3.Slerp(
            _smoothedGravityUp,
            _targetGravityUp,
            Time.deltaTime * gravityRollSpeed
        );

        // 2. Mouse horizontal orbit around the smoothed up axis.
        _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        // 3. Build camera orientation:
        //    a) Align world-up with the smoothed gravity up  →  handles Z / X roll
        //    b) Apply yaw around that new up axis            →  horizontal orbit
        Quaternion gravityAlign = Quaternion.FromToRotation(Vector3.up, _smoothedGravityUp);
        Quaternion yawRot       = Quaternion.AngleAxis(_yaw, _smoothedGravityUp);
        Quaternion desiredRot   = yawRot * gravityAlign;

        // 4. Position: fixed offset from player pivot using rotated camera axes.
        //    new Vector3(0, height, -distance) is always in the camera's own space,
        //    so the pivot (player) never moves — only the camera rotates around it.
        Vector3 desiredPos = target.position
                           + desiredRot * new Vector3(0f, height, -distance);

        // 5. Smooth position follow.
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * followSpeed
        );

        // 6. Look at a point one unit above the player in the smoothed-up direction.
        //    Passing _smoothedGravityUp as the up-vector is what constrains
        //    the roll to Z / X and prevents the camera from spinning on Y.
        Vector3 lookTarget = target.position + _smoothedGravityUp * 1f;
        transform.LookAt(lookTarget, _smoothedGravityUp);
    }
}