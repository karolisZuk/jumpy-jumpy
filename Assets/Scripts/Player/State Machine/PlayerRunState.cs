using UnityEngine;

public class PlayerRunState : PlayerBaseState {
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
    }

    public override void UpdateState() {
        Ctx.appliedMovement.x = Ctx.CurrentVectorInput.x * Ctx.runMultiplier;
        Ctx.appliedMovement.z = Ctx.CurrentVectorInput.y * Ctx.runMultiplier;

        RaycastHit hit;

        Vector3 wallCheckRayCenter = Ctx.transform.position;
        wallCheckRayCenter.y += .5f;

        if (Physics.Raycast(wallCheckRayCenter, Ctx.transform.TransformDirection(Vector3.forward), out hit, 1f, Ctx.Environment)) {
            Ctx.Animator.SetBool(Ctx.isPushingHash, true);
        } else {
            Ctx.Animator.SetBool(Ctx.isPushingHash, false);
        }

        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() {
        if (!Ctx.IsMovementPressed) {
            Ctx.Animator.SetBool(Ctx.isPushingHash, false);
            SwitchState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed) {
            Ctx.Animator.SetBool(Ctx.isPushingHash, false);
            SwitchState(Factory.Walk());
        }
    }
}
