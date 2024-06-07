using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryItemCell : MonoBehaviour, IInventoryCell, IPointerEnterHandler, IPointerExitHandler {
    public static event EventHandler<EquipActionTO> OnEquipItem;

    protected PlayerInputActions controls;

    private bool isHoveredOver = false;
    private InventoryItem cellItem;

    private void Start() {
        controls = PlayerInputs.Instance.PlayerInputActions();
        controls.MenuControls.Equip.performed += Equip_performed;
    }

    void OnDestroy() {
        isHoveredOver = false;
    }

    protected void Equip_performed(InputAction.CallbackContext obj) {
        if (isHoveredOver) {
            OnEquipItem?.Invoke(this, new EquipActionTO(cellItem, cellItem.equipmentSlot));
        }
    }

    public void SetItem(InventoryItem item) {
        cellItem = item;
    }

    public void RemoveItem() {
        cellItem = null;
        isHoveredOver = false;
    }

    public InventoryItem GetItem() {
        return cellItem;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHoveredOver = true;

        // TODO: Lerp scale to show interaction
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHoveredOver = false;

        // TODO: Revert lerp

    }
}

public struct EquipActionTO {
    public InventoryItem inventoryItem;
    public EquipmentSlot slot;

    public EquipActionTO(InventoryItem inventoryItem, EquipmentSlot slot) {
        this.inventoryItem = inventoryItem;
        this.slot = slot;
    }
}
