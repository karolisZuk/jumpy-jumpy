using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour {
    [SerializeField] private float delay;

    private float timer = 0;

    private void Update() {
        if(timer < delay) {
            timer += Time.deltaTime;
        } else {
            Destroy(gameObject);
        }
    }
}
