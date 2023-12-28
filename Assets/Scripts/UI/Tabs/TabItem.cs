using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabItem : MonoBehaviour {
    [SerializeField] GameObject selectionMarker;

    public void Select() {
        selectionMarker.SetActive(true);
    }

    public void Deselect() {
        selectionMarker.SetActive(false);
    }
}
