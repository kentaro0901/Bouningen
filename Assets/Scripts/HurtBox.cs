using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour {

    public Character character;
    public bool isUpArmor;
    public bool isSideArmor;
    public bool isDownArmor;

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox" && !character.playerController.isResistance) { //ヒット
            HitBox hitBox = collision.gameObject.GetComponent<HitBox>();
            character.Damaged(hitBox.attack, hitBox.vector, hitBox.isCritical, isUpArmor, isSideArmor, isDownArmor, hitBox.character.playerController.isLimitBreak);
        }
    }
}
