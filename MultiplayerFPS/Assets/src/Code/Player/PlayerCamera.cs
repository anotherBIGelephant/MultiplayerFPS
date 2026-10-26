using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 0.08f;
    [SerializeField] private float minPitch = -89f;
    [SerializeField] private float maxPitch = 89f;

    private float pitch;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LateUpdate()
    {
        if (inputReader == null || playerBody == null)
            return;

        Vector2 look = inputReader.Look;

        // Mouse delta is already a per-frame displacement.
        // Do not multiply it by deltaTime.
        float yawDelta = look.x * sensitivity;
        float pitchDelta = look.y * sensitivity;

        playerBody.Rotate(Vector3.up * yawDelta);

        pitch = Mathf.Clamp(
            pitch - pitchDelta,
            minPitch,
            maxPitch
        );

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
