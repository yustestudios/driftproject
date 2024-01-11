using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para acceder a la clase Slider
using EVP;

public class InputsUIManager : MonoBehaviour
{
    public Slider throttleSlider;
    public Slider brakeSlider;

    // Referencia al script VehicleController
    public VehicleController vehicleController;

    // Start is called before the first frame update
    void Start()
    {
        // Asegúrate de tener referencias válidas antes de comenzar
        if (throttleSlider == null || brakeSlider == null || vehicleController == null)
        {
            Debug.LogError("Asigna las referencias de los sliders y VehicleController en el Inspector.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Actualiza los valores de los sliders con los valores de VehicleController
        UpdateSliders();
    }

    void UpdateSliders()
    {
        // Asegúrate de tener referencias válidas antes de actualizar
        if (throttleSlider == null || brakeSlider == null || vehicleController == null)
        {
            return;
        }

        // Asigna los valores de los sliders con los valores de VehicleController
        throttleSlider.value = vehicleController.throttleInput;
        brakeSlider.value = vehicleController.brakeInput;
    }
}
