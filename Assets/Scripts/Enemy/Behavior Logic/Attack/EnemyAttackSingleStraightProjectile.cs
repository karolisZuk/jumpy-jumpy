using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Staight-Single Projectile", menuName = "Enemy Logic/Attack Logic/Straight Single Projectile")]
public class EnemyAttackSingleStraightProjectile : EnemyAttackSOBase {
    [SerializeField] private Rigidbody bulletPrefab;
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float shotFacingAngle = 45f;
    [SerializeField] private float attackRotationSpeed = 10f;
    [SerializeField] private float timeTillExit = 3f;

    private float attackTimer;
    private float exitTimer;

    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic() {
        base.DoEnterLogic();
    }

    public override void DoExitLogic() {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic() {
        base.DoFrameUpdateLogic();

        enemy.MoveEnemy(Vector3.zero);

        Vector3 rotationDirection = (playerTransform.position - enemy.transform.position).normalized;
        enemy.CheckRotation(rotationDirection * attackRotationSpeed);

        if (attackTimer > timeBetweenShots) {

            // Only shoot if facing player
            if (Vector3.Angle(enemy.transform.forward, playerTransform.position - enemy.transform.position) < shotFacingAngle) {
                attackTimer = 0f;

                Vector3 dir = (playerTransform.position - enemy.transform.position).normalized;
                dir.y += 0.15f;

                Rigidbody bullet = GameObject.Instantiate(bulletPrefab, enemy.transform.position, Quaternion.identity);

                bullet.velocity = dir * bulletSpeed;
            }
        }

        if (!enemy.IsWithinStrikingDistance) {
            exitTimer += Time.deltaTime;

            if (exitTimer > timeTillExit) {
                enemy.StateMachine.ChangeState(enemy.ChaseState);
            }
        } else {
            exitTimer = 0f;
        }

        attackTimer += Time.deltaTime;
    }

    public override void DoPhysicsLogic() {
        base.DoPhysicsLogic();
    }

    public override void Initialize(GameObject gameObject, Enemy enemy) {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues() {
        base.ResetValues();
    }
}
