using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EVP;

public class CameraModeConfig : MonoBehaviour
{
    public VehicleCameraController vehicleCameraController;
    private Camera camera;
    private float initialFov;

    public WheelSkid wheelSkid;
    public Rigidbody carRigidbody;

    public float vibrationFrequency = 5f;  // Frecuencia de vibración (ajustable según preferencias)
    public float vibrationAmplitude = 0.5f;  // Amplitud de vibración (ajustable según preferencias)

    void Start()
    {
        InitializeCamera();
    }

    void LateUpdate()
    {
        AdjustCameraMode();
    }

    private void InitializeCamera()
    {
        camera = GetComponent<Camera>();
        initialFov = camera.fieldOfView;
    }

    private void AdjustCameraMode()
    {
        if (vehicleCameraController.mode == VehicleCameraController.Mode.AttachTo)
        {
            // Ajustes específicos para el modo AttachTo
            AdjustAttachToMode();
        }
        else if (vehicleCameraController.mode == VehicleCameraController.Mode.SmoothFollow)
        {
            // Ajustes específicos para el modo SmoothFollow
            AdjustSmoothFollowMode();
        }
        else if (vehicleCameraController.mode == VehicleCameraController.Mode.LookAt)
        {
            // Ajustes específicos para el modo LookAt
            AdjustLookAtMode();
        }
        else
        {
            // Ajustes por defecto para otros modos
            AdjustDefaultMode();
        }
    }

    private void AdjustAttachToMode()
    {
        // Ajustes específicos para el modo AttachTo
        camera.fieldOfView = 45f;
        gameObject.transform.localPosition = new Vector3(0f, -0.07f, -0.25f);

        float rotationY = (carRigidbody.velocity.magnitude * 2.23693629f * 1.609344f > 5f) ?
            Mathf.Lerp(-30f, 30f, (Mathf.Clamp(normalizedDriftAngle + 1f, -1f, 1f) / 2f)) :
            0f;

        Quaternion rotation = Quaternion.Euler(0f, rotationY, 0f);
        Vector3 localEulerAngles = rotation.eulerAngles;
        gameObject.transform.localEulerAngles = localEulerAngles;
    }

    private void AdjustSmoothFollowMode()
    {
        // Ajustes específicos para el modo SmoothFollow
        float speedKMH = carRigidbody.velocity.magnitude * 2.23693629f * 1.609344f;
        float maxSpeed = 200f;
        float minFov = 72f;
        float maxFov = 100f;

        float fov = Mathf.Lerp(minFov, maxFov, Mathf.Clamp01(speedKMH / maxSpeed));
        camera.fieldOfView = fov;

        float vibration = Mathf.Lerp(0f, vibrationAmplitude, Mathf.Clamp01(speedKMH / maxSpeed));
        float vibrationAngle = Mathf.Sin(Time.time * vibrationFrequency) * vibration;
        gameObject.transform.localEulerAngles = new Vector3(0f, 0f, vibrationAngle);

        float positionNoise = Mathf.Lerp(0f, vibrationAmplitude, Mathf.Clamp01(speedKMH / maxSpeed));
        float positionNoiseValue = Mathf.PerlinNoise(Time.time * vibrationFrequency, 0f) * positionNoise;
        float positionZ = positionNoiseValue - positionNoise / 2f; // Ajuste para centrar el ruido
        gameObject.transform.localPosition = new Vector3(0f, 0f, positionZ);
    }

    private void AdjustLookAtMode()
    {
        // Ajustes específicos para el modo LookAt
        gameObject.transform.localPosition = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        Vector3 localEulerAngles = rotation.eulerAngles;
        gameObject.transform.localEulerAngles = localEulerAngles;
    }

    private void AdjustDefaultMode()
    {
        // Ajustes por defecto para otros modos
        gameObject.transform.localPosition = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        Vector3 localEulerAngles = rotation.eulerAngles;
        gameObject.transform.localEulerAngles = localEulerAngles;

        if (vehicleCameraController.mode != VehicleCameraController.Mode.LookAt || vehicleCameraController.mode != VehicleCameraController.Mode.Free)
        {
            camera.fieldOfView = initialFov;
        }
    }
}