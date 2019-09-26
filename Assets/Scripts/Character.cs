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
    [SerializeField] Material invert;
    [SerializeField] Material redWhite;
    [SerializeField] Material blueBlack;

    protected void Start() {
        playerTf = playerController.playerTf;
        enemyTf = playerController.enemyController.characterIns.transform;
        //プレイヤーカラーの設定
        switch (playerController.playerNum) { //インデックスでアクセスしないほうがよさそう
            case PlayerController.PlayerNum.player1:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = white;
                playerTf.GetChild(6).gameObject.GetComponent<SpriteRenderer>().material = red;
                playerTf.GetChild(7).gameObject.GetComponent<SpriteRenderer>().material = red; break;
            case PlayerController.PlayerNum.player2:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = black;
                playerTf.GetChild(6).gameObject.GetComponent<SpriteRenderer>().material = blue;
                playerTf.GetChild(7).gameObject.GetComponent<SpriteRenderer>().material = blue; break;
            default:
                playerTf.gameObject.GetComponent<SpriteRenderer>().material = white;
                playerTf.GetChild(6).gameObject.GetComponent<SpriteRenderer>().material = red;
                playerTf.GetChild(7).gameObject.GetComponent<SpriteRenderer>().material = red; break;
        }

        //Boxの参照
        hitBox = this.gameObject.GetComponentsInChildren<HitBox>();
        foreach (HitBox _hitbox in hitBox) {
            _hitbox.character = this;
            _hitbox.gameObject.GetComponent<SpriteRenderer>().material = Main.Instance.isVisibleBox ? red : clear;
            _hitbox.gameObject.SetActive(false);
        }   
        hurtBox = this.gameObject.GetComponentInChildren<HurtBox>();
        hurtBox.character = this;
        hurtBox.gameObject.GetComponent<SpriteRenderer>().material = Main.Instance.isVisibleBox ? blue : clear;
    }

    //被ダメージ
    public void Damaged(float damage, Vector2 vector, bool isCritical) {
        if(0 < playerController.hp && playerController.hp - damage <= 0 && !isCritical) {
            playerController.hp = 1.0f;
        }
        else {
            playerController.hp -= damage;
        }
        playerController.mp += damage * (isCritical ? 1.5f :1.2f);
        playerController.damageVector = new Vector2((enemyTf.position.x < playerTf.position.x) ? vector.x : -vector.x, vector.y);
    }
    public void Resistance(Vector2 vector) {
        playerController.damageVector = new Vector2((enemyTf.position.x < playerTf.position.x) ? vector.x : -vector.x, vector.y);
    }
    public void LimitBreak() {
        if(playerController.playerNum == PlayerController.PlayerNum.player1) {
            playerTf.gameObject.GetComponent<SpriteRenderer>().material = redWhite;
        }
        if (playerController.playerNum == PlayerController.PlayerNum.player2) {
            playerTf.gameObject.GetComponent<SpriteRenderer>().material = blueBlack;
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
