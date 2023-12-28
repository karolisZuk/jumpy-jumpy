using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : InventoryItem, IConsumable {
    [SerializeField] private GameObject prefab;

    protected Animator animator;
    private GameObject spawnedPrefab;

    public virtual void Use() {

    }
}

public interface IConsumable {
    public abstract void Use();
}
