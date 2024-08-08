using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IEnemyMovable, ITriggerCheckable {
    [field: SerializeField] public int MaxHealth { get; set; }
    [field: SerializeField] public float RotationFactorPerFrame { get; set; }
    public int CurrentHealth { get; set; }
    public Rigidbody RB { get; set; }
    public Rigidbody bulletPrefab;

    #region State Machine variables

    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }

    #endregion

    #region Idle variables

    public float RandomMovementRange = 5f;
    public float RandomMovementSpeed = 1f;

    #endregion

    #region Trigger variables

    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get;  set;}

    #endregion

    private float lastYposition;
    private bool grounded;

    private void Awake() {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
    }

    private void Start() {
        CurrentHealth = MaxHealth;
        RB = GetComponent<Rigidbody>();
        StateMachine.Initialize(IdleState);
        lastYposition = transform.position.y;
    }

    public void Update() {
        grounded = (lastYposition == transform.position.y);
        lastYposition = transform.position.y;

        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    public void FixedUpdate() {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region Health / Die

    public void Damage(int damageAmount) {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0) {
            Die();
        }
    }

    public void Die() {
        Destroy(gameObject);
    }

    #endregion

    #region Movement

    public void MoveEnemy(Vector3 velocity) {
        RB.velocity = velocity;
    }

    public void CheckRotation(Vector3 velocity) {
        Vector3 positionToLookAt;

        positionToLookAt.x = velocity.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = velocity.z;

        Quaternion currentRotation = transform.rotation;

        if (positionToLookAt.sqrMagnitude != 0) {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, RotationFactorPerFrame * Time.deltaTime);
        }
    }

    #endregion

    #region Animation triggers

    // TODO: Add animation trigger events to call this when animating
    private void AnimationTriggerEvent(AnimationTriggerType triggerType) {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType {
        EnemyDamaged,
        PlayFootstepSounds,
        Notice
    }

    #endregion

    #region Check triggers

    public void SetAggroStatus(bool isAggroed) {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance) {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }

    #endregion
}
