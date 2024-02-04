using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryItemCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public static event EventHandler<EquipActionTO> OnEquipItem;

    private PlayerInputActions controls;
    private bool isHoveredOver = false;
    private InventoryItem cellItem;

    void Start() {
        controls = PlayerInputs.Instance.PlayerInputActions();
        controls.MenuControls.EquipLeft.performed += EquipLeft_performed;
        controls.MenuControls.EquipRight.performed += EquipRight_performed;
    }

    private void EquipLeft_performed(InputAction.CallbackContext obj) {
        if (isHoveredOver) {
            OnEquipItem?.Invoke(this, new EquipActionTO(cellItem, EquipmentSlot.LeftHand));
        }
    }

    private void EquipRight_performed(InputAction.CallbackContext obj) {
        if (isHoveredOver) {
            OnEquipItem?.Invoke(this, new EquipActionTO(cellItem, EquipmentSlot.RightHand));
        }
    }

    public void SetItem(InventoryItem item) {
        cellItem = item;
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
