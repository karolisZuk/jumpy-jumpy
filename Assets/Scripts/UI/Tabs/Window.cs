using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {
    private bool isActive = false;
    [SerializeField] private GameObject panel;

    public void Show() {
        panel.SetActive(true);
        isActive = true;
    }

    public void Hide() {
        panel.SetActive(false);
        isActive = false;
    }
}
