using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class FreeCameraMovement : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		
		public bool verticalps5;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        [Header("Camera Movement Settings")]
        public float moveSpeed = 5f;
        public float sensitivity = 2f;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnVerticalPS5(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}


		public void VerticalPS5Input(bool newSprintState)
		{
			verticalps5 = newSprintState;
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

        private void Update()
        {
            MoveCamera();
            RotateCamera();
        }

        private void MoveCamera()
        {
            Vector3 moveDirection = new Vector3(move.x, 0f, move.y);

            // If using analog movement, consider vertical input as forward/backward movement
            if (analogMovement)
            {
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection.y = 0f;
            }

            // Multiplica por la intensidad del joystick para que el movimiento sea proporcional a la posición del joystick
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime * Mathf.Clamp01(moveDirection.magnitude), Space.World);
        }


        private void RotateCamera()
        {
            if (cursorInputForLook)
            {
                Vector2 mouseDelta = -look * sensitivity;

                // Rotate the camera based on the mouse input
                transform.Rotate(Vector3.up * mouseDelta.x);
                transform.Rotate(Vector3.left * mouseDelta.y);
            }
        }
	}
	
}