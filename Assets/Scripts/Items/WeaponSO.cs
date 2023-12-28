using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Equiptable/Weapon")]
public class WeaponSO : Equiptable {
    [SerializeField] protected string animationName;
    [SerializeField] protected int damage;
    [SerializeField] protected bool canPush;
}
