using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour {

    public Character character;

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox") {

        }
    }
}
