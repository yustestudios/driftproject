using EVP;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ControllerEvents : MonoBehaviour
{
    public VehicleAudio vehicleAudio;
    public float minRpmForVibration = 3000f;
    public float maxRpmForVibration = 6000f;
    public float minVibrationIntensity = 0.1f;
    public float maxVibrationIntensity = 1.0f;

    private Gamepad gamepad;
    private bool isVibrating = false;

    void Start()
    {
        // Obtener el gamepad (mando)
        string gamepadLayout = "DualShockGamepadHID"; // Ajusta según tu plataforma y mando
        gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.LogError("No se pudo encontrar el gamepad.");
        }
    }

    void Update()
    {
        if (vehicleAudio != null && gamepad != null)
        {
            UpdateVibration();
        }
    }

    void UpdateVibration()
    {
        // Obtener las RPM del vehículo
        float rpm = vehicleAudio.m_engineRpm;

        // Calcular la intensidad de la vibración
        float vibrationIntensity = Mathf.InverseLerp(minRpmForVibration, maxRpmForVibration, rpm);
        vibrationIntensity = Mathf.Clamp01(vibrationIntensity);

        // Aplicar la vibración
        if (vibrationIntensity > 0 && !isVibrating)
        {
            StartCoroutine(VibrateController(vibrationIntensity));
        }
        else if (vibrationIntensity == 0 && isVibrating)
        {
            StopVibration();
        }
    }

    IEnumerator VibrateController(float intensity)
    {
        // Ajustar la intensidad (porcentaje)
        intensity = Mathf.Lerp(minVibrationIntensity, maxVibrationIntensity, intensity);

        // Aplicar la vibración durante un corto periodo de tiempo
        gamepad.SetMotorSpeeds(intensity, intensity);
        isVibrating = true;

        // Esperar un breve momento antes de detener la vibración
        yield return new WaitForSeconds(0.1f);

        StopVibration();
    }

    void StopVibration()
    {
        gamepad.SetMotorSpeeds(0, 0);
        isVibrating = false;
    }
}
