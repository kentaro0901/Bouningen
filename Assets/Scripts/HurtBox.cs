using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour {

    public Character character;

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "HitBox") {

        }
    }
}
