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
    [SerializeField] Material invert;
    [SerializeField] Material redWhite;
    [SerializeField] Material blueBlack;

    protected void Start() {
        playerTf = playerController.playerTf;

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
    public void Damaged(float damage, Vector2 vector, bool isCritical, bool isUpArmor, bool isSideArmor, bool isDownArmor, bool isLimitBreak) {
        if (0 < playerController.hp && playerController.hp - damage * (isLimitBreak ? 1.2f : 1.0f) <= 0 && (!isCritical || !(vector.y == 0 && !isSideArmor))) { //横クリティカル以外ではゲームが終了しない
            playerController.hp = 1.0f;
        }
        else {
            playerController.hp -= damage * (isLimitBreak ? 1.2f : 1.0f);
        }
        if (!playerController.isLimitBreak) playerController.mp += damage * (isCritical ? 1.2f :1.0f);
        playerController.damageVector = vector;
        if (isCritical) { //クリティカル
            if (vector.y == 0 && !isSideArmor) { //横クリティカル
                playerController.counter = 0;
                playerController.animator.Play("Critical");
            }
            else if (vector.y > 0 && !isUpArmor){ //上クリティカル
                playerController.counter = 0;
                playerController.animator.Play("CriticalUp");
            }
            else if (vector.y < 0 && !isDownArmor){ //下クリティカル
                playerController.counter = 0;
                playerController.animator.Play("CriticalDown");
            }
        }
        else {
            playerController.damageVector = Vector3.zero;
        }
    }
    public void Resistance(Vector2 vector, float damage) {
        playerController.damageVector = vector;
        playerController.resistDamage = damage;
        if (playerController.stateInfo.fullPathHash == AnimState.Instance.SideA ||
            playerController.stateInfo.fullPathHash == AnimState.Instance.LightningAttack) {
            playerController.animator.Play("SideA_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.Instance.SideB) {
            playerController.animator.Play("SideB_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.Instance.NutralA ||
            playerController.stateInfo.fullPathHash == AnimState.Instance.CriticalNA) {
            playerController.animator.Play("NutralA_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.Instance.SideA_Air) {
            playerController.animator.Play("SideA_Air_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.Instance.NutralA_Air) {
            playerController.animator.Play("NutralA_Air_R");
        }
        else { //なぜかたまにここに入る
            playerController.animator.Play("SideA_R");
            Debug.LogError("NoneResist");
        }
    }
    public IEnumerator LimitBreak() {
        float speed = playerController.animator.speed;
        BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
        BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        yield return new WaitForSeconds(20.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
        yield return new WaitForSeconds(20.0f / 60 / speed);
        BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
        yield return new WaitForSeconds(20.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(2.0f, 2.0f);
        BattleMgr.Instance.ChangeToneDouble(1.0f, CameraEffect.ToneName.whiteWhite);
        yield return new WaitForSeconds(10.0f / 60 / speed);
        BattleMgr.Instance.ChangeToneDouble(3.0f, CameraEffect.ToneName.reverseTone);
        yield return new WaitForSeconds(55.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
        yield return 0;
    }
    public void LimitBreakEnd() {
        if (playerController.playerNum == PlayerController.PlayerNum.player1) {
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
    public IEnumerator DownA() {
        float speed = playerController.animator.speed;
        yield return new WaitForSeconds(14.0f / 60 / speed);
        Instantiate(playerController.HibiPref[Random.Range(0, playerController.HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
        BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        yield return 0;
    }
    public IEnumerator Resistance() {
        yield return 0;
    }
    public virtual IEnumerator UpB_Fall() {
        yield return 0;
    }
}
