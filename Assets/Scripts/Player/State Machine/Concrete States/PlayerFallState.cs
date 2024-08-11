using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState {
    bool splatTimerStarted;
    float splatTime = 1.2f;
    float timer;
    float fallTimer;

    float splatLimitTime = .3f;

    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        IsRootState = true;
    }

    public override void EnterState() {
        splatTimerStarted = false;
        Ctx.IsLandingAnimating = false;
        timer = 0;
        fallTimer = 0;
        InitializeSubState();
    }

    public override void UpdateState() {
        HandleGravity();

        fallTimer += Time.deltaTime;

        Ctx.CurrentVectorInputX = 0;
        Ctx.CurrentVectorInputY = 0;

        if (Ctx.appliedMovement.y < Ctx.Gravity * 1.1f && !Ctx.Animator.GetBool(Ctx.isFallingHash)) {
            Ctx.Animator.SetBool(Ctx.isFallingHash, true);
        }

        // Only splat if been falling for awhile
        if(fallTimer > splatLimitTime) {
            RaycastHit hit;
            if (Physics.Raycast(Ctx.transform.position, Ctx.transform.TransformDirection(Vector3.down), out hit, .25f, Ctx.Environment)) {
                if (!Ctx.IsLandingAnimating) {
                    Ctx.Animator.SetBool(Ctx.isFallingHash, false);
                    Ctx.Animator.SetBool(Ctx.isHardLandingHash, true);
                    Ctx.IsLandingAnimating = true;
                    splatTimerStarted = true;
                    Ctx.InstantiateLandingDust();
                }
            }
        }

        // Prevent state switching and controls, until character stands up
        if (splatTimerStarted && timer < splatTime) {
            timer += Time.deltaTime;
        } else {
            splatTimerStarted = false;
            Ctx.IsLandingAnimating = false;
            CheckSwitchStates();
        }

    }

    public override void ExitState() {
        Ctx.Animator.SetBool(Ctx.isFallingHash, false);
        splatTimerStarted = false;
        timer = 0;
        fallTimer = 0;
    }

    public override void CheckSwitchStates() {
        if (Ctx.CharacterController.isGrounded) {
            SwitchState(Factory.Grounded());
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

    public void HandleGravity() {
        float previousYVelocity = Ctx.currentMovement.y;
        Ctx.currentMovement.y = Ctx.currentMovement.y + Ctx.Gravity * Time.deltaTime;
        Ctx.appliedMovement.y = Mathf.Max((previousYVelocity + Ctx.currentMovement.y) * 0.5f, -20f);
    }
}
