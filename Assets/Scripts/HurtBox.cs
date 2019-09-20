using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour {

    public Character character;

    void OnTriggerEnter2D(Collider2D collision) { //ヒット
        if(collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox") { //ダメージ
            character.Damaged(collision.gameObject.GetComponent<HitBox>().attack, collision.gameObject.GetComponent<HitBox>().vector);
            if (collision.gameObject.GetComponent<HitBox>().isCritical) { //クリティカル
                BattleMgr.Instance.ChangeTimeScale(0.0f, 0.5f);
                character.playerController.counter = 0;
                if (collision.gameObject.GetComponent<HitBox>().vector.y <= collision.gameObject.GetComponent<HitBox>().vector.x) {
                    character.playerController.animator.Play("Critical");
                }
                else {
                    character.playerController.animator.Play("CriticalFall");
                }
            }
        }
    }
}
