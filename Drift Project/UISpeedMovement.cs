using UnityEngine;

public class UISpeedMovement : MonoBehaviour
{
    public SpeedCalculator speedCalculator;
    public RectTransform rectTransform;

    // Intensidad máxima del efecto
    public float maxEffectIntensity = 10f;

    // Factor de escala para ajustar la velocidad del ruido
    public float noiseScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if (rectTransform == null)
        {
            // Obtener el RectTransform del objeto actual si no se ha asignado
            rectTransform = GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (speedCalculator != null && rectTransform != null)
        {
            // Obtener la velocidad del speedCalculator
            float speed = speedCalculator.Speed;

            // Calcular la posición X e Y con ruido en función de la velocidad
            float noiseX = CalculateNoise(speed, 0);
            float noiseY = CalculateNoise(speed, 1);

            // Aplicar la nueva posición al RectTransform
            rectTransform.anchoredPosition = new Vector2(noiseX, noiseY) * maxEffectIntensity;
        }
    }

    // Función para calcular el ruido en función de la velocidad
    float CalculateNoise(float speed, int axis)
    {
        // Ajusta el factor de escala según tus necesidades
        float scaledSpeed = speed * noiseScale;

        // Utiliza Mathf.PerlinNoise para generar valores suaves y pseudoaleatorios
        return Mathf.PerlinNoise(Time.time * scaledSpeed, axis);
    }
}
