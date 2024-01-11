using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<GameObject> cameraObjects; // Lista que contiene todos los objetos de cámara que deseas alternar
    private int currentCameraIndex = 0; // Índice del objeto de cámara actual

    void Start()
    {
        InitializeCameras();
    }

    void Update()
    {
        SwitchCameraOnInput();
    }

    private void InitializeCameras()
    {
        // Asegúrate de que al menos haya un objeto de cámara y que estén desactivados al inicio
        foreach (var cameraObject in cameraObjects)
        {
            cameraObject.SetActive(false);
        }

        // Activa el primer objeto de cámara
        ActivateCurrentCamera();
    }

    private void SwitchCameraOnInput()
    {
        // Cambia el objeto de cámara al presionar la tecla "C" o hacer clic en el círculo del mando de PS4
        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("joystick button 2"))
        {
            DeactivateCurrentCamera();
            IncrementCameraIndex();
            ActivateCurrentCamera();
        }
    }

    private void DeactivateCurrentCamera()
    {
        // Desactiva el objeto de cámara actual
        cameraObjects[currentCameraIndex].SetActive(false);
    }

    private void IncrementCameraIndex()
    {
        // Incrementa el índice del objeto de cámara actual o reinícialo si llega al final de la lista
        currentCameraIndex = (currentCameraIndex + 1) % cameraObjects.Count;
    }

    private void ActivateCurrentCamera()
    {
        // Activa el nuevo objeto de cámara
        cameraObjects[currentCameraIndex].SetActive(true);
    }
}
