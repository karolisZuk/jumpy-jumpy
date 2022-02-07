using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    CharacterController characterController;

    Vector2 currentMovementInput;
    bool isMovementPressed;
    bool isRunPressed;
    bool isJumpPressed;

    [SerializeField] private bool alwaysRun;

    [HideInInspector] public Vector3 appliedMovement;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();

        playerInputActions.CharacterControls.Move.started += OnMovementInput;
        playerInputActions.CharacterControls.Move.canceled += OnMovementInput;
        playerInputActions.CharacterControls.Move.performed += OnMovementInput;

        playerInputActions.CharacterControls.Run.started += OnRun;
        playerInputActions.CharacterControls.Run.canceled += OnRun;

        playerInputActions.CharacterControls.Jump.started += OnJump;
        playerInputActions.CharacterControls.Jump.canceled += OnJump;
    }

    private void Update()
    {
        characterController.Move(appliedMovement * Time.deltaTime);
    }

    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void OnRun(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    public Vector2 CurrentMovementInput()
    {
        return currentMovementInput;
    }

    public bool IsMovementPressed()
    {
        return isMovementPressed;
    }

    public bool IsRunPressed()
    {
        return isRunPressed || alwaysRun;
    }

    public bool IsJumpPressed()
    {
        return isJumpPressed;
    }

    private void OnEnable()
    {
        playerInputActions.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.CharacterControls.Disable();
    }
}
