using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラの共通処理
public abstract class Character : MonoBehaviour {

    public PlayerController controller;
    protected Transform playerTf;
    protected Transform enemyTf;
    public HitBox[] hitBox = new HitBox[5];
    public HurtBox hurtBox;
    protected Animator animator;
    public int counter;
    public Status status;

    public int prestatenum = 0;
    private bool preResistButtonDown = false;

    [SerializeField] Material white;
    [SerializeField] Material black;
    [SerializeField] Material red;
    [SerializeField] Material blue;
    [SerializeField] Material clear;
    [SerializeField] Material redWhite;
    [SerializeField] Material blueBlack;

    protected void Start() {
        playerTf = controller.playerTf;
        enemyTf = controller.enemyController.playerTf;
        animator = gameObject.GetComponent<Animator>();
        counter = 0;

        //プレイヤーカラーの設定
        Material main;
        Material sub;
        switch (controller.playerNum) { //インデックスでアクセスしないほうがよさそう
            case PlayerController.PlayerNum.player1: (main, sub) = (white, red); break;
            case PlayerController.PlayerNum.player2: (main, sub) = (black, blue); break;
            default: (main, sub) = (white, red); break;
        }
        playerTf.gameObject.GetComponent<SpriteRenderer>().material = main;
        playerTf.GetChild(6).gameObject.GetComponent<SpriteRenderer>().material = sub;
        playerTf.GetChild(7).gameObject.GetComponent<SpriteRenderer>().material = sub;

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
        if (0 < controller.hp && controller.hp - damage * (isLimitBreak ? 1.2f : 1.0f) <= 0 && (!isCritical || !(vector.y == 0 && !isSideArmor))) { //横クリティカル以外ではゲームが終了しない
            controller.hp = 1.0f;
        }
        else {
            controller.hp -= damage * (isLimitBreak ? 1.2f : 1.0f);
        }
        if (!controller.isLimitBreak) controller.mp += damage * (isCritical ? 1.2f :1.0f);
        controller.damageVector = vector;
        if (isCritical) { //クリティカル
            if (vector.y == 0 && !isSideArmor) { //横クリティカル
                counter = 0;
                controller.animator.Play("Critical");
            }
            else if (vector.y > 0 && !isUpArmor){ //上クリティカル
                counter = 0;
                controller.animator.Play("CriticalUp");
            }
            else if (vector.y < 0 && !isDownArmor){ //下クリティカル
                counter = 0;
                controller.animator.Play("CriticalDown");
            }
        }
        else {
            controller.damageVector = Vector3.zero;
        }
    }
    public void StartResistance(Vector2 vector, float damage) {
        controller.damageVector = vector;
        controller.resistDamage = damage;
        prestatenum = 13;
        if (controller.stateInfo.fullPathHash == AnimState.SideA ||
            controller.stateInfo.fullPathHash == AnimState.LightningAttack) {
            controller.animator.Play("SideA_R");
        }
        else if (controller.stateInfo.fullPathHash == AnimState.SideB ||
            controller.stateInfo.fullPathHash == AnimState.SideB_Air ||
            controller.stateInfo.fullPathHash == AnimState.UpB_Fall) {
            controller.animator.Play("SideB_R");
        }
        else if (controller.stateInfo.fullPathHash == AnimState.NutralA ||
            controller.stateInfo.fullPathHash == AnimState.CriticalNA) {
            controller.animator.Play("NutralA_R");
        }
        else if (controller.stateInfo.fullPathHash == AnimState.SideA_Air) {
            controller.animator.Play("SideA_Air_R");
        }
        else if (controller.stateInfo.fullPathHash == AnimState.NutralA_Air) {
            controller.animator.Play("NutralA_Air_R");
        }
        else {
            controller.animator.Play("SideA_R");
            Debug.Log("NoneR");
        }
    }

