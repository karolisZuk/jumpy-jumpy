using UnityEngine;

public class PlayerDodgeState : PlayerBaseState {
    private float dodgeTime = 0.5f;
    private float timer;

    public PlayerDodgeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        timer = 0;
        Ctx.EnableForceMode();
        Ctx.Animator.SetTrigger(Ctx.dodgeHash);
        Ctx.IsDodging = true;
        Ctx.IsDodgePressed = false;
        HandleGravity();
        HandleDodge();
    }

    public override void UpdateState() {
        if (timer < dodgeTime) {
            timer += Time.deltaTime;
        } else {
            Ctx.IsDodging = false; 
        }

        CheckSwitchStates();
    }

    public override void ExitState() {
        Ctx.DisableForceMode();
        Ctx.IsDodging = false;
        timer = 0;
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates() {
        RaycastHit hit;
        if (!Physics.Raycast(Ctx.transform.position, Ctx.transform.TransformDirection(Vector3.down), out hit, .5f, Ctx.Environment)) {
            SwitchState(Factory.Fall());
        }

        else if (timer > dodgeTime) {
            if (Ctx.IsMovementPressed && Ctx.IsRunPressed) {
                SwitchState(Factory.Run());
            } else if (Ctx.IsMovementPressed) {
                SwitchState(Factory.Walk());
            } else {
            SwitchState(Factory.Idle()); 
            }
        }
    }

    void HandleDodge() {
        // TODO: Make unhitable for split second when dodging
        Ctx.Rb.AddForce(new Vector3(Ctx.transform.forward.x, 0.1f, Ctx.transform.forward.z) * Ctx.dodgeMultiplier, ForceMode.Impulse);
    }

    public void HandleGravity() {
        Ctx.currentMovement.y = Ctx.Gravity;
        Ctx.appliedMovement.y = Ctx.Gravity;
    }
}
