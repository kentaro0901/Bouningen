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

        //鍔迫り合い
        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox" && isResist && collision.gameObject.GetComponent<HitBox>().isResist) {
            //Debug.Log("Resistance");
            collision.gameObject.GetComponent<HitBox>().character.Resistance(vector);
            if (character.playerController.stateInfo.IsName("SideA")) {
                character.playerController.animator.Play("SideA_R");
            }
            if (character.playerController.stateInfo.IsName("NutralA")) {
                character.playerController.animator.Play("NutralA_R");
            }
        }
    }
}
