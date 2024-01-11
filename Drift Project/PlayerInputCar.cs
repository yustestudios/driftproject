using UnityEngine;
using UnityEngine.InputSystem;

namespace EVP
{
    public class PlayerInputCar : MonoBehaviour
    {
        private InputAction horizontalPS4;
        private float horizontal;
        private InputAction verticalPS4;
        private float vertical;
        private InputAction brakePS4;
        private float brake;
        public SpeedCalculator sc;
        public bool continuousForwardAndReverse = true;
        public KeyCode resetVehicleKey = KeyCode.Return;
        public string handbrakeAction = "joystick button 5";
        public string tractionControlAction = "joystick button 20";
        public VehicleController target;
        public bool steeringWheel = false;
        public WheelSkid[] wheelSkids;
        private LogitechGSDK.DIJOYSTATE2ENGINES rec;
        private bool driftingActivated;
        private bool activatedLeft;
        private bool activatedRight;
        private int driftDirection;

        private int currentForce;
        private bool wasButtonPressed = false;
        private int targetForce;
        private float forceLerpSpeed;

        void Start()
        {
            // Configurar acciones de entrada
            horizontalPS4 = new InputAction("HorizontalPS4", binding: "<Gamepad>/leftStick/x");
            verticalPS4 = new InputAction("VerticalPS4", binding: "<Gamepad>/rightTrigger");
            brakePS4 = new InputAction("BrakePS4", binding: "<Gamepad>/leftTrigger");

            activatedLeft = false;
            activatedRight = false;

            driftingActivated = false;
            driftDirection = 0;

            currentForce = 0;
            targetForce = 0;
            forceLerpSpeed = 5f;

            // Añadir escuchadores a las acciones
            horizontalPS4.performed += OnHorizontalPS4;
            horizontalPS4.canceled += OnHorizontalPS4Canceled;
            verticalPS4.performed += OnVerticalPS4;
            verticalPS4.canceled += OnVerticalPS4Canceled;
            brakePS4.performed += OnBrakePS4;
            brakePS4.canceled += OnBrakePS4Canceled;

            // Habilitar las acciones
            horizontalPS4.Enable();
            verticalPS4.Enable();
            brakePS4.Enable();

            if (steeringWheel && LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                rec = LogitechGSDK.LogiGetStateUnity(0);
                LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
                LogitechGSDK.LogiGetCurrentControllerProperties(0, ref actualProperties);
                actualProperties.wheelRange = 270;
                actualProperties.damperGain = -5;
                LogitechGSDK.LogiSetPreferredControllerProperties(actualProperties);

                LogitechGSDK.DIJOYSTATE2ENGINES rec2 = new LogitechGSDK.DIJOYSTATE2ENGINES();
                rec2.lFX = 10;
            }
        }

