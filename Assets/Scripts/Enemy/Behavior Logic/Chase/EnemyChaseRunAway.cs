using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Run Away", menuName = "Enemy Logic/Chase Logic/Run Away")]
public class EnemyChaseRunAway : EnemyChaseSOBase {
    [SerializeField] private float runAwaySpeed = 3f;

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

        Vector3 moveDirection = (playerTransform.position - enemy.transform.position).normalized * -1f;
        enemy.MoveEnemy(moveDirection * runAwaySpeed);
        enemy.CheckRotation(moveDirection * runAwaySpeed);

        if (!enemy.IsAggroed) {
            enemy.StateMachine.ChangeState(enemy.IdleState);
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
}
