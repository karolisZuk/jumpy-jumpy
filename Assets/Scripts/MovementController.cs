using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    CharacterController characterController;
    Animator animator;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;

    #region Walking and Running Variables
    bool isMovementPressed;
    bool isRunPressed;
    [Header("Movement")]
    [Range(0, 40)] public float rotationFactorPerFrame = 15;
    [Range(0, 20)] public float runMultiplier = 3f;
    #endregion

    #region Animation Hashes
    int isWalkingHash;
    int isRunningHash;
    #endregion

    #region Gravity
    float gravity = -9f;
    float groundedGravity = -0.5f;
    #endregion

    #region Jumping Variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    [Header("Jumping")]
    [Range(0.1f, 20)] public float maxJumpHeight = 0.5f;
    [Range(0.1f, 20)] public float maxJumpTime = 1.0f;
    bool isJumping = false;
    #endregion

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        playerInputActions.CharacterControls.Move.started += OnMovementInput;
        playerInputActions.CharacterControls.Move.canceled += OnMovementInput;
        playerInputActions.CharacterControls.Move.performed += OnMovementInput;

        playerInputActions.CharacterControls.Run.started += OnRun;
        playerInputActions.CharacterControls.Run.canceled += OnRun;

        playerInputActions.CharacterControls.Jump.started += OnJump;
        playerInputActions.CharacterControls.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    void HandleJump()
    {
        if(!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            currentRunMovement.y = initialJumpVelocity;
        } else if(!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    void HandleGravity()
    {
        if(characterController.isGrounded)
        {
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        } else
        {
            currentMovement.y += gravity * Time.deltaTime;
            currentRunMovement.y += gravity * Time.deltaTime;
        }
    }

    void HandleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if(isMovementPressed && !isWalking)
            animator.SetBool(isWalkingHash, true);
        else if (!isMovementPressed && isWalking)
            animator.SetBool(isWalkingHash, false);

        if((isMovementPressed && isRunPressed) && !isRunning)
            animator.SetBool(isRunningHash, true);
        else if ((!isMovementPressed || !isRunPressed) && isRunning)
            animator.SetBool(isRunningHash, false);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if(isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void OnRun(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }

    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void Update()
    {
        HandleRotation();
        HandleAnimation();

        if (isRunPressed)
            characterController.Move(currentRunMovement * Time.deltaTime);
        else
            characterController.Move(currentMovement * Time.deltaTime);

        HandleGravity();
        HandleJump();
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
