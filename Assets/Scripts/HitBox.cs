﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    public Character character;
    public float attack = 0.0f;
    public Vector2 vector = Vector2.zero;
    public bool isCritical = false;
    public bool isResist = false;

    void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HurtBox") { //ヒット
            character.playerController.mp += attack * (isCritical ? 1.2f : 0.8f);
        }

        //鍔迫り合い
        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox" && isResist && collision.gameObject.GetComponent<HitBox>().isResist) {
            character.playerController.isResistance = true;
            collision.gameObject.GetComponent<HitBox>().character.Resistance(vector);
            if (character.playerController.stateInfo.IsName("SideA")) {
                character.playerController.animator.Play("SideA_R");
            }
            if (character.playerController.stateInfo.IsName("SideB")) {
                character.playerController.animator.Play("SideB_R");
            }
            if (character.playerController.stateInfo.IsName("NutralA")) {
                character.playerController.animator.Play("NutralA_R");
            }
        }
    }
}
