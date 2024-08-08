using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState {
    private Vector3 targetPos;
    private Vector3 direction;

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) {

    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState() {
        base.EnterState();

        targetPos = GetRandomPointInCircle();
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void FrameUpdate() {
        base.FrameUpdate();

        if (enemy.IsAggroed) {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }

        direction = (targetPos - enemy.transform.position).normalized;

        enemy.MoveEnemy(direction * enemy.RandomMovementSpeed);

        if (Vector3.Distance(enemy.transform.position, targetPos) < 2f) {
            targetPos = GetRandomPointInCircle();
        }

    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }

    private Vector3 GetRandomPointInCircle() {
        Vector3 randomPos = enemy.transform.position + Random.insideUnitSphere * enemy.RandomMovementRange;
        randomPos.y = 0;

        return randomPos;
    }
}
