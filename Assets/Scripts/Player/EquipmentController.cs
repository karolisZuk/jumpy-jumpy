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
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;

    [Header("Inventory UI")]
    [SerializeField] private bool clearOnQuit = true;
    [SerializeField] private int X_SPACE_BETWEEN_ITEM;
    [SerializeField] private int Y_SPACE_BETWEEN_ITEM;
    [SerializeField] private int NUMBER_OF_COLUMNS;
    [SerializeField] private GameObject equipmentUICell;
    [SerializeField] private GameObject equipmentInventoryPanel;
    [SerializeField] private GameObject weaponSlot;

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
        itemsDisplayed.Clear();

        for (int i = 0; i < equipmentInventory.Container.Count; i++) {
            EquipmentCell weaponSlotCell = weaponSlot.GetComponent<EquipmentCell>();
            InventoryItem item = equipmentInventory.Container[i].item;
            Sprite sprite = item.InventoryIcon();

            if (IsItemDisplayedInGrid(item)) {
                // Skip if item is equipted, or is already displayed;
                continue;
            }

            // Render item cell
            GameObject obj = Instantiate(equipmentUICell, equipmentInventoryPanel.transform);
            InventoryItemCell cell = obj.GetComponent<InventoryItemCell>();

            obj.GetComponent<Image>().sprite = sprite;
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i, equipmentInventoryPanel.GetComponent<RectTransform>());

            if (IsItemEquipted(weaponSlotCell, item)) {
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "E";
            } else {
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

            cell.SetItem(item);

            itemsDisplayed.Add(obj);
        }
    }

    private bool IsItemEquipted(IInventoryCell cell, InventoryItem item) {
        if (cell.GetItem() == null) {
            return false;
        }

        return cell.GetItem().GetInstanceID() == item.GetInstanceID();
    }

    private bool IsItemDisplayedInGrid(InventoryItem item) {
        bool res = false;

        foreach (GameObject gridCell in itemsDisplayed) {
            InventoryItemCell cell = gridCell.GetComponent<InventoryItemCell>();

            if (cell.GetItem().name == item.name) {
                res = true;
                break;
            }
        }

        return res;
    }

    private void InventoryItemCell_OnUnequipItem(object sender, EquipActionTO e) {
        Debug.Log("TODO: Do not allow unequip from main weapon slot");
        // Destroy spawned gameobject
        for (int i = spawnedFieldItems.Count - 1; i > -1; --i) {
            Weapon weapon = spawnedFieldItems[i].GetComponent<Weapon>();

            if (weapon != null && e.inventoryItem.equipmentSlot == weapon.GetSlot()) {
                Destroy(spawnedFieldItems[i]);
                spawnedFieldItems.RemoveAt(i);
            }
        }

        // Remove icon from body slot
        GameObject resetSlot = GetAffectedSlot(e.inventoryItem.equipmentSlot);

        if (resetSlot != null) {
            EquipmentCell cell = resetSlot.GetComponent<EquipmentCell>();
            cell.RemoveItem();
        }

        CreateInventoryDisplay();
    }

    private void InventoryItemCell_OnEquipItem(object sender, EquipActionTO e) {
        GameObject slot = GetAffectedSlot(e.inventoryItem.equipmentSlot);
  
        //RemoveItemIconFromItemsList(e);
        SetEquiptedItemSlotIcons(slot, e);
        CreateInventoryDisplay();

        if (e.inventoryItem is IEquiptable) {
            SpawnEquiptedItem(e.inventoryItem, e.slot);
        }
    }

    // Converts slot enum to slot gameobject
    private GameObject GetAffectedSlot(EquipmentSlot slot) {
        // TODO: Add other slots

        if (slot == EquipmentSlot.Weapon) {
            return weaponSlot;
        }

        return null;
    }

    private void SetEquiptedItemSlotIcons(GameObject slot, EquipActionTO e) {
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

    private void RemoveItemIconFromItemsList(EquipActionTO e) {
        GameObject toRemove = null;

        foreach (GameObject item in itemsDisplayed) {
            InventoryItemCell cell = item.GetComponent<InventoryItemCell>();

            if (cell.GetItem().name == e.inventoryItem.name) {
                toRemove = item;
                break;
            }
        }

        if (toRemove != null) {
            itemsDisplayed.Remove(toRemove);
            Destroy(toRemove);
        }
    }

    private void SpawnEquiptedItem(InventoryItem item, EquipmentSlot slot) {
        GameObject s = null;

        if (item.spawnPoint == SpawnPoint.RightArm) {
            s = rightArm;
        } else if (item.spawnPoint == SpawnPoint.LeftArm) {
            s = leftArm;
        }

        if (s != null) {
            GameObject spawnedFieldItem = (item as IEquiptable).Equip(s, transform.rotation, animator, slot);

            spawnedFieldItems.Add(spawnedFieldItem);
        }
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
    Weapon
}