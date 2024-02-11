using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour, ICombatParticipant {
    [SerializeField] private int maxHealth;
    private int currentHealth;
    private bool isDead = false;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void Die() {
        isDead = true;
        // TODO: Clean up
    }

    public int GetHealth() {
        return currentHealth;
    }

    public bool IsAlive() {
        return !isDead;
    }

    public bool IsDead() {
        return isDead;
    }

    public void GetPushed(Vector3 dir) {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        if(currentHealth <= 0) {
            Die();
        }
     }
}
