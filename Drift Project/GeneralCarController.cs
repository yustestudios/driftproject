using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCarController : MonoBehaviour
{
    private Quaternion originalRotation;
    private bool isFlipped = false;

    void Start()
    {
        // Almacenar la rotación original al comienzo del juego
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // Verificar si la tecla 'R' ha sido presionada y el coche está volcado
        if (Input.GetKeyDown(KeyCode.R) && isFlipped)
        {
            ResetCar();
        }

        // Verificar si el coche está volcado
        if (IsCarFlipped())
        {
            isFlipped = true;
        }
    }

    void ResetCar()
    {
        // Restablecer la rotación original
        transform.rotation = originalRotation;
        isFlipped = false;
    }

    bool IsCarFlipped()
    {
        // Definir el ángulo límite para considerar el coche volcado (-270 a 270 grados)
        float flipAngle = 270.0f;

        // Obtener el ángulo de rotación en el eje Z
        float angleZ = transform.eulerAngles.z;

        // Verificar si el ángulo de rotación está fuera del rango deseado
        if (angleZ > flipAngle || angleZ < -flipAngle)
        {
            return true;
        }

        return false;
    }
}
