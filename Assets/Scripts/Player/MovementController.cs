using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour {
    PlayerInputActions playerInputActions;
    CharacterController characterController;
    [SerializeField] private Animator animator;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;

    #region Walking and Running Variables
    bool isMovementPressed;
    bool isRunPressed;

    [Header("Movement")]
    [Range(1, 20)] public float walkMultiplier = 1.2f;
    [Range(1, 40)] public float rotationFactorPerFrame = 15;
    [Range(1, 20)] public float runMultiplier = 3f;
    [Range(1, 20)] public float dodgeMultiplier = 6f;
    [Range(0, 1)] public float smoothInputSpeed;
    [SerializeField] private bool alwaysRun = true;

    private Vector2 currentVectorInput;
    private Vector2 smoothInputVelocity;
    #endregion

    #region Animation Hashes
    int isWalkingHash;
    int isRunningHash;
    #endregion

    #region Gravity
    float gravity = -9f;
    float groundedGravity = -0.5f;

    [Header("Gravity")]
    [Range(1f, 10)] public float fallMultiplier = 2f;
    #endregion

    #region Jumping Variables
    bool isJumpPressed = false;
    float initialJumpVelocity;

    [Header("Jumping")]
    [Range(0.1f, 20)] public float maxJumpHeight = 0.5f;
    [Range(0.1f, 20)] public float maxJumpTime = 1.0f;
    [SerializeField] private LayerMask environment;
    bool isJumping = false;
    int isJumpingHash;
    int isLandingHash;
    bool isJumpAnimating = false;
    bool isLandingAnimating = false;
    #endregion

    #region Dodge
    bool isDodgePressed = false;
    bool isDodging = false;
    #endregion

    private void Awake() {
        playerInputActions = PlayerInputs.Instance.PlayerInputActions();
        characterController = GetComponent<CharacterController>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isLandingHash = Animator.StringToHash("isLanding");

        playerInputActions.CharacterControls.Move.started += OnMovementInput;
        playerInputActions.CharacterControls.Move.canceled += OnMovementInput;
        playerInputActions.CharacterControls.Move.performed += OnMovementInput;

        playerInputActions.CharacterControls.Run.started += OnRun;
        playerInputActions.CharacterControls.Run.canceled += OnRun;

        playerInputActions.CharacterControls.Jump.started += OnJump;
        playerInputActions.CharacterControls.Jump.canceled += OnJump;

        playerInputActions.CharacterControls.Dodge.started += OnDodge;
        playerInputActions.CharacterControls.Dodge.canceled += OnDodge;

        SetupJumpVariables();

        if (alwaysRun) {
            isRunPressed = true;
        }
    }

    private void Update() {
        HandleRotation();
        HandleAnimation();
        HandleDodge();

        currentVectorInput = Vector2.SmoothDamp(currentVectorInput, currentMovementInput, ref smoothInputVelocity, smoothInputSpeed);

        currentMovement.x = currentVectorInput.x * walkMultiplier;
        currentMovement.z = currentVectorInput.y * walkMultiplier;

        currentRunMovement.x = currentVectorInput.x * runMultiplier;
        currentRunMovement.z = currentVectorInput.y * runMultiplier;

        if (isDodging) {
            currentMovement.x = currentMovement.x * dodgeMultiplier;
            currentMovement.z = currentMovement.z * dodgeMultiplier;
            currentRunMovement.x = currentRunMovement.x * dodgeMultiplier;
            currentRunMovement.z = currentRunMovement.z * dodgeMultiplier;
        }

        if (isRunPressed) {
            appliedMovement.x = currentRunMovement.x;
            appliedMovement.z = currentRunMovement.z;
        } else {
            appliedMovement.x = currentMovement.x;
            appliedMovement.z = currentMovement.z;
        }

        if (isLandingAnimating) {
            currentMovement.x = 0;
            currentMovement.z = 0;
            currentRunMovement.x = 0;
            currentRunMovement.z = 0;
            appliedMovement.x = 0;
            appliedMovement.z = 0;
        }

        characterController.Move(appliedMovement * Time.deltaTime);
        animator.SetFloat("InputMagnitude", currentVectorInput.magnitude);

        HandleGravity();
        HandleJump();
    }

    void HandleDodge() {
        if (characterController.isGrounded && !isDodging && !isJumping && isDodgePressed) {
            // TODO: Shrink hit collider when dodging
            animator.SetTrigger("Dodge");
            isDodging = true;
            isDodgePressed = false;

            StartCoroutine(StopDodge());
        }

    }

    private IEnumerator StopDodge() {
        yield return new WaitForSeconds(0.5f);
        isDodging = false;
    }

    void SetupJumpVariables() {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void OnDodge(InputAction.CallbackContext ctx) {
        isDodgePressed = ctx.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext ctx) {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    void HandleJump() {
        if (!isJumping && !isDodging && characterController.isGrounded && isJumpPressed) {
            isJumping = true;
            isJumpAnimating = true;
            animator.SetBool(isJumpingHash, true);

            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + characterController.height, transform.position.z), transform.TransformDirection(Vector3.up), out hit, 1f, environment)) {
                isJumpPressed = false;
            }

        } else if (!isJumpPressed && isJumping && characterController.isGrounded) {
            isJumping = false;
        }
    }

    void HandleGravity() {
        bool isFalling = currentMovement.y <= 0f || !isJumpPressed;

        if (characterController.isGrounded) {
            if (isJumpAnimating) {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }

            if (isLandingAnimating) {
                animator.SetBool(isLandingHash, false);
                isLandingAnimating = false;
            }

            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        } else if (isFalling) {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * .5f, -20f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f, environment)) {
                if (!isLandingAnimating) {
                    animator.SetBool(isLandingHash, true);
                    isLandingAnimating = true;
                }
            }

        } else {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * .5f;
        }
    }

    void HandleAnimation() {
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

    void HandleRotation() {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if(isMovementPressed && positionToLookAt.sqrMagnitude != 0) {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void OnRun(InputAction.CallbackContext ctx) {
        if(alwaysRun) {
            isRunPressed = true;
        } else {
            isRunPressed = ctx.ReadValueAsButton();
        }
    }

    void OnMovementInput(InputAction.CallbackContext ctx) {
        currentMovementInput = ctx.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void OnEnable() {
        playerInputActions.CharacterControls.Enable();
    }

    private void OnDisable() {
        playerInputActions.CharacterControls.Disable();
    }
}
