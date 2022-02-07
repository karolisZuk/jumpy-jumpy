using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
        isRootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        if(ctx.isJumpPressed && !ctx.requireNewJumpPress)
        {
            SwitchState(factory.Jump());
        }
    }

    public override void EnterState()
    {
        ctx.currentMovement.y = ctx.groundedGravity;
        ctx.appliedMovement.y = ctx.groundedGravity;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        if(!ctx.isMovementPressed && !ctx.isRunPressed)
            SetSubState(factory.Idle());
        else if (ctx.isMovementPressed && !ctx.isRunPressed)
            SetSubState(factory.Walk());
        else
            SetSubState(factory.Run());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
