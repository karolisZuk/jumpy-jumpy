using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equiptable : InventoryItem, IEquiptable {
    [SerializeField] private GameObject prefab;

    protected Animator animator;
    protected EquipmentSlot equipmentSlot;
    protected GameObject spawnedPrefab;

    public virtual void Equip(GameObject targetPoint, Quaternion initialRotation, Animator anim, EquipmentSlot slot) {
        // Set variables
        animator = anim;
        equipmentSlot = slot;

        // Spawn prefab and parent it to correct location
        spawnedPrefab = Instantiate(prefab, targetPoint.transform.position, initialRotation);
        spawnedPrefab.transform.localScale = Vector3.one;
        spawnedPrefab.transform.SetParent(targetPoint.transform);
    }

    public void Unequip() {
        Destroy(spawnedPrefab);
        spawnedPrefab = null;
    }
}

public interface IEquiptable {
    public abstract void Equip(GameObject targetPoint, Quaternion initialRotation, Animator anim, EquipmentSlot slot);
    public abstract void Unequip();
}
