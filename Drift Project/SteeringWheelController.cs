using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    public float maxSteeringAngle = 540f; // El ángulo máximo de giro del volante
    public GameObject steeringWheelObject; // Referencia al GameObject del volante
    public float rotationSpeed = 5f; // Factor de velocidad de rotación

    public WheelController wc;

    // Update is called once per frame
    void Update()
    {
        // Obtén el valor de Horizontal (-1 a 1)
        float horizontalInput = -wc.Horizontal;

        // Calcula el ángulo del volante basado en el valor de Horizontal
        float mappedWheelAngle = Mathf.Lerp(0, maxSteeringAngle * (horizontalInput < 0 ? 1 : -1), Mathf.Abs(horizontalInput));

        // Interpola suavemente la rotación actual hacia la nueva rotación
        float newRotation = Mathf.LerpAngle(steeringWheelObject.transform.localRotation.eulerAngles.z, mappedWheelAngle, Time.deltaTime * rotationSpeed);

        // Aplica la rotación al GameObject del volante
        steeringWheelObject.transform.localRotation = Quaternion.Euler(0, 0, newRotation);
   
    }
}
