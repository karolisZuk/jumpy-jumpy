public class PlayerRunState : PlayerBaseState {
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
    }

    public override void UpdateState() {
        Ctx.appliedMovement.x = Ctx.CurrentVectorInput.x * Ctx.runMultiplier;
        Ctx.appliedMovement.z = Ctx.CurrentVectorInput.y * Ctx.runMultiplier;
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() {
        if (!Ctx.IsMovementPressed) {
            SwitchState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed) {
            SwitchState(Factory.Walk());
        }
    }
}
