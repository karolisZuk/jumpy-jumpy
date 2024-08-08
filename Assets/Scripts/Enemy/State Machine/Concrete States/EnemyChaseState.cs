using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState {

    private Transform playerTransform;
    private float movementSpeed = 3f;

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void ExitState() {
        base.ExitState();
    }

    public override void FrameUpdate() {
        base.FrameUpdate();

        Vector3 moveDirection = (playerTransform.position - enemy.transform.position).normalized;

        enemy.MoveEnemy(moveDirection * movementSpeed);
        enemy.CheckRotation(moveDirection * movementSpeed);

        if (enemy.IsWithinStrikingDistance) {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}

