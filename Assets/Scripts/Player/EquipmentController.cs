using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class EquipmentController : MonoBehaviour {
    public static event EventHandler OnMenuOpen;

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

    [Header("Inventories")]
    [SerializeField] private InventoryObject equipmentInventory;
    [SerializeField] private InventoryObject consumablesInventory;
    [SerializeField] private InventoryObject questItemsInventory;

    List<GameObject> itemsDisplayed = new List<GameObject>();

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
        for(int i = 0; i < equipmentInventory.Container.Count; i++) {
            InventoryItem item = equipmentInventory.Container[i].item;
            Sprite sprite = item.InventoryIcon();
            GameObject obj = Instantiate(equipmentUICell, equipmentInventoryPanel.transform);

            obj.GetComponent<Image>().sprite = sprite;
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i, equipmentInventoryPanel.GetComponent<RectTransform>());
            obj.GetComponentInChildren<TextMeshProUGUI>().text = equipmentInventory.Container[i].amount.ToString("n0");

            itemsDisplayed.Add(obj);
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
