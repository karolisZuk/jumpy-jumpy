using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour, ICombatParticipant {
    private int health;

    public void Die() {
        throw new System.NotImplementedException();
    }

    public int GetHealth() {
        throw new System.NotImplementedException();
    }

    public void GetPushed(Vector3 dir) {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage) {
        throw new System.NotImplementedException();
    }
}
