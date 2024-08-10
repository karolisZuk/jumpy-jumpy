using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState {
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        IsRootState = true;
    }

    public override void EnterState() {
        InitializeSubState();
    }

    public override void UpdateState() {
        HandleGravity();

        if (Ctx.appliedMovement.y < Ctx.Gravity * 1.1f && !Ctx.Animator.GetBool(Ctx.isFallingHash)) {
            Ctx.Animator.SetBool(Ctx.isFallingHash, true);
        }

        CheckSwitchStates();
    }

    public override void ExitState() {
        Ctx.Animator.SetBool(Ctx.isFallingHash, false);
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
