using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExitCarController : MonoBehaviour
{
    public GameObject player;
    public WheelController wheelController;
    public Rigidbody carRigidbody;
    public GameObject carCamera;
    public Transform spawnPoint;
    public float maxCarSpeedForExit = 50.0f;
    private bool canInteract = false;
    private bool inside = false;

    void Start()
    {
        inside = false;
        carCamera.SetActive(false);
        wheelController.enabled = false;
    }

    private void Update()
    {
        if (canInteract && !inside && (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("joystick button 3")))
        {
            EnterCar();
        }
        else if (canInteract && inside && (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("joystick button 3")))
        {
            ExitCar();
        }
    }

    private void EnterCar()
    {
        inside = true;
        carCamera.SetActive(true);
        wheelController.enabled = true;
        player.SetActive(false);
        carRigidbody.constraints = RigidbodyConstraints.None;
    }

    private void ExitCar()
    {
        inside = false;
        carCamera.SetActive(false);
        wheelController.Horizontal = 0;
        wheelController.Vertical = 0;
        wheelController.enabled = false;

        // Si la velocidad del coche es menor que la velocidad máxima, detenerlo
        if (carRigidbody.velocity.magnitude * 3.6f <= maxCarSpeedForExit)
        {
            carRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }

        player.transform.position = spawnPoint.position;
        player.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}
