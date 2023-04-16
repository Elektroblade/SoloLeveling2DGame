using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform pfDamagePopup;

    private void Start() {
        //DamagePopup.Create(Vector3.zero, 300);
    }

    private void Update() {
        if (Input.GetButton("Dodge")) {
            bool isCriticalHit = Random.Range(0, 100) < 30;
            //DamagePopup.Create(Vector3.zero, 100, isCriticalHit);
        }
    }
}
