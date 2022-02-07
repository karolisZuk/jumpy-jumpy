using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        // TODO: This check never passees
        if(ctx.characterController.isGrounded)
        {
            SwitchState(factory.Grounded());
        }
    }

    public override void EnterState()
    {
        HandleJump();
    }

    public override void ExitState()
    {
        ctx.animator.SetBool(ctx.isJumpingHash, false);
        ctx.isJumpAnimating = false;
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleGravity();
    }

    void HandleJump()
    {
        ctx.isJumping = true;
        ctx.isJumpAnimating = true;
        ctx.animator.SetBool(ctx.isJumpingHash, true);

        ctx.currentMovement.y = ctx.initialJumpVelocity;
        ctx.appliedMovement.y = ctx.initialJumpVelocity;
    }

    void HandleGravity()
    {
        bool isFalling = ctx.currentMovement.y <= 0f || !ctx.isJumpPressed;

        if (isFalling)
        {
            float previousYVelocity = ctx.currentMovement.y;
            ctx.currentMovement.y = ctx.currentMovement.y + (ctx.gravity * ctx.fallMultiplier * Time.deltaTime);
            ctx.appliedMovement.y = Mathf.Max((previousYVelocity + ctx.currentMovement.y) * .5f, -20f);
        }
        else
        {
            float previousYVelocity = ctx.currentMovement.y;
            ctx.currentMovement.y = ctx.currentMovement.y + (ctx.gravity * Time.deltaTime);
            ctx.appliedMovement.y = (previousYVelocity + ctx.currentMovement.y) * .5f;
        }
    }
}
