using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラの共通処理
public abstract class Character : MonoBehaviour {

    public PlayerController playerController;
    Transform playerTf;
    public HitBox[] hitBox = new HitBox[5];
    public HurtBox hurtBox;

    [SerializeField] Material white;
    [SerializeField] Material black;
    [SerializeField] Material red;
    [SerializeField] Material blue;
    [SerializeField] Material clear;

    protected void Start() {
        playerTf = playerController.playerTf;
        //プレイヤーカラーの設定
        switch (playerController.playerNum) { //インデックスでアクセスしないほうがよさそう
            case PlayerController.PlayerNum.player1:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = white;
                playerTf.GetChild(6).gameObject.GetComponent<SpriteRenderer>().material = red; break;
            case PlayerController.PlayerNum.player2:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = black;
                playerTf.GetChild(6).gameObject.GetComponent<SpriteRenderer>().material = blue; break;
            default:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = white;
                playerTf.GetChild(6).gameObject.GetComponent<SpriteRenderer>().material = red; break;
        }

        //Boxの参照
        hitBox = this.gameObject.GetComponentsInChildren<HitBox>();
        foreach (HitBox _hitbox in hitBox) {
            _hitbox.character = this;
            _hitbox.gameObject.GetComponent<SpriteRenderer>().material = playerController.isVisibleBox ? red : clear;
            _hitbox.gameObject.SetActive(false);
        }   
        hurtBox = this.gameObject.GetComponentInChildren<HurtBox>();
        hurtBox.character = this;
        hurtBox.gameObject.GetComponent<SpriteRenderer>().material = playerController.isVisibleBox ? blue : clear;
    }

    //被ダメージ
    public void Damaged(float damage, Vector3 vector, bool isCritical) {
        if (!playerController.isResistance) {
            playerController.hp -= damage;
            playerController.damageVector = vector;
            if (playerController.isDamaged == false) {
                playerController.isDamaged = true;
            }
            if (isCritical && playerController.isCriticaled == false) {
                playerController.isCriticaled = true;
            }
        }   
    }

    public void LightningAttack() {
        foreach (HitBox _hitbox in hitBox) {
            _hitbox.attack = 3.0f;
            _hitbox.vector = (playerTf.localScale.x > 0 ? Vector3.right : Vector3.left) * 3;
        }
    }
    public void SideA() {
        foreach (HitBox _hitbox in hitBox) {
            _hitbox.attack = 5.0f;
            _hitbox.vector = (playerTf.localScale.x > 0 ? Vector3.right : Vector3.left) * 3;
        }
    }
    public void DownA() {
        foreach (HitBox _hitbox in hitBox) {
            _hitbox.attack = 5.0f;
            _hitbox.vector = (playerTf.localScale.x > 0 ? Vector3.right : Vector3.left) *2;
        }
    }
    public void Resistance() {
        //playerController.isResistance = true;
    }
    public virtual void UpB() {
    }
}
