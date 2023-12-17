using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionPlane : MonoBehaviour {
    void Start() {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other != null && other.CompareTag("Player")) {
            Transform t = other.GetComponent<Transform>();
            CharacterController cc = other.GetComponent<CharacterController>();

            cc.enabled = false;
            t.position = new Vector3(0, 1f, 0);
            cc.enabled = true;
        }
    }
}
