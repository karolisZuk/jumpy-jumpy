using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState {
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        IsRootState = true;
    }

    public override void EnterState() {
        InitializeSubState();
        HandleGravity();
    }

    public override void ExitState() {}

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
        CheckSwitchStates();
    }

    public override void CheckSwitchStates() {
        if (Ctx.IsJumpPressed) {
            SwitchState(Factory.Jump());
        }

        else if (!Ctx.CharacterController.isGrounded) {
            SwitchState(Factory.Fall());
        }
    }

    public void HandleGravity() {
        Ctx.currentMovement.y = Ctx.Gravity;
        Ctx.appliedMovement.y = Ctx.Gravity;
    }
}
