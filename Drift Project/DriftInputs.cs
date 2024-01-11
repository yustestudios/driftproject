using UnityEngine;
using UnityEngine.InputSystem;

public class DriftInputs : MonoBehaviour
{
    [Header("Drift Input Values")]
    public float verticalPS5;
    public float brakePS5;
    public Vector2 leftStick;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;

    #if ENABLE_INPUT_SYSTEM
    public void OnVerticalPS5(InputValue value)
    {
        verticalPS5 = value.Get<float>();
    }

    public void OnBrakePS5(InputValue value)
    {
        brakePS5 = value.Get<float>();
    }

    public void OnLeftStick(InputValue value)
    {
        leftStick = value.Get<Vector2>();
    }
    #endif

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    public void VerticalPS5Input(float newVerticalPS5State)
    {
        verticalPS5 = newVerticalPS5State;
    }

    public void BrakePS5Input(float newBrakePS5State)
    {
        brakePS5 = newBrakePS5State;
    }

    public void LeftStickInput(Vector2 newLeftStickState)
    {
        leftStick = newLeftStickState;
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void VibratePS5(float intensity)
    {
        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            // Asegúrate de que la intensidad esté en el rango correcto
            intensity = Mathf.Clamp01(intensity);

            // Aplica la vibración a ambos motores del controlador PS5
            gamepad.SetMotorSpeeds(0, intensity); // Motor izquierdo
            gamepad.SetMotorSpeeds(1, intensity); // Motor derecho
        }
    }
}
