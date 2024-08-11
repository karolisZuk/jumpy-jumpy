public class PlayerIdleState : PlayerBaseState {
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);

        Ctx.appliedMovement.x = 0;
        Ctx.appliedMovement.z = 0;
    }

    public override void UpdateState() {
        Ctx.appliedMovement.x = 0;
        Ctx.appliedMovement.z = 0;
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() {
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed) {
            SwitchState(Factory.Run());
        } else if (Ctx.IsMovementPressed) {
            SwitchState(Factory.Walk());
        }
    }
}
