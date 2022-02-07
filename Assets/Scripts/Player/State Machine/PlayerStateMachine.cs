using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [HideInInspector] public PlayerBaseState currentState;
    PlayerStateFactory states;

    #region Walking and Running Variables
    [HideInInspector] public Vector2 currentMovementInput;
    [HideInInspector] public Vector3 currentMovement;
    [HideInInspector] public Vector3 currentRunMovement;
    [HideInInspector] public Vector3 appliedMovement;

    [HideInInspector] public bool isMovementPressed;
    [HideInInspector] public bool isRunPressed;
    [Header("Movement")]
    [Range(1, 20)] public float walkMultiplier = 1.2f;
    [Range(1, 40)] public float rotationFactorPerFrame = 15;
    [Range(1, 20)] public float runMultiplier = 3f;
    #endregion

    #region Animation Hashes
    [HideInInspector] public int isWalkingHash;
    [HideInInspector] public int isRunningHash;
    #endregion

    #region Gravity
    [HideInInspector] public float gravity = -9f;
    [HideInInspector] public float groundedGravity = -0.5f;
    [Header("Gravity")]
    [Range(1f, 10)] public float fallMultiplier = 2f;
    #endregion

    #region Jumping Variables
    [HideInInspector] public bool isJumpPressed = false;
    [HideInInspector] public float initialJumpVelocity;
    [Header("Jumping")]
    [Range(0.1f, 20)] public float maxJumpHeight = 0.5f;
    [Range(0.1f, 20)] public float maxJumpTime = 1.0f;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public int isJumpingHash;
    [HideInInspector] public bool requireNewJumpPress;
    #endregion

    private void Awake()
    {
        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();

        #region Set Member Variable
        playerInputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        #endregion

        #region Set Animator Hashes
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        #endregion

        #region Read Inputs
        playerInputActions.CharacterControls.Move.started += OnMovementInput;
        playerInputActions.CharacterControls.Move.canceled += OnMovementInput;
        playerInputActions.CharacterControls.Move.performed += OnMovementInput;

        playerInputActions.CharacterControls.Run.started += OnRun;
        playerInputActions.CharacterControls.Run.canceled += OnRun;

        playerInputActions.CharacterControls.Jump.started += OnJump;
        playerInputActions.CharacterControls.Jump.canceled += OnJump;
        #endregion

        SetupJumpVariables();
    }

    private void Update()
    {
        HandleRotation();
        characterController.Move(appliedMovement * Time.deltaTime);
        currentState.UpdateStates();
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * walkMultiplier;
        currentMovement.z = currentMovementInput.y * walkMultiplier;

        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        isJumpPressed = ctx.ReadValueAsButton();
        requireNewJumpPress = false;
    }

    void OnRun(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
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
