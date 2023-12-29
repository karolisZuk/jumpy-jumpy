using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private TabGroup tabGroup;

    public Image background;

    private void Start() {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Clicked");
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        tabGroup.OnTabExit(this);
    }
}


/**
public class TabItem : MonoBehaviour {
    [SerializeField] GameObject selectionMarker;

    public void Select() {
        selectionMarker.SetActive(true);
    }

    public void Deselect() {
        selectionMarker.SetActive(false);
    }
}

**/
