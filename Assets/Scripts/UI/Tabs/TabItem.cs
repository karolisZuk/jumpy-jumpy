using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class TabItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private TabGroup tabGroup;

    public Image background;
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    private PlayerInputActions controls;

    private void Start() {
        background = GetComponent<Image>();
        controls = PlayerInputs.Instance.PlayerInputActions();

        controls.MenuControls.Interact.performed += Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext obj) {
        Vector2 position = Pointer.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);

        if (PointerIsUIHit(position)) {
            // Mouse hit the button
            tabGroup.OnTabSelected(this);
        }
    }

    private bool PointerIsUIHit(Vector2 position) {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = position;
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        // UI Elements must have `picking mode` set to `position` to be hit
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0) {
            foreach (RaycastResult result in raycastResults) {
                if (result.distance == 0 && result.isValid && (result.gameObject.name == gameObject.name || result.gameObject.transform.parent.name == gameObject.name)) {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        tabGroup.OnTabExit(this);
    }

    public void Select() {
        if (onTabSelected != null) {
            onTabSelected.Invoke();
        }
    }

    public void Deselect() {
        if (onTabDeselected != null) {
            onTabDeselected.Invoke();
        }
    }
}