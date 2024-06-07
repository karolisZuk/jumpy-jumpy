using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionPlane : MonoBehaviour {
    void Start() {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other != null && (other.CompareTag("Player") || other.CompareTag("Respawnable"))) {
            Transform t = other.GetComponent<Transform>();

            if (other.CompareTag("Player")) {
                CharacterController cc = other.GetComponent<CharacterController>();
                cc.enabled = false;
                t.position = new Vector3(0, 1f, 0);
                cc.enabled = true;
            } else {
                t.position = new Vector3(0, 1f, 0);

            }

        }
    }
}
