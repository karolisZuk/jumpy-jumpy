using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : ScriptableObject {
    [SerializeField] protected string label;
    [SerializeField] public bool isStackable;
    [TextArea(15, 20)] public string description;
    [SerializeField] private Sprite inventoryIcon;
    [SerializeField] private InventoryItemType type;

    public InventoryItemType Type() {
        return type;
    }

    public Sprite InventoryIcon() {
        return inventoryIcon;
    }
}

public enum InventoryItemType {
    Equipment,
    Consumable,
    Quest
}