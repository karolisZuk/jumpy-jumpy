using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour {
    public enum GameDevice { KeyboardMouse, Gamepad }
    public static PlayerInputs Instance { get; private set; }

    // TODO: Move all input action subscriptions to this file and fire custom events for each action
    public event EventHandler OnGameDeviceChanged;

    private PlayerInputActions playerInputActions;
    private GameDevice activeGameDevice;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;

        Instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        playerInputActions = new PlayerInputActions();
        InputSystem.onActionChange += InputSystem_onActionChange;

        playerInputActions.Enable();
    }

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

    public PlayerInputActions PlayerInputActions () {
        if(playerInputActions == null) {
            playerInputActions = new PlayerInputActions();
        }

        return playerInputActions;
    }

    private void OnDestroy() {
        InputSystem.onActionChange -= InputSystem_onActionChange;
    }
}
