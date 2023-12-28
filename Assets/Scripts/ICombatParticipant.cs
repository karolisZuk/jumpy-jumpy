using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatParticipant {
    public void TakeDamage(int damage);
    public void Die();
    public int GetHealth();
    public void GetPushed(Vector3 dir);
}
