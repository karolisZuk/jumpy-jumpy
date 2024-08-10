using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Stores the persistent state data that is passed to active states.
 * This data is used by the states, and to switch between them
 */
public class PlayerStateMachine : MonoBehaviour {
    PlayerInputActions playerInputActions;
    CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject landingParticles;

    public Vector2 currentMovementInput;
    public Vector3 currentMovement;
    public Vector3 appliedMovement;

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
    private Collider forceCollider;
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

    [Header("Gravity")]
    [Range(1f, 10f)] public float fallMultiplier = 2f;
    float gravity = 0;

    #endregion

    #region Jumping Variables

    [Header("Jumping")]
    [Range(0.1f, 20)] public float maxJumpHeight = 0.5f;
    [Range(0.1f, 20)] public float maxJumpTime = 1.0f;
    [SerializeField] private LayerMask environment;

    bool isJumping = false;
    bool isLandingAnimating = false;
    bool isJumpPressed = false;
    float initialJumpVelocity;

    #endregion

    #region Dodge variables

    bool isDodgePressed = false;
    bool isDodging = false;

    #endregion

    #region State Variables

    PlayerBaseState currentState;
    PlayerStateFactory states;

    #endregion

    #region Animation Hashes
    [HideInInspector] public int isJumpingHash = Animator.StringToHash("isJumping");
    [HideInInspector] public int isLandingHash = Animator.StringToHash("isLanding");
    [HideInInspector] public int isHardLandingHash = Animator.StringToHash("isHardLanding");
    [HideInInspector] public int isFallingHash = Animator.StringToHash("isFalling");
    #endregion

    #region Getters and Setters

    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public bool IsJumpPressed { get { return isJumpPressed; } set { isJumpPressed = value; } }
    public bool IsMovementPressed { get { return isMovementPressed; } }
    public bool IsRunPressed { get { return isRunPressed; }}
    public bool IsJumping { set { isJumping = value; } }
    public Animator Animator { get { return animator; } }
    public float InitialJumpVelocity { get { return initialJumpVelocity; } }
    public bool IsLandingAnimating { get { return isLandingAnimating; } set { isLandingAnimating = value; } }
    public CharacterController CharacterController { get { return characterController;  } }
    public bool IsForceModeEnabled { get { return isForceModeEnabled; } }
    public LayerMask Environment { get { return environment; } }
    public Rigidbody Rb { get { return rb; } }
    public float Gravity { get { return gravity; } }
    public int IsWalkingHash { get { return isWalkingHash; } }
    public int IsRunningHash { get { return isRunningHash; } }
    public Vector2 CurrentVectorInput { get { return currentVectorInput; } }

    #endregion

    private void Awake() {
        playerInputActions = PlayerInputs.Instance.PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        forceCollider = GetComponent<CapsuleCollider>();

        forceCollider.enabled = false;

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

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

        states = new PlayerStateFactory(this);
        currentState = states.Grounded();
        currentState.EnterState();
    }

    private void Start() {
        characterController.Move(appliedMovement * Time.deltaTime);
    }

    private void Update() {
        currentVectorInput = Vector2.SmoothDamp(currentVectorInput, currentMovementInput, ref smoothInputVelocity, smoothInputSpeed);

        currentState.UpdateStates();
        HandleRotation();

        if (!isForceModeEnabled) {
            characterController.Move(appliedMovement * Time.deltaTime);
            animator.SetFloat("InputMagnitude", currentVectorInput.magnitude);
        }
    }

    void SetupJumpVariables() {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void HandleRotation() {
        if (isDodging) return;

        Vector3 positionToLookAt;

        positionToLookAt.x = currentVectorInput.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentVectorInput.y;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed && positionToLookAt.sqrMagnitude != 0) {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
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

    #region Control Inputs

    void OnMovementInput(InputAction.CallbackContext ctx) {
        currentMovementInput = ctx.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void OnDodge(InputAction.CallbackContext ctx) {
        isDodgePressed = ctx.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext ctx) {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext ctx) {
        if (alwaysRun) {
            isRunPressed = true;
        } else {
            isRunPressed = ctx.ReadValueAsButton();
        }
    }

    #endregion

    public void InstantiateLandingDust() {
        GameObject go = Instantiate(landingParticles);
        go.SetActive(true);
        go.transform.position = new Vector3(transform.position.x, 0.49f, transform.position.z);
    }

    private void OnEnable() {
        playerInputActions.CharacterControls.Enable();
    }

    private void OnDisable() {
        playerInputActions.CharacterControls.Disable();
    }
}
