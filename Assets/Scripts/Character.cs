using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラの共通処理
public abstract class Character : MonoBehaviour {

    public PlayerController playerController;
    Transform playerTf;
    Transform enemyTf;
    public HitBox[] hitBox = new HitBox[5];
    public HurtBox hurtBox;

    [SerializeField] Material white;
    [SerializeField] Material black;
    [SerializeField] Material red;
    [SerializeField] Material blue;
    [SerializeField] Material clear;

    protected void Start() {
        playerTf = playerController.playerTf;
        enemyTf = playerController.enemyController.characterIns.transform;
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
    public void Damaged(float damage, Vector2 vector, bool isCritical) {
        playerController.hp -= damage;
        playerController.damageVector = new Vector2((enemyTf.position.x < playerTf.position.x) ? vector.x : -vector.x, vector.y);
        if (isCritical) {
            playerController.animator.Play("Critical");
        }
    }

    public void LightningAttack() {
    }
    public void SideA() {
    }
    public void DownA() {
    }
    public void Resistance() {
    }
    public virtual void UpB() {
    }
}
