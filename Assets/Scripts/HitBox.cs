using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    public Character character;
    public float attack = 0.0f;
    public Vector2 vector = Vector2.zero;
    public bool isCritical = false;

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HurtBox") { //ヒット
            Debug.Log("Hit"); //
            if (!character.playerController.isResistance) {//
                collision.gameObject.GetComponent<HurtBox>().character.Damaged(attack, vector, isCritical);
            }
        }
        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox") { //鍔迫り合い
            Debug.Log("Resistance");
            //character.playerController.animator.speed = 0;//
            //character.Resistance();
        }
    }
}
