using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    public Character character;
    public float attack = 0.0f;
    public Vector3 vector = Vector3.zero;

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "HurtBox") {
            Debug.Log("Hit");
            collision.gameObject.GetComponent<HurtBox>().character.Damaged(attack, vector);
        }
    }
}
