using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle Random Wander", menuName = "Enemy Logic/Idle Logic/Random Wander")]
public class EnemyIdleRandomWander : EnemyIdleSOBase {
    [SerializeField] private float RandomMovementRange = 5f;
    [SerializeField] private float RandomMovementSpeed = 1f;

    private Vector3 targetPos;
    private Vector3 direction;

    public override void DoAnimationTriggerEventLogic(Enemy.AnimationTriggerType triggerType) {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic() {
        base.DoEnterLogic();

        targetPos = GetRandomPointInCircle();
    }

    public override void DoExitLogic() {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic() {
        base.DoFrameUpdateLogic();

        direction = (targetPos - enemy.transform.position).normalized;

        enemy.MoveEnemy(direction * RandomMovementSpeed);
        enemy.CheckRotation(direction * RandomMovementSpeed);

        if (Vector3.Distance(enemy.transform.position, targetPos) < 2f) {
            targetPos = GetRandomPointInCircle();
        }
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

    private Vector3 GetRandomPointInCircle() {
        Vector3 randomPos = enemy.transform.position + Random.insideUnitSphere * RandomMovementRange;
        randomPos.y = 0;

        return randomPos;
    }
}