    public void StartGame() {
        if (counter == 0) {
            BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateVFX("Line", playerTf.position, Quaternion.identity, 0.1f);
            animator.speed = Main.Instance.gameSpeed;
        }
    }
    public void Idle() {
        if (animator.GetBool(AnimState.ButtonL) && !controller.isLimitBreak && 100 <= controller.mp) {
            animator.Play("LimitBreak");
            controller.mp = 0;
        }
    }
    public void Dash() {
        if(counter == 0) {
            prestatenum = 0;
        }        
        playerTf.position += Vector3.right * status.dashspeed * controller.input.xAxis * animator.speed;
    }
    public void JumpStart() {
        if(counter == 0) {
            prestatenum = 1;
        }
    }
    public void Fall() {
        playerTf.position += Vector3.down * counter * 0.1f * animator.speed;
        if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
    }
    public void Landing() {
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
    }
    public void NutralA() {
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
            prestatenum = 3;
        }
    }
    public void NutralA_Air() {
        if(counter == 0) {
            prestatenum = 3;
        }    
    }
    public void LightningStart() {
        if (counter == 0) {
            prestatenum = 2;
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
        }
    }
    public void Lightning() {
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.8f, 1.0f);
            BattleMgr.Instance.CreateVFX("HitWave", playerTf.position, Quaternion.identity, 1.0f);
            GameObject g = BattleMgr.Instance.CreateVFX("LandKick", playerTf.position, Quaternion.identity, 1.0f);
            if (0 < playerTf.localScale.x)
                g.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (counter <= 5) {
            BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            Vector3 lightningPos = 5 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right);
            Vector3 ofs = Vector3.zero;
            if (controller.input.xAxis > 0)
                ofs = 2 * Vector3.right;
            else if (controller.input.xAxis < 0)
                ofs = 2 * Vector3.left;
            if (controller.input.yAxis > 0)
                ofs = 7 * Vector3.up;
            else if (controller.input.yAxis < 0)
                ofs = 7 * Vector3.down;
            playerTf.position += (enemyTf.position + lightningPos + ofs - playerTf.position) / (counter == 5 ? 1 : 2);
        }
        else
            playerTf.position += new Vector3(controller.enemyController.damageVector.x, 0, 0);
    }
    public void LightningAttack() {
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
        }
        playerTf.position += new Vector3(controller.enemyController.damageVector.x * Time.timeScale, 0, 0);
    }
    public void LightningEnd() {
        playerTf.position += new Vector3(controller.enemyController.damageVector.x * Time.timeScale, 0, 0);
    }
    public void SideA() {
        if (counter == 0) {
            StartCoroutine("SideACoroutine");
            prestatenum = 5;
        }
    }
    public void SideA_Air() {
        if (counter == 0) {
            StartCoroutine("SideACoroutine");
            prestatenum = 5;
        }
    }
    IEnumerator SideACoroutine() {
        float speed = controller.animator.speed;
        yield return new WaitForSeconds(10.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
        BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, Quaternion.identity, 1.0f);
        yield return new WaitForSeconds(8.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), playerTf.position.y + 1, 0), Quaternion.Euler(0, 0, (playerTf.localScale.x > 0 ? 1 : -1) * 90), 1.0f);
        yield return new WaitForSeconds(8.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), playerTf.position.y + 1, 0), Quaternion.Euler(0, 0, (playerTf.localScale.x > 0 ? 1 : -1) * 90), 1.0f);
        yield return 0;
    }
    public void DownA() {
        if (counter == 0) {
            StartCoroutine("DownACoroutine");
            prestatenum = 7;
        }
    }
    public void DownA_Air() {
        if (counter == 0) {
            StartCoroutine("DownACoroutine");
            prestatenum = 6;
        }
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && playerTf.position.y != 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
    }
    IEnumerator DownACoroutine() {
        float speed = controller.animator.speed;
        yield return new WaitForSeconds(14.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("WhiteStone", playerTf.position, Quaternion.identity, 1.0f);
        BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position, Quaternion.identity, 1.0f);
        BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        yield return 0;
    }
    public void UpA() {
        if (counter == 0) {
            StartCoroutine("UpACoroutine");
            prestatenum = 9;
        }
    }
    IEnumerator UpACoroutine() {
        float speed = controller.animator.speed;
        BattleMgr.Instance.VibrateDouble(0.3f, 0.5f);
        yield return new WaitForSeconds(11.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, Quaternion.identity, 1.0f);
        yield return new WaitForSeconds(2.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position + Vector3.up*3, Quaternion.Euler(0,0,180), 1.0f);
        yield return new WaitForSeconds(4.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position + Vector3.up * 3, Quaternion.Euler(0, 0, 180), 1.0f);
        yield return new WaitForSeconds(4.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position + Vector3.up * 3, Quaternion.Euler(0, 0, 180), 1.0f);
        yield return 0;
    }
    public void LimitBreak() {
        if (counter == 0) {
            controller.isLimitBreak = true;
            StartCoroutine(LimitBreakCoroutine());
            prestatenum = 11;
        }
    }
    IEnumerator LimitBreakCoroutine() {
        float speed = controller.animator.speed;
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
        BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
        playerTf.position = new Vector3(playerTf.position.x, 0, playerTf.position.z);
        animator.speed *= 1.2f;
        if (controller.playerNum == PlayerController.PlayerNum.player1) {
            playerTf.gameObject.GetComponent<SpriteRenderer>().material = redWhite;
        }
        if (controller.playerNum == PlayerController.PlayerNum.player2) {
            playerTf.gameObject.GetComponent<SpriteRenderer>().material = blueBlack;
        }
    }
    public void Critical() {
        if (counter == 0) {
            EndCoroutine();
            playerTf.localScale = controller.damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
            BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + new Vector3(enemyTf.position.x < playerTf.position.x ? 3 : -3, 1, 0), Quaternion.identity, 1.0f);
            if (controller.hp < controller.maxhp * 0.25f)
                BattleMgr.Instance.CreateCrack(controller.damageVector.x < 0);
            if (controller.hp <= 0) {
                Main.battleResult = Main.BattleResult.Finish;
                BattleMgr.Instance.ChangeTimeScale(0.02f, 0.8f);
                BattleMgr.Instance.ChangeToneDouble(0.8f, ((int)controller.playerNum == 2 ? CameraEffect.ToneName.blackRed : CameraEffect.ToneName.blackBlue));
            }
            else {
                BattleMgr.Instance.ChangeTimeScale(0.06f, 0.4f);
                BattleMgr.Instance.ChangeToneDouble(0.4f, ((int)controller.playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
            }
            BattleMgr.Instance.ZoomInOutDouble(0.1f);
            prestatenum = 12;
        }
        playerTf.position += controller.damageVector * Time.timeScale;
        playerTf.position += Vector3.down * counter * 0.002f * animator.speed;
        if (playerTf.position.y < 0.1f && 60 < counter * animator.speed) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            if (controller.hp <= 0)
                animator.Play("Death");
            else
                animator.Play("CriticalEnd");
        }
    }
    public void CriticalUp() {
        if (counter == 0) {
            EndCoroutine();
            BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
        }
        playerTf.position += new Vector3(controller.damageVector.x, 0, 0);
    }
    public void CriticalFall() {
        if(counter == 0) {
            EndCoroutine();
        }
        playerTf.position += new Vector3(controller.damageVector.x, -counter * 0.1f * animator.speed, 0);
        if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            if (Mathf.Abs( controller.damageVector.x) < 0.2f) {
                BattleMgr.Instance.VibrateDouble(0.5f, 0.3f);
                animator.Play("CriticalFallEnd");
            }
            else {
                BattleMgr.Instance.VibrateDouble(0.5f, 0.7f);
                animator.Play("CriticalEnd");
            }
        }
    }
    public void CriticalDown() {
        if (counter == 0) {
            BattleMgr.Instance.ChangeToneDouble(0.1f, ((int)controller.playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
            BattleMgr.Instance.CreateVFX("CriticalDownWave", playerTf.position + Vector3.up * 2.0f, Quaternion.identity, 1.0f);
        }
        playerTf.position = new Vector3(playerTf.position.x + controller.damageVector.x, playerTf.position.y + controller.damageVector.y, 0) * animator.speed;
        if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.CreateVFX("LandWave", playerTf.position, Quaternion.identity, 1.0f);
            BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
            animator.Play("CriticalUp");
        }
    }
    public void CriticalEnd() {
        controller.damageVector *= 0.9f;
        playerTf.position += new Vector3(controller.damageVector.x, 0, 0);
        if (Mathf.Abs(controller.damageVector.x) < 0.1f && 20 <= counter) {
            controller.damageVector = Vector3.zero;
            animator.Play("Idle");
        }
    }
    public void Wince() {
        playerTf.position += new Vector3(controller.damageVector.x, -counter * 0.1f * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
        }
    }
    public void Resistance() {
        if (counter == 0 && controller.playerNum == PlayerController.PlayerNum.player1) {
            BattleMgr.Instance.StartResistance();
        }
        if (controller.input.A || controller.input.B) {
            if (!preResistButtonDown) {
                if (controller.playerNum == PlayerController.PlayerNum.player1) {
                    BattleMgr.Instance.resistCounter1P++;
                }
                if (controller.playerNum == PlayerController.PlayerNum.player2)
                    BattleMgr.Instance.resistCounter2P++;
            }
            preResistButtonDown = true;
        }
        else {
            preResistButtonDown = false;
        }
        if (counter == 20) {
            BattleMgr.Instance.VibrateDouble(0.3f, 2.0f);
            BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("Line", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-70, 70)), 1.0f);
        }
        if (counter == 40) {
            BattleMgr.Instance.VibrateDouble(0.4f, 2.0f);
            BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("Line", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-70, 70)), 1.0f);
        }
        if (counter == 60) {
            controller.isResistance = false;
            if ((BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical1P && controller.playerNum == PlayerController.PlayerNum.player1) ||
                (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical2P && controller.playerNum == PlayerController.PlayerNum.player2)) { //鍔迫り合いに負けた時
                BattleMgr.Instance.CreateVFX("Hit", playerTf.position, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("HitWave", playerTf.position, Quaternion.identity, 1.0f);
                controller.hp -= controller.resistDamage * 1.5f;
                controller.mp += controller.resistDamage;
                controller.resistDamage = 0;
                animator.Play("Critical");
            }
            else if (BattleMgr.Instance.resistResult != BattleMgr.ResistResult.Wince) { //勝った時
                controller.damageVector = Vector3.zero;
                controller.mp += controller.resistDamage * 0.5f;
                controller.resistDamage = 0;
                if (controller.stateInfo.fullPathHash == AnimState.NutralA_R)
                    animator.Play("NutralA_RW");
                else if (controller.stateInfo.fullPathHash == AnimState.NutralA_Air_R)
                    animator.Play("NutralA_Air_RW");
                else if (controller.stateInfo.fullPathHash == AnimState.SideA_R)
                    animator.Play("SideA_RW");
                else if (controller.stateInfo.fullPathHash == AnimState.SideA_Air_R)
                    animator.Play("SideA_Air_RW");
                else if (controller.stateInfo.fullPathHash == AnimState.SideB_R)
                    animator.Play("SideB_RW");
                else if (controller.stateInfo.fullPathHash == AnimState.SideB_Air_R)
                    animator.Play("SideB_Air_RW");
                else {
                    animator.Play("SideA_RW");
                    Debug.Log("NoneRW");
                }
            }
            else {
                BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone); //引き分け
                controller.resistDamage = 0;
                controller.damageVector = Vector3.zero;
                animator.Play("Wince");
            }
        }
    }
    public void GameEnd() { //負けた方が発動
        if (counter == 0) {
            BattleMgr.Instance.BattleEnd();
        }
    }

    public void GroundCollision() {
        playerTf.position = new Vector3(playerTf.position.x, 0, 0);
    }
    public void AirControll() {
        playerTf.position += Vector3.right * status.airspeed * controller.input.xAxis * animator.speed;
    }
    public void AutoInvert() {
        playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
    }
    public void ManualInvert() {
        if (controller.input.xAxis > 0) playerTf.localScale = new Vector3(1, 1, 1);
        if (controller.input.xAxis < 0) playerTf.localScale = new Vector3(-1, 1, 1);
    }

    public virtual void NutralB() { 
        if(counter == 0) {
            prestatenum = 4;
        }    
    }
    public virtual void NutralB_Air() {
        if(counter == 0) {
            prestatenum = 4;
        }    
    }
    public virtual void SideB() { 
        if(counter == 0) {
            prestatenum = 6;
        }    
    }
    public virtual void SideB_Air() {
        if (counter == 0) {
            prestatenum = 6;
        }
    }
    public virtual void UpB() { 
        if(counter == 0) {
            prestatenum = 10;
        }    
    }
    public virtual void UpB_Fall() { }
    public virtual void DownB() { 
        if(counter ==0) {
            prestatenum = 8;
        }    
    }
    public virtual void DownB_Fall() { }
    public virtual void DownB_Air() { 
        if(counter == 0) {
            prestatenum = 8;
        }    
    }
    public virtual void DownB_Air_Fall() { }

    protected virtual void EndCoroutine() {
        StopCoroutine("SideACoroutine");
        StopCoroutine("DownACoroutine");
        StopCoroutine("UpACoroutine");
    }
}