        void Update()
        {
            if (steeringWheel && LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                rec = LogitechGSDK.LogiGetStateUnity(0);
                horizontal = rec.lX / 32760.0f;
                vertical = rec.lY / -32760.0f;
                brake = rec.lRz / -32760.0f;

                float bumperMultiplier = 1f;

                float forceSpeed = 1f;
                if (wheelSkids[0].isDrifting && wheelSkids[1].isDrifting && target.speed > 20f)
                {
                    forceSpeed = 10f;
                    bumperMultiplier = 10f;
                }
                else
                {
                    forceSpeed = 1f;
                    bumperMultiplier = 1f;
                }

                LogitechGSDK.LogiPlayDamperForce(0, 25);

                if (wheelSkids[0].isDrifting && wheelSkids[1].isDrifting && sc.Speed > 20f && !driftingActivated)
                {
                    driftingActivated = true;

                    if (horizontal > 0.01f)
                    {
                        driftDirection = 1;
                    }
                    else if (horizontal < -0.01f)
                    {
                        driftDirection = -1;
                    }

                    targetForce = Mathf.RoundToInt(sc.Speed / forceSpeed * 60f * driftDirection * Mathf.Abs(horizontal));
                    LogitechGSDK.LogiPlayConstantForce(0, targetForce);
                }
                else if (driftingActivated)
                {
                    if (!wheelSkids[0].isDrifting || !wheelSkids[1].isDrifting)
                    {
                        driftingActivated = false;
                        targetForce = 0; // Restablecer la fuerza objetivo a 0 al dejar de derrapar
                    }

                    // Gradualmente transiciona hacia 0
                    currentForce = Mathf.RoundToInt(Mathf.Lerp(currentForce, targetForce, Time.deltaTime * forceLerpSpeed));
                    LogitechGSDK.LogiPlayConstantForce(0, currentForce);
                }
                else
                {
                    driftingActivated = false;
                    driftDirection = 0;

                    // Gradualmente transiciona hacia 0
                    int forceW = int.Parse(Mathf.RoundToInt((sc.Speed * horizontal * 20f)).ToString());
                    currentForce = Mathf.RoundToInt(Mathf.Lerp(currentForce, forceW, Time.deltaTime * forceLerpSpeed));
                    LogitechGSDK.LogiPlayConstantForce(0, currentForce);

                    if (Mathf.Abs(currentForce) < 1)
                    {
                        currentForce = 0;
                        LogitechGSDK.LogiStopConstantForce(0);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (target == null) return;

            float steerInput = Mathf.Clamp(horizontal, -1.0f, 1.0f);

            float handbrakeInput;

            if (Input.GetButton(handbrakeAction))
            {
                handbrakeInput = 1.0f;
            }
            else
            {
                handbrakeInput = 0.0f;
            }

            if (steeringWheel)
            {
                if (rec.rgbButtons[12] == 128)
                {
                    handbrakeInput = 1.0f;
                    sc.SetHandbrakeState(true);
                }
                else
                {
                    handbrakeInput = 0.0f;
                    sc.SetHandbrakeState(false);
                }
            }

            float forwardInput = Mathf.Clamp01(vertical);
            float reverseInput = Mathf.Clamp01(brake);

            float throttleInput = 0.0f;
            float brakeInput = 0.0f;

            if (continuousForwardAndReverse)
            {
                float minSpeed = 0.1f;
                float minInput = 0.1f;

                if (target.speed > minSpeed)
                {
                    throttleInput = forwardInput;
                    brakeInput = reverseInput;
                }
                else
                {
                    if (reverseInput > minInput)
                    {
                        throttleInput = -reverseInput;
                        brakeInput = 0.0f;
                    }
                    else if (forwardInput > minInput)
                    {
                        if (target.speed < -minSpeed)
                        {
                            throttleInput = 0.0f;
                            brakeInput = forwardInput;
                        }
                        else
                        {
                            throttleInput = forwardInput;
                            brakeInput = 0;
                        }
                    }
                }
            }
            else
            {
                bool reverse = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

                if (!reverse)
                {
                    throttleInput = forwardInput;
                    brakeInput = reverseInput;
                }
                else
                {
                    throttleInput = -reverseInput * 10;
                    brakeInput = 0;
                }
            }

            if (steeringWheel)
            {
                if (rec.rgbButtons[20] == 128 && !wasButtonPressed)
                {
                    target.tractionControl = !target.tractionControl;
                    sc.traction = target.tractionControl;

                    wasButtonPressed = true;
                }
                else if (rec.rgbButtons[20] == 0)
                {
                    wasButtonPressed = false;
                }
            }

            target.steerInput = steerInput;
            target.throttleInput = throttleInput;
            target.brakeInput = brakeInput;
            target.handbrakeInput = handbrakeInput;
        }

        // Callbacks para la acción HorizontalPS4
        private void OnHorizontalPS4(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            if (!steeringWheel)
            {
                horizontal = value;
            }
        }

        private void OnHorizontalPS4Canceled(InputAction.CallbackContext context)
        {
            if (!steeringWheel)
            {
                horizontal = 0.0f;
            }
        }

        // Callbacks para la acción VerticalPS4
        private void OnVerticalPS4(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            if (!steeringWheel)
            {
                vertical = value;
            }
        }

        private void OnVerticalPS4Canceled(InputAction.CallbackContext context)
        {
            if (!steeringWheel)
            {
                vertical = 0.0f;
            }
        }

        // Callbacks para la acción BrakePS4
        private void OnBrakePS4(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            if (!steeringWheel)
            {
                brake = value;
            }
        }

        private void OnBrakePS4Canceled(InputAction.CallbackContext context)
        {
            if (!steeringWheel)
            {
                brake = 0.0f;
            }
        }

        // Limpiar acciones al destruir el objeto
        private void OnDestroy()
        {
            horizontalPS4.Disable();
            verticalPS4.Disable();
            brakePS4.Disable();
        }
    }
}
