using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState {
    private Transform playerTransform;
    private float timer;
    private float timeBetweenShots = 1f;
    private float bulletSpeed = 15f;
    private float shotFacingAngle = 45f;
    private float attackRotationSpeed = 10f;

    private float exitTimer;
    private float timeTillExit = 3f;
    private float distanceToCountExit = 3f;

    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) {
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

        enemy.MoveEnemy(Vector3.zero);
        enemy.CheckRotation(playerTransform.position * attackRotationSpeed);

        if (timer > timeBetweenShots) {
            timer = 0f;

            // Only shoot if facing player
            if (Vector3.Angle(enemy.transform.forward, playerTransform.position - enemy.transform.position) < shotFacingAngle) {

                Vector3 dir = (playerTransform.position - enemy.transform.position).normalized;
                dir.y += 0.15f;

                Rigidbody bullet = GameObject.Instantiate(enemy.bulletPrefab, enemy.transform.position, Quaternion.identity);

                bullet.velocity = dir * bulletSpeed;
            } else {
                enemy.CheckRotation(playerTransform.position * attackRotationSpeed);
            }
        }

        if (Vector3.Distance(playerTransform.position, enemy.transform.position) > distanceToCountExit) {
            exitTimer += Time.deltaTime;

            if (exitTimer > timeTillExit) {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
            }
        } else {
            exitTimer = 0f;
        }

        timer += Time.deltaTime;
    }

    public override void PhysicsUpdate() {
        base.PhysicsUpdate();
    }
}
