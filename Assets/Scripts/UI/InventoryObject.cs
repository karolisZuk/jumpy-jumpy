using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Items/Inventory")]
public class InventoryObject : ScriptableObject {
    
    public List<InventorySlot> Container = new List<InventorySlot>();

    public void AddItem(InventoryItem _item, int _amount) {
        bool hasItem = false;

        foreach(InventorySlot slot in Container) {
            if(slot.item == _item) {
                slot.AddAmount(_amount);
                hasItem = true;
                break;
            }
        }

        if(!hasItem) {
            Container.Add(new InventorySlot(_item, _amount));
        }
    }
}

[System.Serializable]
public class InventorySlot {
    public InventoryItem item;
    public int amount;

    public InventorySlot(InventoryItem _item, int _amount) {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value) {
        amount += value;
    }
}