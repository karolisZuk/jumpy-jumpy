using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class VirtualMouseUI : MonoBehaviour {
    public static VirtualMouseUI Instance { get; private set; }

    [SerializeField] private RectTransform virtualMouse;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private float mouseBorderSize = 5f;
    [SerializeField] private bool shouldCursorAlwaysBeUnlocked;
    [SerializeField] private bool forceDisabled = false;

    private VirtualMouseInput virtualMouseInput;

    private void Awake() {
        Instance = this;
        virtualMouseInput = GetComponent<VirtualMouseInput>();

    }

    private void Start() {
        PlayerInputs.Instance.OnGameDeviceChanged += Instance_OnGameDeviceChanged;

        ResetMouseToCenter();
        UpdateVisibility();
    }

    private void Update() {
        transform.localScale = Vector3.one * (1f / canvasRectTransform.localScale.x);
        transform.SetAsLastSibling();
    }

    private void LateUpdate() {
        Vector2 virtualMousePosition = virtualMouseInput.virtualMouse.position.value;

        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0f + mouseBorderSize, Screen.width - mouseBorderSize);
        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0f + mouseBorderSize, Screen.height - mouseBorderSize);

        InputState.Change(virtualMouseInput.virtualMouse.position, virtualMousePosition);
    }

    private void Instance_OnGameDeviceChanged(object sender, System.EventArgs e) {
        UpdateVisibility();
    }

    private void UpdateVisibility() {
        if((PlayerInputs.Instance.GetActiveGameDevice() == PlayerInputs.GameDevice.Gamepad || shouldCursorAlwaysBeUnlocked) && !forceDisabled) {
            ResetMouseToCenter();
            Show();
        } else {
            Hide();
        }
    }

    private void ResetMouseToCenter() {
        virtualMouse.anchoredPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        try {
            gameObject.SetActive(false);
        } catch (Exception e) {
            Debug.LogError("VirtualMouseUI: " + e);
        }
    }

    private void OnDestroy() {
        PlayerInputs.Instance.OnGameDeviceChanged -= Instance_OnGameDeviceChanged;

    }
}
