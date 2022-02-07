using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
    }

    public override void EnterState()
    {
        HandleJump();
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

    void HandleJump()
    {
        ctx.isJumping = true;
        ctx.isJumpAnimating = true;
        ctx.animator.SetBool(ctx.isJumpingHash, true);

        ctx.currentMovement.y = ctx.initialJumpVelocity;
        ctx.appliedMovement.y = ctx.initialJumpVelocity;
    }
}
