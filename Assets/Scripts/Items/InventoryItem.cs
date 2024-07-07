using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : ScriptableObject {
    [SerializeField] public string label;
    [SerializeField] public bool isStackable;
    [TextArea(15, 20)] public string description;
    [SerializeField] public EquipmentSlot equipmentSlot;
    [SerializeField] private Sprite inventoryIcon;
    [SerializeField] private InventoryItemType type;
    [SerializeField] public SpawnPoint spawnPoint;

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

public enum SpawnPoint {
    LeftArm,
    RightArm,
    Head
}