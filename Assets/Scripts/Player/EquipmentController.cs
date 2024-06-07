using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class EquipmentController : MonoBehaviour {
    public static event EventHandler OnMenuOpen;

    [SerializeField] private Animator animator;

    [Header("Equipment points")]
    [SerializeField] private GameObject equipmentPointLeft;
    [SerializeField] private GameObject equipmentPointRight;

    [Header("Inventory UI")]
    [SerializeField] private bool clearOnQuit = true;
    [SerializeField] private int X_SPACE_BETWEEN_ITEM;
    [SerializeField] private int Y_SPACE_BETWEEN_ITEM;
    [SerializeField] private int NUMBER_OF_COLUMNS;
    [SerializeField] private GameObject equipmentUICell;
    [SerializeField] private GameObject equipmentInventoryPanel;
    [SerializeField] private GameObject inventoryLeftHandSlot;
    [SerializeField] private GameObject inventoryRightHandSlot;

    [Header("Inventories")]
    [SerializeField] private InventoryObject equipmentInventory;
    [SerializeField] private InventoryObject consumablesInventory;
    [SerializeField] private InventoryObject questItemsInventory;

    private List<GameObject> itemsDisplayed = new List<GameObject>();
    private List<GameObject> spawnedFieldItems = new List<GameObject>();

    [Header("Pickups")]
    [SerializeField] private LayerMask itemLayer;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = PlayerInputs.Instance.PlayerInputActions();
        playerInputActions.CharacterControls.Enable();

        playerInputActions.CharacterControls.ShowMenu.started += OnShowMenu;

        // TODO: Testing items saving and loading
        playerInputActions.MenuControls.SaveTest.started += SaveTest_started;
        playerInputActions.MenuControls.LoadTest.started += LoadTest_started;

        PlayerMenu.OnMenuClose += OnHideMenu;
        InventoryItemCell.OnEquipItem += InventoryItemCell_OnEquipItem;
        EquipmentCell.OnUnequipItem += InventoryItemCell_OnUnequipItem;
    }

    private void SaveTest_started(InputAction.CallbackContext obj) {
        SaveInventory();
    }

    private void LoadTest_started(InputAction.CallbackContext obj) {
        LoadInventory();
    }

    public void OnTriggerEnter(Collider other) {
        if (((1 << other.gameObject.layer) & itemLayer) != 0) {
            Item inventoryItem = other.GetComponent<Item>();

            if (inventoryItem != null) {
                if (inventoryItem.item.Type() == InventoryItemType.Equipment) {
                    equipmentInventory.AddItem(inventoryItem.item, 1);
                } else if (inventoryItem.item.Type() == InventoryItemType.Consumable) {
                    consumablesInventory.AddItem(inventoryItem.item, 1);
                } else if (inventoryItem.item.Type() == InventoryItemType.Quest) {
                    questItemsInventory.AddItem(inventoryItem.item, 1);
                }

                Destroy(other.gameObject);
            }
        }
    }

    public void SaveInventory() {
        equipmentInventory.Save();
        consumablesInventory.Save();
        questItemsInventory.Save();
    }

    public void LoadInventory() {
        equipmentInventory.Load();
        consumablesInventory.Load();
        questItemsInventory.Load();

        foreach (GameObject go in itemsDisplayed) {
            Destroy(go);
        }

        itemsDisplayed.Clear();

        CreateInventoryDisplay();
    }

    private void OnHideMenu(object sender, EventArgs e) {
        playerInputActions.CharacterControls.Enable();

        foreach(GameObject go in itemsDisplayed) {
            Destroy(go);
        }

        itemsDisplayed.Clear();
    }

    private void OnShowMenu(InputAction.CallbackContext obj) {
        playerInputActions.CharacterControls.Disable();
        OnMenuOpen?.Invoke(this, EventArgs.Empty);
        CreateInventoryDisplay();
    }

    private void CreateInventoryDisplay() {
        for (int i = 0; i < equipmentInventory.Container.Count; i++) {
            EquipmentCell LCell = inventoryLeftHandSlot.GetComponent<EquipmentCell>();
            EquipmentCell RCell = inventoryRightHandSlot.GetComponent<EquipmentCell>();

            InventoryItem item = equipmentInventory.Container[i].item;
            Sprite sprite = item.InventoryIcon();

            if (!this.IsItemEquipted(LCell, item) && !this.IsItemEquipted(RCell, item)) {
                GameObject obj = Instantiate(equipmentUICell, equipmentInventoryPanel.transform);
                InventoryItemCell cell = obj.GetComponent<InventoryItemCell>();

                obj.GetComponent<Image>().sprite = sprite;
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i, equipmentInventoryPanel.GetComponent<RectTransform>());
                obj.GetComponentInChildren<TextMeshProUGUI>().text = equipmentInventory.Container[i].amount.ToString("n0");
                cell.SetItem(item);

                itemsDisplayed.Add(obj);
            }
        }
    }

    private bool IsItemEquipted(IInventoryCell cell, InventoryItem item) {
        if (cell.GetItem() == null) {
            return false;
        }

        return cell.GetItem().GetInstanceID() == item.GetInstanceID();
    }

    private void InventoryItemCell_OnUnequipItem(object sender, EquipActionTO e) {
        // Destroy spawned gameobject
        for (int i = spawnedFieldItems.Count - 1; i > -1; --i) {
            Weapon weapon = spawnedFieldItems[i].GetComponent<Weapon>();

            if (weapon != null && e.inventoryItem.equipmentSlot == weapon.GetSlot()) {
                Destroy(spawnedFieldItems[i]);
                spawnedFieldItems.RemoveAt(i);
            }
        }

        // Remove icon from body slot
        List<GameObject> resetSlots = GetAffectedSlots(e.inventoryItem.equipmentSlot);

        foreach (GameObject slot in resetSlots) {
            EquipmentCell cell = slot.GetComponent<EquipmentCell>();
            cell.RemoveItem();
        }

        // Create new icon in items list
        CreateInventoryDisplay();
    }

    private void InventoryItemCell_OnEquipItem(object sender, EquipActionTO e) {
        List<GameObject> slots = GetAffectedSlots(e.inventoryItem.equipmentSlot);

        SetEquiptedItemSlotIcons(slots, e);
        RemoveItemIconFromItemsList(e);


        if (e.inventoryItem is IEquiptable) {
            SpawnEquiptedItem(e.inventoryItem, e.slot);
        }
    }

    // Converts slot enum to slot gameobjects
    private List<GameObject> GetAffectedSlots(EquipmentSlot slot) {
        List<GameObject> slots = new List<GameObject>();

        if (slot == EquipmentSlot.LeftHand) {
            slots.Add(inventoryLeftHandSlot);
        } else if (slot == EquipmentSlot.RightHand) {
            slots.Add(inventoryRightHandSlot);
        } else if (slot == EquipmentSlot.BothHands) {
            slots.Add(inventoryRightHandSlot);
            slots.Add(inventoryLeftHandSlot);
        }

        return slots;
    }

    private void SetEquiptedItemSlotIcons(List<GameObject> slots, EquipActionTO e) {
        RemoveItemIconFromItemsList(e);

        foreach (GameObject slot in slots) {
            EquipmentCell cell = slot.GetComponent<EquipmentCell>();

            // Cell is not empty
            if (cell.GetItem() != null) {
                InventoryItem item = cell.GetItem();

                InventoryItemCell_OnUnequipItem(this, new EquipActionTO(item, e.slot));
                cell.RemoveItem();
            }

            Image img = slot.GetComponent<Image>();
            img.sprite = e.inventoryItem.InventoryIcon();
            img.color = Color.white;

            cell.SetItem(e.inventoryItem);
        }

        CreateInventoryDisplay();
    }

    private void RemoveItemIconFromItemsList(EquipActionTO e) {
        GameObject toRemove = null;

        foreach (GameObject item in itemsDisplayed) {
            InventoryItemCell cell = item.GetComponent<InventoryItemCell>();

            if (cell.GetItem().GetInstanceID() == e.inventoryItem.GetInstanceID()) {
                toRemove = item;
                break;
            }
        }

        itemsDisplayed.Remove(toRemove);
        Destroy(toRemove);
    }

    private void SpawnEquiptedItem(InventoryItem item, EquipmentSlot slot) {
        GameObject s = slot == EquipmentSlot.LeftHand ? equipmentPointLeft : equipmentPointRight;
        GameObject spawnedFieldItem = (item as IEquiptable).Equip(s, transform.rotation, animator, slot);

        spawnedFieldItems.Add(spawnedFieldItem);
    }

    private Vector3 GetPosition(int i, RectTransform parent) {
        float X_START = -parent.offsetMin.x + 40f;
        float Y_START = (-parent.offsetMin.y / 2f) - 20f;
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMNS)), Y_START + (-Y_SPACE_BETWEEN_ITEM * (i / NUMBER_OF_COLUMNS)), 0f);
    }

    private void OnApplicationQuit() {
        if(clearOnQuit) {
            if (equipmentInventory.Container.Count > 0) {
                equipmentInventory.Container.Clear();
            }

            if (consumablesInventory.Container.Count > 0) {
                consumablesInventory.Container.Clear();
            }

            if (questItemsInventory.Container.Count > 0) {
                questItemsInventory.Container.Clear();
            }
        }
    }
}

public enum EquipmentSlot {
    LeftHand, RightHand, BothHands
}