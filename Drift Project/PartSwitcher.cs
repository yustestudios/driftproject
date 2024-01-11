using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PartSwitcher : MonoBehaviour
{
    public CameraShop cameraShop; // Referencia al script CameraShop
    public Button leftButton; // Botón de izquierda
    public Button rightButton; // Botón de derecha

    public GameObject[] frontBumpers; // Array de FrontBumpers
    public GameObject[] rearBumpers; // Array de RearBumpers
    public GameObject[] spoilers; // Array de Spoilers
    public GameObject[] exhausts; // Array de Exhausts

    private GameObject[] currentPartGroup;
    private int currentIndex;

    void Start()
    {
        // Inicializar los grupos de piezas y los índices
        currentPartGroup = frontBumpers; // Inicia con FrontBumpers
        currentIndex = 0;

        // Activar la pieza actual del grupo actual
        ActivateParts(currentPartGroup, currentIndex);

        // Asignar funciones a los botones
        leftButton.onClick.AddListener(OnLeftButtonClicked);
        rightButton.onClick.AddListener(OnRightButtonClicked);
    }

    void OnLeftButtonClicked()
    {
        SwitchPart(-1); // Pasar a la pieza anterior en el grupo actual
    }

    void OnRightButtonClicked()
    {
        SwitchPart(1); // Pasar a la siguiente pieza en el grupo actual
    }

    void SwitchPart(int direction)
    {
        // Guardar la configuración actual
        SaveCurrentConfiguration();

        // Cambiar el grupo de piezas según el índice actual de CameraShop
        int currentFocusIndex = cameraShop.currentFocusIndex;
        switch (currentFocusIndex)
        {
            case 0:
                currentPartGroup = rearBumpers;
                break;
            case 1:
                currentPartGroup = frontBumpers;
                break;
            case 2:
                currentPartGroup = spoilers;
                break;
            case 3:
                currentPartGroup = exhausts;
                break;
            default:
                Debug.LogError("Unknown focus index: " + currentFocusIndex);
                break;
        }

        // Calcular el nuevo índice
        currentIndex = (currentIndex + direction + currentPartGroup.Length) % currentPartGroup.Length;

        // Activar la pieza actual del grupo actual
        ActivateParts(currentPartGroup, currentIndex);

        // Actualizar las piezas del coche
        UpdateCarParts();
    }

    void SaveCurrentConfiguration()
    {
        // No desactivar ninguna pieza al cambiar de parte, ya que queremos mantener la selección anterior
    }

    void UpdateCarParts()
    {
        // Implementar lógica de actualización si es necesario
        // ...
    }

    void ActivateParts(GameObject[] parts, int index)
    {
        // Desactivar todas las piezas del grupo actual
        DeactivateAllParts(parts);

        // Activar la pieza actual del grupo actual
        if (parts.Length > 0 && index >= 0 && index < parts.Length)
        {
            parts[index].SetActive(true);
        }
    }

    void DeactivateAllParts(GameObject[] parts)
    {
        foreach (var part in parts)
        {
            part.SetActive(false);
        }
    }
}
