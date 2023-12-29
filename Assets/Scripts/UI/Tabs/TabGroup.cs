using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class TabGroup: MonoBehaviour {
    [SerializeField] private List<GameObject> objectsToSwap;
    [SerializeField] private List<TabItem> tabButtons;
    [SerializeField] private TabItem selectedTab;

    [Header("UI")]
    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabHover;
    [SerializeField] private Sprite tabActive;

    private void Awake() {
        PlayerInputs.Instance.OnMenuNextTab += PlayerInputs_OnMenuNextTab;
        PlayerInputs.Instance.OnMenuPreviousTab += PlayerInputs_OnMenuPreviousTab;
    }

    private void Start() {
        if (selectedTab != null) {
            StartCoroutine(Preselect(selectedTab));
        }
    }

    IEnumerator Preselect (TabItem selectedTab) {
        yield return new WaitForSeconds(1);
        OnTabSelected(selectedTab);
    }

    private void PlayerInputs_OnMenuNextTab(object sender, EventArgs e) {
        if (selectedTab == null) {
            OnTabSelected(tabButtons[0]);
            return;
        }

        int currentIndex = selectedTab.transform.GetSiblingIndex();

        if (currentIndex < tabButtons.Count - 1) {
            OnTabSelected(tabButtons[currentIndex + 1]);
        } else {
            OnTabSelected(tabButtons[0]);
        }
    }

    private void PlayerInputs_OnMenuPreviousTab(object sender, EventArgs e) {
        if (selectedTab == null) {
            OnTabSelected(tabButtons[0]);
            return;
        }

        int currentIndex = selectedTab.transform.GetSiblingIndex();

        if (currentIndex > 0) {
            OnTabSelected(tabButtons[currentIndex - 1]);
        } else {
            OnTabSelected(tabButtons[tabButtons.Count - 1]);
        }
    }

    public void OnTabEnter(TabItem button) {
        ResetTabs();

        if (selectedTab == null || button != selectedTab) {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabItem button) {
        ResetTabs();
    }

    public void OnTabSelected(TabItem button) {
        if (selectedTab != null) {
            selectedTab.Deselect();
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();

        for(int i = 0; i < objectsToSwap.Count; i++) {
            if (i == index) {
                objectsToSwap[i].SetActive(true);
            } else {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs() {
        foreach(TabItem button in tabButtons) {
            if (selectedTab != null && button == selectedTab) continue;
            button.background.sprite = tabIdle;
        }
    }
}