using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Weapon : MonoBehaviour, IUsable {
    private bool isBusy;
    private string animationName;
    private Animator animator;
    private int damage;
    private bool canPush;
    private float cooldown;
    private EquipmentSlot slot;

    private float cooldownTimer = 0;

    public void Init(Animator anim, string animationName, int damage, bool canPush, float cooldown, EquipmentSlot slot) {
        animator = anim;
        this.animationName = animationName;
        this.damage = damage;
        this.canPush = canPush;
        this.cooldown = cooldown;
        this.slot = slot;

        PlayerInputs.Instance.AssignEquipmentControls(slot, this);
        isBusy = false;
    }

    void Update() {
        if(isBusy && cooldownTimer <= cooldown) {
            cooldownTimer += Time.deltaTime;
        } else {
            isBusy = false;
            cooldownTimer = 0;
        }
    }

    public void Use(InputAction.CallbackContext obj) {
        if (isBusy) return;

        isBusy = true;
        Debug.Log("TODO: Using " + name);
        // TODO: Mirror animatino based on hand
        // TODO: Enable and disable damage collider
        animator.SetTrigger(animationName);
    }

    private void OnDestroy() {
        PlayerInputs.Instance.ClearEquipmentControls(slot, this);
    }
}

public interface IUsable {
    public abstract void Use(InputAction.CallbackContext obj);
}
