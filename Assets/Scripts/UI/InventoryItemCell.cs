using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryItemCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private PlayerInputActions controls;
    private bool isHoveredOver = false;
    private InventoryItem cellItem;

    void Start() {
        controls = PlayerInputs.Instance.PlayerInputActions();
        controls.MenuControls.EquipLeft.performed += EquipLeft_performed;
        controls.MenuControls.EquipRight.performed += EquipRight_performed;
    }

    private void EquipLeft_performed(InputAction.CallbackContext obj) {
        if(isHoveredOver) {
            Debug.Log("TODO: Left" + cellItem.name);

            // TODO: Bubble up static event for equipment controller.
            // Controller should remove item from items slots and
            // add item to equipted slot
        }
    }

    private void EquipRight_performed(InputAction.CallbackContext obj) {
        if (isHoveredOver) {
            Debug.Log("TODO: Right" + cellItem.name);

            // TODO: Bubble up static event for equipment controller.
            // Controller should remove item from items slots and
            // add item to equipted slot
        }
    }

    public void SetItem(InventoryItem item) {
        cellItem = item;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHoveredOver = true;

        // TODO: Lerp scale to show interaction
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHoveredOver = false;
    }
}
