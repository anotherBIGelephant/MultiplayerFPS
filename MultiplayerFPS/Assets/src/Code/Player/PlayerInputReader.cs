using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool FirePressed { get; private set; }

    public void OnMove(InputAction.CallbackContext value)
    {
        Move = value.action.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        Look = value.action.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed)
            JumpPressed = true;
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (value.performed)
            FirePressed = true;
    }

    public void ConsumeJump()
    {
        JumpPressed = false;
    }

    public void ConsumeFire()
    {
        FirePressed = false;
    }

    public void ClearInput()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        JumpPressed = false;
        FirePressed = false;
    }
}
