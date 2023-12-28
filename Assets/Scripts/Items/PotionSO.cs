using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Items/Consumable/Potion")]
public class PotionSO : Consumable {
    [SerializeField] protected string animationName;
    [SerializeField] int amount;

    public override void Use() {
        base.Use();

        // TODO: update
        Debug.Log("Gulp Gulp");

    }
}
