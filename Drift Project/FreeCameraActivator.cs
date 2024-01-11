using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets;
using Cinemachine;

public class FreeCameraActivator : MonoBehaviour
{
    public FreeCameraMovement fcm;
    public PlayerInput playerInput;
    public CinemachineBrain cinemachineBrain;
    public GameObject player;
    public GameObject canvas;

    private bool isCameraActive = true;

    void Update()
    {
        // Verifica si la tecla 1 está siendo presionada
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Cambia el estado de los componentes y el GameObject
            ToggleComponentState(fcm);
            ToggleComponentState(cinemachineBrain);

            // Activa/desactiva el GameObject del jugador
            if (player != null && canvas != null)
            {
                player.SetActive(isCameraActive);
                canvas.SetActive(isCameraActive);
            }
        }
    }

    private void ToggleComponentState(MonoBehaviour component)
    {
        if (component != null)
        {
            isCameraActive = !component.enabled;
            component.enabled = !component.enabled;
        }
    }
}