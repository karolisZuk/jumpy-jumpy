using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Items/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver {
    [SerializeField] private string savePath;

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

    public void Save() {
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));

        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load() {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath))) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
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