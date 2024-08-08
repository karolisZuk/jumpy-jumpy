using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour, IDamagable {
    [field: SerializeField] public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }

    private void Start() {
        CurrentHealth = MaxHealth;
    }

    public void Damage(int damageAmount) {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0) {
            Die();
        }
    }

    public void Die() {

    }
}
