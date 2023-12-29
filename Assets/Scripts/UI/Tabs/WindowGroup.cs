using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowGroup : MonoBehaviour {
    [SerializeField] private List<Window> windows = new List<Window>();

    private int currentSelectionIndex = 0;

    private void Awake() {
        if (windows.Count > 0) {
            foreach (Window window in windows) {
                window.Hide();
            }
        }

        // TODO: Might remove later
        //TabGroup.OnMainMenuTabChange += TabGroup_OnMainMenuTabChange;
    }

    private void TabGroup_OnMainMenuTabChange(object sender, int index) {
        windows[currentSelectionIndex].Hide();
        currentSelectionIndex = index;
        windows[currentSelectionIndex].Show();

    }
}
