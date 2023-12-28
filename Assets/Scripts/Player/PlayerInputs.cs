using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour {
    private PlayerInputActions playerInputActions;
    public static PlayerInputs Instance { get; private set; }


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }

        playerInputActions = new PlayerInputActions();
    }

    public PlayerInputActions PlayerInputActions () {
        if(playerInputActions == null) {
            playerInputActions = new PlayerInputActions();
        }

        return playerInputActions;
    }
}
