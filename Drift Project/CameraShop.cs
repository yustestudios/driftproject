using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using EVP;

public class CameraShop : MonoBehaviour
{
    public CameraModeConfig cameraModeConfig;
    private VehicleCameraController vehicleCameraController;
    public Transform carTransform;
    public Transform shopCameraTransform;
    public Camera camera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private float orbitX = 0f;
    private float orbitY = 20f;
    private float orbitDistance = 8f;
    private bool isRotating = false;
    private bool isDragging = false;
    private float rotationSpeed = 2.0f;
    private float autoRotationSpeed = 4.0f;
    private float dragSpeed = 2.0f;

    private Coroutine autoRotateCoroutine;

    void Start()
    {
        vehicleCameraController = GetComponent<VehicleCameraController>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        autoRotateCoroutine = StartCoroutine(AutoRotate());
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (isDragging)
            {
                HandleDrag();
            }
            else
            {
                RotateAroundCar();
            }

            HandlePieceSwitching();
        }
    }

    void LateUpdate()
    {
        HandleMouseInput();
    }

    public void ChangeCameraShopStatus(bool isActive)
    {
        if (camera != null)
        {
            if (isActive)
            {
                SetToShopCameraTransform();
                FocusOnCurrentPiece();
            }
            else
            {
                ResetToOriginalTransform();
                StopAutoRotate();
            }
        }

        if (vehicleCameraController != null && cameraModeConfig != null)
        {
            vehicleCameraController.enabled = !isActive;
            cameraModeConfig.enabled = !isActive;
            camera.fieldOfView = 40f;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartRotating();
        }
        else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            StopRotating();
        }

        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            StartDragging();
        }
        else if (Input.GetMouseButtonUp(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            StopDragging();
        }
    }

    private void HandlePieceSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToNextPiece();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToPreviousPiece();
        }
    }

    private void StartRotating()
    {
        isRotating = true;
        StopAutoRotate();
    }

    private void StopRotating()
    {
        isRotating = false;
        StartAutoRotate();
    }

    private void StartDragging()
    {
        isDragging = true;
    }

    private void StopDragging()
    {
        isDragging = false;
    }

    private void HandleDrag()
    {
        float mouseX = Input.GetAxis("Mouse X") * dragSpeed;
        orbitX += mouseX;
        orbitY -= Input.GetAxis("Mouse Y") * dragSpeed;
        orbitY = Mathf.Clamp(orbitY, -80f, 80f);

        Quaternion rotation = Quaternion.Euler(orbitY, orbitX, 0);
        Vector3 desiredPosition = carTransform.position - rotation * Vector3.forward * orbitDistance;

        desiredPosition.y += 0.4f;
        transform.position = desiredPosition;
        transform.rotation = rotation;
    }

    private void RotateAroundCar()
    {
        if (isRotating && !isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            orbitX += mouseX;

            Quaternion rotation = Quaternion.Euler(orbitY, orbitX, 0);
            Vector3 desiredPosition = carTransform.position - rotation * Vector3.forward * orbitDistance;

            desiredPosition.y += 0.4f;
            transform.position = desiredPosition;
            transform.rotation = rotation;
        }
    }

    private IEnumerator AutoRotate()
    {
        while (true)
        {
            if (!isRotating && !isDragging)
                yield return null;

            orbitX += autoRotationSpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.Euler(orbitY, orbitX, 0);
            Vector3 desiredPosition = carTransform.position - rotation * Vector3.forward * orbitDistance;

            desiredPosition.y += 0.4f;
            transform.position = desiredPosition;
            transform.rotation = rotation;

            yield return null;
        }
    }

    private void StopAutoRotate()
    {
        if (autoRotateCoroutine != null)
            StopCoroutine(autoRotateCoroutine);
    }

    private void StartAutoRotate()
    {
        StopAutoRotate();
        autoRotateCoroutine = StartCoroutine(AutoRotate());
    }

    private void SetToShopCameraTransform()
    {
        transform.position = shopCameraTransform.position;
        transform.rotation = shopCameraTransform.rotation;
    }

    private void ResetToOriginalTransform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    private void FocusOnCurrentPiece()
    {
        if (focusTransforms.Count > 0 && currentFocusIndex >= 0 && currentFocusIndex < focusTransforms.Count)
        {
            Transform targetTransform = focusTransforms[currentFocusIndex];
            transform.position = targetTransform.position;
            transform.LookAt(carTransform);
        }
    }

    private void SwitchToNextPiece()
    {
        StopAutoRotate();
        currentFocusIndex = (currentFocusIndex + 1) % focusTransforms.Count;
        FocusOnCurrentPiece();
    }

    private void SwitchToPreviousPiece()
    {
        StopAutoRotate();
        currentFocusIndex = (currentFocusIndex - 1 + focusTransforms.Count) % focusTransforms.Count;
        FocusOnCurrentPiece();
    }
}
