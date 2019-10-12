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

        //ヒット
        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HurtBox") {
            if (!character.playerController.isLimitBreak) character.playerController.mp += attack * (isCritical ? 0.6f : 0.5f);
            BattleMgr.Instance.CreateVFX("Hit", transform.position,Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("HitWave", transform.position,Quaternion.identity, 1.0f);
        }

        //鍔迫り合い
        if (collision.gameObject.transform.parent != this.transform.parent && collision.gameObject.tag == "HitBox" && isResist && collision.gameObject.GetComponent<HitBox>().isResist) {
            HitBox hitBox = collision.gameObject.GetComponent<HitBox>();
            character.playerController.isResistance = true;
            /*バグ目立つからやめておこう
            if (!character.playerController.animator.GetBool("isLand") && !hitBox.character.playerController.animator.GetBool("isLand")) { //両方空中だと滑る
                character.playerController.resistVector = Random.Range(0, 3.0f) * new Vector3((((character.playerController.enemyTf.position.x < character.playerController.playerTf.position.x && hitBox.vector.x > vector.x)
                || character.playerController.enemyTf.position.x > character.playerController.playerTf.position.x && hitBox.vector.x < vector.x) ? 1 : -1), 0, 0);
            }
            */
            Vector2 v = new Vector2(character.playerController.animator.GetBool("isRight") ? vector.x : -vector.x, vector.y);
            collision.gameObject.GetComponent<HitBox>().character.Resistance(v, attack);
        }
    }
}
