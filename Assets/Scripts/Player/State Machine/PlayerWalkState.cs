using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!ctx.isMovementPressed)
            SwitchState(factory.Idle());
        else if (ctx.isMovementPressed && ctx.isRunPressed)
            SwitchState(factory.Run());
    }

    public override void EnterState()
    {
        ctx.animator.SetBool(ctx.isWalkingHash, true);
        ctx.animator.SetBool(ctx.isRunningHash, false);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        ctx.appliedMovement.x = ctx.currentMovementInput.x * ctx.walkMultiplier;
        ctx.appliedMovement.z = ctx.currentMovementInput.y * ctx.walkMultiplier;
    }
}
