using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EVP;

public class EnterRaceTrack : MonoBehaviour
{
    public EventManager eventManager;
    public GameObject player;
    public GameObject initialMap;
    public GameObject raceMap;
    public Transform playerInitialPosition;
    public Text interactionText;
    public Text countdownText;
    public Text driftPoints;
    public GameObject labelDrift;
    public float countdownDuration = 3.0f;
    public float raceDuration = 120.0f;
    public MoneyManager moneyManager;
    public bool cancelRace = false;
    public DriftController driftController;
    private static int triggerCount = 0;
    private bool inTrigger = false;
    public bool raceInProgress = false;
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;
    private Rigidbody playerRigidbody;
    private float totalDriftPoints = 0f;

    private void Start()
    {
        interactionText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        driftPoints.gameObject.SetActive(false);
        labelDrift.gameObject.SetActive(false);
        initialMap.SetActive(true);
        raceMap.SetActive(false);
        initialPlayerPosition = player.transform.position;
        initialPlayerRotation = player.transform.rotation;
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (triggerCount == 1)
        {
            interactionText.text = "Press E to enter the track";
            interactionText.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E) && !raceInProgress)
            {
                StartCoroutine(StartRace());
            }
        }
        else
        {
            if (!raceInProgress)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerCount++;
            inTrigger = true;
            eventManager.EnteredTriggerEventManager(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerCount--;

            // Asegurar que el contador no sea negativo
            triggerCount = Mathf.Max(triggerCount, 0);

            inTrigger = false;
            if (!raceInProgress)
            {
                eventManager.Restart();
            }
        }
    }

    public void StartRaceFromExternal()
    {
        StartCoroutine(StartRace());
    }

    private IEnumerator StartRace()
    {
        raceInProgress = true;
        driftController.enterRacingTrack = this;

        interactionText.gameObject.SetActive(true);

        // Detener el movimiento del coche
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        playerRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        playerRigidbody.interpolation = RigidbodyInterpolation.None;

        // Desactivar el mapa actual y activar el mapa de carreras
        initialMap.SetActive(false);
        raceMap.SetActive(true);

        driftPoints.gameObject.SetActive(true);
        labelDrift.gameObject.SetActive(true);

        // Transportar al jugador al punto inicial
        player.transform.position = playerInitialPosition.position;
        player.transform.rotation = playerInitialPosition.rotation;

        totalDriftPoints = 0;
        driftPoints.text = totalDriftPoints.ToString();

        for (int i = 3; i > 0; i--)
        {
            interactionText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }

        interactionText.text = "GO";
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        playerRigidbody.constraints = RigidbodyConstraints.None;

        yield return new WaitForSeconds(1.0f);

        interactionText.gameObject.SetActive(false);

        // Mostrar el contador de tiempo
        countdownText.gameObject.SetActive(true);

        float timer = raceDuration;
        while (timer > 0 && !cancelRace)
        {
            countdownText.text = timer.ToString("F0");
            yield return new WaitForSeconds(1.0f);
            timer -= 1.0f;
        }

        // Cuando acabe el tiempo, restablecer todo
        raceInProgress = false;
        countdownText.text = "Race Over";

        moneyManager.AddMoney(Mathf.FloorToInt(totalDriftPoints / 10f));

        yield return new WaitForSeconds(2.0f);
        countdownText.gameObject.SetActive(false);
        interactionText.gameObject.SetActive(false);
        labelDrift.gameObject.SetActive(false);

        totalDriftPoints = 0f;
        driftPoints.text = totalDriftPoints.ToString();
        driftPoints.gameObject.SetActive(false);

        // Detener el coche y devolver al jugador a su posición inicial
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
        playerRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        // Desactivar el mapa de carreras y activar el mapa inicial
        raceMap.SetActive(false);
        initialMap.SetActive(true);

        // Devolver al jugador a su posición inicial sin velocidad
        player.transform.position = initialPlayerPosition;
        player.transform.rotation = initialPlayerRotation;
        playerRigidbody.constraints = RigidbodyConstraints.None;

        eventManager.Restart();

        yield return new WaitForSeconds(1.0f);
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void AddDriftPoints(float points)
    {
        totalDriftPoints += points;
        driftPoints.text = totalDriftPoints.ToString();
    }
}
