using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Items/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver {

    public ItemDatabaseSO database;
    public List<InventorySlot> Container = new List<InventorySlot>();

    public void AddItem(InventoryItem _item, int _amount) {
        foreach(InventorySlot slot in Container) {
            if(slot.item == _item) {
                slot.AddAmount(_amount);
                return;
            }
        }

        Container.Add(new InventorySlot(database.GetId[_item], _item, _amount));
    }

    public void OnAfterDeserialize() {
        for(int i = 0; i < Container.Count; i++) {
            InventoryItem deserializedItem = database.GetItem[Container[i].ID];
            int id = database.GetId[deserializedItem];
            Container[i] = new InventorySlot(id, deserializedItem, Container[i].amount);
        }
    }

    public void OnBeforeSerialize() {}
}

[System.Serializable]
public class InventorySlot {
    public int ID;
    public InventoryItem item;
    public int amount;

    public InventorySlot(int _id, InventoryItem _item, int _amount) {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value) {
        amount += value;
    }
}