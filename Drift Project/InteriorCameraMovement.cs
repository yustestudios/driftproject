using UnityEngine;

public class InteriorCameraMovement : MonoBehaviour
{
    public Transform carTransform; // Referencia al transform del coche
    public float cameraSensitivity = 1.0f; // Sensibilidad de la cámara

    // Ajustes adicionales para mejorar el realismo
    public float maxPitch = 80.0f; // Límite máximo de inclinación hacia arriba
    public float maxRoll = 30.0f; // Límite máximo de inclinación lateral
    public float smoothSpeed = 5.0f; // Velocidad de suavizado

    private Vector3 previousCarPosition;

    void Start()
    {
        // Inicializa la posición anterior del coche
        previousCarPosition = carTransform.position;
    }

    void Update()
    {
        // Calcula el desplazamiento del coche desde el último frame
        Vector3 carDisplacement = carTransform.position - previousCarPosition;

        // Calcula la rotación de la cámara basada en el desplazamiento del coche
        float pitch = Mathf.Clamp(carDisplacement.z * cameraSensitivity, -maxPitch, maxPitch);
        float roll = Mathf.Clamp(carDisplacement.x * cameraSensitivity, -maxRoll, maxRoll);

        // Aplica la rotación de la cámara
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(pitch, 0, -roll), Time.deltaTime * smoothSpeed);

        // Guarda la posición actual del coche para el siguiente frame
        previousCarPosition = carTransform.position;
    }
}