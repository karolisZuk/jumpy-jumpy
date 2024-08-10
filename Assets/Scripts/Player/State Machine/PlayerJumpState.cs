using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState {
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext, playerStateFactory) {
        IsRootState = true;
    }

    public override void CheckSwitchStates() {
        if (Ctx.CharacterController.isGrounded) {
            SwitchState(Factory.Grounded());
        }
    }

    public override void EnterState() {
        InitializeSubState();
        HandleJump();
    }

    public override void ExitState() {
        Ctx.InstantiateLandingDust();
        Ctx.Animator.SetBool(Ctx.isJumpingHash, false);
        Ctx.Animator.SetBool(Ctx.isLandingHash, false);
        Ctx.IsLandingAnimating = false;

        if (Ctx.IsJumpPressed) {
            Ctx.IsJumpPressed = false;
        }
    }

    public override void InitializeSubState() {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed) {
            SetSubState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed) {
            SetSubState(Factory.Walk());
        } else {
            SetSubState(Factory.Run());
        }
    }

    public override void UpdateState() {
        HandleGravity();
        CheckSwitchStates();
    }

    void HandleJump() {
        Ctx.IsJumping = true;
        Ctx.Animator.SetBool(Ctx.isJumpingHash, true);

        Ctx.currentMovement.y = Ctx.InitialJumpVelocity;
        Ctx.appliedMovement.y = Ctx.InitialJumpVelocity;
    }

    public void HandleGravity() {
        bool isFalling = Ctx.currentMovement.y <= 0f || !Ctx.IsJumpPressed;

        // TODO: Move to its own state
        if (Ctx.IsForceModeEnabled) {
            RaycastHit hit;
            if (!Physics.Raycast(Ctx.transform.position, Ctx.transform.TransformDirection(Vector3.down), out hit, 1f, Ctx.Environment)) {
                Ctx.Rb.AddForce(-Ctx.transform.up * 4.5f);
            }
            return;
        }

        if (isFalling) {
            float previousYVelocity = Ctx.currentMovement.y;
            Ctx.currentMovement.y = Ctx.currentMovement.y + (Ctx.Gravity * Ctx.fallMultiplier * Time.deltaTime);
            Ctx.appliedMovement.y = Mathf.Max((previousYVelocity + Ctx.currentMovement.y) * .5f, -20f);

            RaycastHit hit;
            if (Physics.Raycast(Ctx.transform.position, Ctx.transform.TransformDirection(Vector3.down), out hit, 1f, Ctx.Environment)) {
                if (!Ctx.IsLandingAnimating) {
                    Ctx.Animator.SetBool(Ctx.isLandingHash, true);
                    Ctx.IsLandingAnimating = true;
                }
            }

        } else {
            float previousYVelocity = Ctx.currentMovement.y;
            Ctx.currentMovement.y = Ctx.currentMovement.y + (Ctx.Gravity * Time.deltaTime);
            Ctx.appliedMovement.y = (previousYVelocity + Ctx.currentMovement.y) * .5f;
        }
    }
}