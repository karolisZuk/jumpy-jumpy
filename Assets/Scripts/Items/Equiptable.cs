using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equiptable : InventoryItem, IEquiptable {
    [SerializeField] private GameObject prefab;

    protected Animator animator;
    private GameObject spawnedPrefab;

    public virtual void Equip(GameObject targetPoint, Animator anim) {
        animator = anim;
        spawnedPrefab = Instantiate(prefab);
        spawnedPrefab.transform.localPosition = targetPoint.transform.position;
        spawnedPrefab.transform.localScale = Vector3.one;

        spawnedPrefab.transform.SetParent(targetPoint.transform);
    }

    public void Unequip() {
        // Destroy Spawned Prefab
        Destroy(spawnedPrefab);
        spawnedPrefab = null;
    }
}

public interface IEquiptable {
    public abstract void Equip(GameObject targetPoint, Animator anim);
    public abstract void Unequip();
}
