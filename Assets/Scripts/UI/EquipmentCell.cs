using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipmentCell : MonoBehaviour, IInventoryCell, IPointerEnterHandler, IPointerExitHandler {
    // TODO: Store default image and change back to it when item is removed
    public static event EventHandler<EquipActionTO> OnUnequipItem;
    public InventoryItem cellItem;

    protected PlayerInputActions controls;

    private bool isHoveredOver = false;
    private Color startingColor;
    private Sprite startingImage;
    private Image image;


    private void Start() {
        controls = PlayerInputs.Instance.PlayerInputActions();
        controls.MenuControls.Equip.performed += Unequip_performed;
        image = GetComponent<Image>();

        startingImage = image.sprite;
        startingColor = image.color;
    }

    void OnDestroy() {
        isHoveredOver = false;
    }


    protected void Unequip_performed(InputAction.CallbackContext obj) {
        if (isHoveredOver) {

            if (cellItem != null) {
                OnUnequipItem?.Invoke(this, new EquipActionTO(cellItem, cellItem.equipmentSlot));

                RemoveItem();
            }
        }
    }

    public void SetItem(InventoryItem item) {
        cellItem = item;
    }

    public void RemoveItem() {
        cellItem = null;
        isHoveredOver = false;
        Image img = GetComponent<Image>();
        img.sprite = startingImage;
        img.color = startingColor;
    }

    public InventoryItem GetItem() {
        return cellItem;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHoveredOver = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHoveredOver = false;
    }
}

public interface IInventoryCell {
    public abstract void SetItem(InventoryItem item);
    public abstract InventoryItem GetItem();
}
