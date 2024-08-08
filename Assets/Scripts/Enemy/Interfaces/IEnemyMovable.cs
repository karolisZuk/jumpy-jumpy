using UnityEngine;

public interface IEnemyMovable {
    Rigidbody RB { get; set; }
    float RotationFactorPerFrame { get; set; }

    void MoveEnemy(Vector3 velocity);
    void CheckRotation(Vector3 velocity);
}
