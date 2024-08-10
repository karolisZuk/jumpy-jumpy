using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState {
    public Transform playerTransform;

    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) {
        base.AnimationTriggerEvent(triggerType);
        enemy.EnemyAttackBaseInstance.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void EnterState() {
        base.EnterState();
        enemy.EnemyAttackBaseInstance.DoEnterLogic();
    }

    public override void ExitState() {
        base.ExitState();
        enemy.EnemyAttackBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate() {
        base.FrameUpdate();
        enemy.EnemyAttackBaseInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
        enemy.EnemyAttackBaseInstance.DoPhysicsLogic();
    }
}
