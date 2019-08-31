using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public PlayerController playerController;
    Transform playerTf;
    public HitBox hitBox;
    public HurtBox hurtBox;

    public Material white;
    public Material black;

    protected void Start() {
        playerTf = playerController.playerTf;
        switch (playerController.playerNum) {
            case PlayerController.PlayerNum.player1:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = white; break;
            case PlayerController.PlayerNum.player2:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = black; break;
            default:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = white; break;
        }
        
        hitBox = this.gameObject.GetComponentInChildren<HitBox>();
        hitBox.character = this;
        hurtBox = this.gameObject.GetComponentInChildren<HurtBox>();
        hurtBox.character = this;
    }

    public void Damaged(float damage, Vector3 vector) {
        playerController.hp -= damage;
        playerController.damageVector = vector;
        if (playerController.isDamaged == false) {
            playerController.isDamaged = true;
        }
        if (damage >= 5.0f && playerController.isCriticaled == false) {
            playerController.isCriticaled = true;
        }
    }

    public void LightningAttack() {
        hitBox.attack = 5.0f;
        hitBox.vector = playerTf.localScale.x > 0 ? Vector3.right: Vector3.left;
    }
    public void SideA() {
        hitBox.attack = 5.0f;
        hitBox.vector = playerTf.localScale.x > 0 ? Vector3.right : Vector3.left;
    }
    public virtual void UpB() {
    }
}
