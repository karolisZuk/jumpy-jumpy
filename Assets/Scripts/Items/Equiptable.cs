using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equiptable : InventoryItem {
    [SerializeField] private GameObject prefab;

    protected Animator animator;

    private GameObject spawnedPrefab;

    public virtual void Equip(GameObject targetPoint, Animator anim) {
        animator = anim;
        // Spawn prefab at target point and set spawnedPrefab to store it
        // Set needed prefab params
    }

    public void Unequip() {
        // Destroy Spawned Prefab
    }
}
