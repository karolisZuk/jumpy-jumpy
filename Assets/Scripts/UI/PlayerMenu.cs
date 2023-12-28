using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerMenu : MonoBehaviour {
    public static event EventHandler OnMenuClose;

    [SerializeField] private Canvas menuCanvas;

    PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = PlayerInputs.Instance.PlayerInputActions();
        playerInputActions.MenuControls.Disable();

        EquipmentController.OnMenuOpen += EquipmentController_OnMenuOpen;
        playerInputActions.MenuControls.HideMenu.started += HideMenu;
        playerInputActions.MenuControls.HideMenu.canceled += HideMenu;
    }

    private void HideMenu(InputAction.CallbackContext obj) {
        menuCanvas.enabled = false;
        playerInputActions.MenuControls.Disable();
        OnMenuClose?.Invoke(this, EventArgs.Empty);
    }

    private void EquipmentController_OnMenuOpen(object sender, System.EventArgs e) {
        playerInputActions.MenuControls.Enable();
        menuCanvas.enabled = true;
    }

    private void Start() {
        menuCanvas.enabled = false;
    }
}
