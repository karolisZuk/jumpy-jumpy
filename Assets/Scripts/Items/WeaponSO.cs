using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Equiptable/Weapon")]
public class WeaponSO : Equiptable {
    [SerializeField] protected string animationName;
    [SerializeField] protected int damage;
    [SerializeField] protected bool canPush;
    [SerializeField] private float cooldown;

    public override GameObject Equip(GameObject targetPoint, Quaternion initialRotation, Animator anim, EquipmentSlot slot) {
        GameObject go = base.Equip(targetPoint, initialRotation, anim, slot);

        Weapon spawnedWeapon = go.GetComponent<Weapon>();

        if (spawnedWeapon != null) {
            spawnedWeapon.Init(anim, animationName, damage, canPush, cooldown, equipmentSlot);
        }

        return go;
    }
}


