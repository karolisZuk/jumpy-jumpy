using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TabGroup : MonoBehaviour {
    public static event EventHandler<int> OnMainMenuTabChange;

    [SerializeField] private List<TabItem> tabItems = new List<TabItem>();
    [SerializeField] private int preselectedTab = 0;

    PlayerInputActions playerInputActions;

    private int currentSelectionIndex = 0;

    private void Awake() {
        currentSelectionIndex = preselectedTab;

        if (currentSelectionIndex > tabItems.Count - 1) {
            Debug.LogError("TabGroup: preselected tab does not exist");
            return;
        }

        if (tabItems.Count > 0) {
            foreach(TabItem tab in tabItems) {
                tab.Deselect();
            }
        }
    }

    private void Start() {
        tabItems[currentSelectionIndex].Select();
        OnMainMenuTabChange?.Invoke(this, currentSelectionIndex);

        playerInputActions = PlayerInputs.Instance.PlayerInputActions();

        playerInputActions.MenuControls.NextTab.started += NextTab_started;
        playerInputActions.MenuControls.PreviousTab.started += PreviousTab_started;
    }

    private void PreviousTab_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (currentSelectionIndex > 0) {
            tabItems[currentSelectionIndex].Deselect();
            currentSelectionIndex -= 1;
            tabItems[currentSelectionIndex].Select();
        } else {
            tabItems[currentSelectionIndex].Deselect();
            currentSelectionIndex = tabItems.Count - 1;
            tabItems[currentSelectionIndex].Select();
        }

        OnMainMenuTabChange?.Invoke(this, currentSelectionIndex);
    }

    private void NextTab_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (currentSelectionIndex < tabItems.Count - 1) {
            tabItems[currentSelectionIndex].Deselect();
            currentSelectionIndex += 1;
            tabItems[currentSelectionIndex].Select();
        } else {
            tabItems[currentSelectionIndex].Deselect();
            currentSelectionIndex = 0;
            tabItems[currentSelectionIndex].Select();
        }

        OnMainMenuTabChange?.Invoke(this, currentSelectionIndex);
    }

    private void OnDisable() {
        playerInputActions.MenuControls.Disable();
    }
}
