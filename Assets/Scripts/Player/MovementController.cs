using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour {
    PlayerInputActions playerInputActions;
    CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject landingParticles;

    Vector2 currentMovementInput;
    public Vector3 currentMovement;
    public Vector3 appliedMovement;
    Vector3 currentRunMovement;
    Vector2 lastDirectionInput;

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
    [SerializeField] private Collider forceCollider;
    private bool isForceModeEnabled = false;

    private Vector2 currentVectorInput;
    private Vector2 smoothInputVelocity;
    private Rigidbody rb;

    #endregion

    #region Animation Hashes

    int isWalkingHash;
    int isRunningHash;

    #endregion

    #region Gravity variables

    float gravity = -9.8f;
    float groundedGravity = -0.5f;

    [Header("Gravity")]
    [Range(1f, 10)] public float fallMultiplier = 2f;

    #endregion

    #region Jumping Variables

    [Header("Jumping")]
    [Range(0.1f, 20)] public float maxJumpHeight = 0.5f;
    [Range(0.1f, 20)] public float maxJumpTime = 1.0f;
    [SerializeField] private LayerMask environment;

    bool isJumping = false;
    int isJumpingHash;
    int isLandingHash;
    bool isJumpAnimating = false;
    bool isLandingAnimating = false;
    bool isJumpPressed = false;
    float initialJumpVelocity;

    #endregion

    #region Dodge variables

    bool isDodgePressed = false;
    bool isDodging = false;

    #endregion

    private void Awake() {
        playerInputActions = PlayerInputs.Instance.PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        forceCollider.enabled = false;

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

        if(!isForceModeEnabled) {
            characterController.Move(appliedMovement * Time.deltaTime);
            animator.SetFloat("InputMagnitude", currentVectorInput.magnitude);
        }
        

        HandleGravity();
        HandleJump();

        if(currentMovement.x != 0 || currentMovement.z != 0) {
            lastDirectionInput = new Vector2(currentMovement.x, currentMovement.z);
        }
    }

    #region Force mode

    private void EnableForceMode() {
        isForceModeEnabled = true;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        characterController.enabled = false;
        forceCollider.enabled = true;
    }

    private void DisableForceMode() {
        isForceModeEnabled = false;
        rb.isKinematic = true;
        rb.detectCollisions = false;
        characterController.enabled = true;
        forceCollider.enabled = false;
    }

    #endregion

    #region Dodge

    void HandleDodge() {
        if (lastDirectionInput.sqrMagnitude < 0.5f) {
            return;
        }

        if (characterController.isGrounded && !isDodging && !isJumping && isDodgePressed) {
            // TODO: Make unhitable for split second when dodging
            EnableForceMode();
            animator.SetTrigger("Dodge");
            isDodging = true;
            isDodgePressed = false;

            rb.AddForce(new Vector3(transform.forward.x, 0.1f, transform.forward.z) * dodgeMultiplier, ForceMode.Impulse);

            StartCoroutine(StopDodge());
        }

    }

    private IEnumerator StopDodge() {
        yield return new WaitForSeconds(0.5f);
        isDodging = false;
        DisableForceMode();
    }

    void OnDodge(InputAction.CallbackContext ctx) {
        isDodgePressed = ctx.ReadValueAsButton();
    }

    #endregion

    #region Jump

    void SetupJumpVariables() {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void OnJump(InputAction.CallbackContext ctx) {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    void HandleJump() {
        if (!isJumping && !isDodging && characterController.isGrounded && isJumpPressed) {
            isJumping = true;
            isJumpAnimating = true;
            animator.SetBool(isJumpingHash, true);

            InstantiateLandingDust();

            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;

        } else if (!isJumpPressed && isJumping && characterController.isGrounded) {
            isJumping = false;
            InstantiateLandingDust();
        }

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + characterController.height, transform.position.z), transform.TransformDirection(Vector3.up), out hit, 1f, environment)) {
            isJumpPressed = false;
        }
    }

    void InstantiateLandingDust() {
        GameObject go = Instantiate(landingParticles);
        go.SetActive(true);
        go.transform.position = new Vector3(transform.position.x, 0.49f, transform.position.z);
    }

    #endregion

    void HandleGravity() {
        bool isFalling = currentMovement.y <= 0f || !isJumpPressed;

        if (isForceModeEnabled) {
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f, environment)) {
                // Apply gravity directly
                rb.AddForce(-transform.up * 4.5f);
            }

            return;
        }

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
        if (isDodging) return;

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

    public Vector3 CurrentMovement() {
        return currentMovement;
    }

    public bool IsForceModeEnabled() {
        return isForceModeEnabled;
    }
}
