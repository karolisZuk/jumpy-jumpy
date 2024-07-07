using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour {
    public enum GameDevice { KeyboardMouse, Gamepad }
    public static PlayerInputs Instance { get; private set; }

    public event EventHandler OnGameDeviceChanged;

    // Menu Events
    public event EventHandler OnMenuNextTab;
    public event EventHandler OnMenuPreviousTab;

    private PlayerInputActions playerInputActions;
    private GameDevice activeGameDevice;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;

        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        playerInputActions = new PlayerInputActions();
        InputSystem.onActionChange += InputSystem_onActionChange;

        playerInputActions.Enable();

        playerInputActions.MenuControls.NextTab.started += Menu_NextTab_started;
        playerInputActions.MenuControls.PreviousTab.started += Menu_PreviousTab_started;
    }

    #region MENU_ACTIONS
    private void Menu_PreviousTab_started(InputAction.CallbackContext obj) {
        OnMenuPreviousTab?.Invoke(this, EventArgs.Empty);
    }

    private void Menu_NextTab_started(InputAction.CallbackContext obj) {
        OnMenuNextTab?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    private void InputSystem_onActionChange(object arg1, InputActionChange inputActionChange) {
        if (inputActionChange == InputActionChange.ActionPerformed && arg1 is InputAction) {
            InputAction inputAction = arg1 as InputAction;

            if (inputAction.activeControl.device.displayName == "VirtualMouse") {
                // Ignore virtual mouse
                return;
            }

            if (inputAction.activeControl.device is Gamepad) {
                if (activeGameDevice != GameDevice.Gamepad) {
                    ChangeActiveGameDevice(GameDevice.Gamepad);
                }
            } else {
                if (activeGameDevice != GameDevice.KeyboardMouse) {
                    ChangeActiveGameDevice(GameDevice.KeyboardMouse);
                }
            }
        }
    }

    private void ChangeActiveGameDevice(GameDevice gameDevice) {
        activeGameDevice = gameDevice;
        Debug.Log("New Active Game Device: " + activeGameDevice);

        Cursor.visible = activeGameDevice == GameDevice.KeyboardMouse;

        OnGameDeviceChanged?.Invoke(this, EventArgs.Empty);
    }

    public GameDevice GetActiveGameDevice() {
        return activeGameDevice;
    }

    public void AssignEquipmentControls(EquipmentSlot slot, IUsable equipment) {
        if (slot == EquipmentSlot.Weapon) {
            playerInputActions.CharacterControls.UseWeapon.started += equipment.Use;
        }
    }

    public void ClearEquipmentControls(EquipmentSlot slot, IUsable equipment) {
        if (slot == EquipmentSlot.Weapon) {
            playerInputActions.CharacterControls.UseWeapon.started -= equipment.Use;
        }
    }

    public PlayerInputActions PlayerInputActions () {
        if(playerInputActions == null) {
            playerInputActions = new PlayerInputActions();
        }

        return playerInputActions;
    }

    private void OnDestroy() {
        InputSystem.onActionChange -= InputSystem_onActionChange;

        playerInputActions.MenuControls.NextTab.started -= Menu_NextTab_started;
        playerInputActions.MenuControls.PreviousTab.started -= Menu_PreviousTab_started;
    }
}
