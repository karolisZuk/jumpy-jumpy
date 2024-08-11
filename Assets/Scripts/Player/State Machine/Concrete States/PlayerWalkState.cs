public class PlayerWalkState : PlayerBaseState {

    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }

    public override void UpdateState() {
        Ctx.appliedMovement.x = Ctx.CurrentVectorInput.x * Ctx.walkMultiplier;
        Ctx.appliedMovement.z = Ctx.CurrentVectorInput.y * Ctx.walkMultiplier;
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() {
        if (!Ctx.IsMovementPressed) {
            SwitchState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && Ctx.IsRunPressed) {
            SwitchState(Factory.Run());
        }
    }
}
