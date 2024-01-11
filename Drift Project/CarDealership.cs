using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarDealership : MonoBehaviour
{
    public Camera mainCamera;
    public Camera dealershipCamera;
    public GameObject[] carModels;
    public GameObject[] carModelsPlayer;
    public GameObject player;
    public WheelController wheelControllerObject;
    public GameObject catalogPanel;
    public Button nextButton;
    public Button prevButton;
    public float cameraTransitionSpeed = 2.0f;

    private int currentCarIndex = 0;
    private int preselectedCarIndex = 0;
    private bool isInDealership = false;
    private bool canInteract = false;
    private bool isTransitioning = false;
    private WheelController wheelController;
    private Vector3 initialMainCameraPosition;
    private Quaternion initialMainCameraRotation;
    private float initialMainCameraFOV;
    private Vector3 initialDealershipCameraPosition;
    private Quaternion initialDealershipCameraRotation;
    private float transitionStartTime;

    private void Start()
    {
        InitializeCarModels();
        InitializeCameras();
        InitializeUIButtons();
        wheelController = wheelControllerObject.GetComponent<WheelController>();
        Cursor.visible = false;
    }

    private void Update()
    {
        if (isInDealership)
        {
            HandleDealershipInput();
        }
        else if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            EnterDealership();
        }

        if (isTransitioning)
        {
            UpdateCameraTransition();
        }
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

    private void InitializeCarModels()
    {
        foreach (var car in carModels)
        {
            car.SetActive(false);
        }
        carModels[currentCarIndex].SetActive(true);
        catalogPanel.SetActive(false);
        nextButton.interactable = false;
        prevButton.interactable = false;
    }

    private void InitializeCameras()
    {
        initialMainCameraPosition = mainCamera.transform.position;
        initialMainCameraRotation = mainCamera.transform.rotation;
        initialMainCameraFOV = mainCamera.fieldOfView;
    }

    private void InitializeUIButtons()
    {
        nextButton.onClick.AddListener(ShowNextCar);
        prevButton.onClick.AddListener(ShowPrevCar);
    }

    private void HandleDealershipInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ReturnToMainCamera();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ShowNextCar();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ShowPrevCar();
        }
    }

    private void EnterDealership()
    {
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        ShowCatalog(true);
        StartCameraTransition(mainCamera, dealershipCamera);
        EnableWheelController(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowCatalog(bool show)
    {
        catalogPanel.SetActive(show);
        nextButton.interactable = show;
        prevButton.interactable = show;
        isInDealership = show;
    }

    private void ShowNextCar()
    {
        carModels[preselectedCarIndex].SetActive(false);
        preselectedCarIndex = (preselectedCarIndex + 1) % carModels.Length;
        carModels[preselectedCarIndex].SetActive(true);
    }

    private void ShowPrevCar()
    {
        carModels[preselectedCarIndex].SetActive(false);
        preselectedCarIndex = (preselectedCarIndex - 1 + carModels.Length) % carModels.Length;
        carModels[preselectedCarIndex].SetActive(true);
    }

    private void StartCameraTransition(Camera fromCamera, Camera toCamera)
    {
        isTransitioning = true;
        transitionStartTime = Time.time;
        initialMainCameraPosition = fromCamera.transform.position;
        initialMainCameraRotation = fromCamera.transform.rotation;
        initialMainCameraFOV = fromCamera.fieldOfView;
        initialDealershipCameraPosition = toCamera.transform.position;
        initialDealershipCameraRotation = toCamera.transform.rotation;
    }

    private void UpdateCameraTransition()
    {
        float transitionProgress = (Time.time - transitionStartTime) * cameraTransitionSpeed;
        float t = Mathf.Clamp01(transitionProgress);

        mainCamera.transform.position = Vector3.Lerp(initialMainCameraPosition, initialDealershipCameraPosition, t);
        mainCamera.transform.rotation = Quaternion.Slerp(initialMainCameraRotation, initialDealershipCameraRotation, t);
        mainCamera.fieldOfView = Mathf.Lerp(initialMainCameraFOV, dealershipCamera.fieldOfView, t);

        if (t >= 1.0f)
        {
            isTransitioning = false;
        }
    }

    private void EnableWheelController(bool enable)
    {
        wheelController.enabled = enable;
    }

    private void ReturnToMainCamera()
    {
        ShowCatalog(false);
        EnableWheelController(true);
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Cursor.visible = false;

        Vector3 targetPosition = new Vector3(0, 2.19f, -6.79f);
        Quaternion targetRotation = Quaternion.Euler(7, 0, 0);
        float targetFOV = 90;

        StartCoroutine(TransitionToMainCamera(targetPosition, targetRotation, targetFOV));
    }

    private IEnumerator TransitionToMainCamera(Vector3 targetPosition, Quaternion targetRotation, float targetFOV)
    {
        float elapsedTime = 0;
        Vector3 initialPosition = mainCamera.transform.localPosition;
        Quaternion initialRotation = mainCamera.transform.localRotation;
        float initialFOV = mainCamera.fieldOfView;

        while (elapsedTime < (cameraTransitionSpeed / 2f))
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / (cameraTransitionSpeed / 2f));
            mainCamera.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
            mainCamera.transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            mainCamera.fieldOfView = Mathf.Lerp(initialFOV, targetFOV, t);
            yield return null;
        }

        mainCamera.transform.localPosition = targetPosition;
        mainCamera.transform.localRotation = targetRotation;
        mainCamera.fieldOfView = targetFOV;

        isTransitioning = false;
    }
}
