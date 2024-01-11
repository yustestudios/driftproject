using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarCustomization : MonoBehaviour
{
    public GameObject customizationObject;
    public Slider suspensionSlider;
    public Slider rotationZSlider;
    public WheelCollider[] wheelColliders;
    public CameraShop cameraShop;

    private float minSuspensionDistance = 0.1f;
    private float maxSuspensionDistance = 0.4f;
    private float maxRotationZ = 20f;
    private bool isCameraActivated = true;

    void Start()
    {
        // Realiza configuraciones iniciales aquí si es necesario
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleCustomization();
        }

        // Verifica si el objeto de personalización está activo y los sliders existen
        if (customizationObject.activeSelf && suspensionSlider != null && rotationZSlider != null)
        {
            ApplySuspension();
            ApplyRotationZ();
        }
    }

    void ToggleCustomization()
    {
        customizationObject.SetActive(!customizationObject.activeSelf);
        cameraShop.ChangeCameraShopStatus(isCameraActivated);
        isCameraActivated = !isCameraActivated;
    }

    void ApplySuspension()
    {
        // Obtiene el valor actual del slider de suspensión
        float suspensionSliderValue = suspensionSlider.value;

        // Calcula la suspensión deseada basándose en el rango definido
        float suspensionDistance = minSuspensionDistance + (maxSuspensionDistance - minSuspensionDistance) * suspensionSliderValue;

        // Aplica la suspensión a todos los Wheel Colliders
        foreach (var wheelCollider in wheelColliders)
        {
            wheelCollider.suspensionDistance = suspensionDistance;
        }
    }

    void ApplyRotationZ()
    {
        // Obtiene el valor actual del slider de rotación Z
        float rotationZSliderValue = rotationZSlider.value;

        // Calcula la rotación Z deseada basándose en el rango definido
        float rotationZ = maxRotationZ * rotationZSliderValue;

        // Aplica la rotación Z a las ruedas izquierdas y ajusta para las derechas
        foreach (var wheelCollider in wheelColliders)
        {
            // Ajusta la rotación Z para las ruedas derechas
            float adjustedRotationZ = wheelCollider.transform.localPosition.x < 0 ? rotationZ : -rotationZ;

            // Aplica la rotación Z al transform del Wheel Collider
            Vector3 localEulerAngles = wheelCollider.transform.localEulerAngles;
            localEulerAngles.z = adjustedRotationZ;
            wheelCollider.transform.localEulerAngles = -localEulerAngles;
        }
    }
}
