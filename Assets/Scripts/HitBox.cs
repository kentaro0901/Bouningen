using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    public Character character;
    public float attack = 0.0f;
    public Vector2 vector = Vector2.zero;
    public bool isCritical = false;
    public bool isResist = false;

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HurtBox") { //ヒット
            //Debug.Log("Hit");
            collision.gameObject.GetComponent<HurtBox>().character.Damaged(attack, vector, isCritical);
        }
        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox" && isResist && collision.gameObject.GetComponent<HitBox>().isResist) { //鍔迫り合い
            //Debug.Log("Resistance");
            if (character.playerController.stateInfo.IsName("SideA")) {
                character.playerController.animator.Play("SideA_R");
            }
            //character.playerController.isResistance = true;
            //character.playerController.animator.Play("SideA_R");
            //character.playerController.animator.SetBool("isResistance", true);
        }
    }
}
