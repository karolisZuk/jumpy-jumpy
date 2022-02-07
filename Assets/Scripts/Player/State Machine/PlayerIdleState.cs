using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) {}

    public override void CheckSwitchStates()
    {
        if (ctx.isMovementPressed && ctx.isRunPressed)
            SwitchState(factory.Run());
        else if (ctx.isMovementPressed)
            SwitchState(factory.Walk());
    }

    public override void EnterState()
    {
        ctx.animator.SetBool(ctx.isWalkingHash, false);
        ctx.animator.SetBool(ctx.isRunningHash, false);
        ctx.appliedMovement.x = 0;
        ctx.appliedMovement.z = 0;
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
    }
}
