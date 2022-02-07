using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

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
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
