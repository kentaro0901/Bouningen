using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static AnimState;

//キャラの共通処理
public abstract class Character : MonoBehaviour {

    public PlayerController playerController;
    protected Transform playerTf;
    public HitBox[] hitBox = new HitBox[5];
    public HurtBox hurtBox;
    protected Animator animator;
    public int counter;
    public static float dashspeed = 0.9f;
    public static float airspeed = 0.25f;
    public static float vectorspeed = 3.0f;

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
        animator = gameObject.GetComponent<Animator>();
        counter = 0;

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
    public void StartResistance(Vector2 vector, float damage) {
        playerController.damageVector = vector;
        playerController.resistDamage = damage;
        if (playerController.stateInfo.fullPathHash == AnimState.SideA ||
            playerController.stateInfo.fullPathHash == AnimState.LightningAttack) {
            playerController.animator.Play("SideA_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.SideB ||
            playerController.stateInfo.fullPathHash == AnimState.SideB_Air ||
            playerController.stateInfo.fullPathHash == AnimState.UpB_Fall) {
            playerController.animator.Play("SideB_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.NutralA ||
            playerController.stateInfo.fullPathHash == AnimState.CriticalNA) {
            playerController.animator.Play("NutralA_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.SideA_Air) {
            playerController.animator.Play("SideA_Air_R");
        }
        else if (playerController.stateInfo.fullPathHash == AnimState.NutralA_Air) {
            playerController.animator.Play("NutralA_Air_R");
        }
        else {
            playerController.animator.Play("SideA_R");
            Debug.Log("NoneR");
        }
    }

    public void StartGame() {
        if (counter == 0) {
            BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
            BattleMgr.Instance.ZoomInOutDouble(0.1f);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            animator.speed = Main.Instance.gameSpeed;
        }
    }
    public void Idle() {
        if (animator.GetBool(AnimState.ButtonL) && !playerController.isLimitBreak && 100 <= playerController.mp) {
            animator.Play("LimitBreak");
            playerController.mp = 0;
        }
    }
    public void Dash() {
        playerTf.position += Vector3.right * dashspeed * playerController.input.xAxis * animator.speed;
        Teach(0);
    }
    public void JumpStart() {
        if (counter == 0)
            Teach(1);
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
            Teach(3);
        }
    }
    public void NutralA_Air() {
        if (counter == 0) {
            Teach(3);
        }
    }
    public void LightningStart() {
        if (counter == 0) {
            playerTf.localScale = playerController.enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            Teach(2);
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
            playerTf.localScale = playerController.enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            Vector3 lightningPos = 5 * (playerController.enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right);
            Vector3 ofs = Vector3.zero;
            if (playerController.input.xAxis > 0)
                ofs = 1 * Vector3.right;
            else if (playerController.input.xAxis < 0)
                ofs = 1 * Vector3.left;
            if (playerController.input.yAxis > 0)
                ofs = 5 * Vector3.up;
            else if (playerController.input.yAxis < 0)
                ofs = 5 * Vector3.down;
            playerTf.position += (playerController.enemyTf.position + lightningPos + ofs - playerTf.position) / (counter == 5 ? 1 : 2);
        }
        else
            playerTf.position += new Vector3(playerController.enemyController.damageVector.x, 0, 0);
    }
    public void LightningAttack() {
        playerTf.position += new Vector3(playerController.enemyController.damageVector.x * Time.timeScale, 0, 0);
    }
    public void LightningEnd() {
        playerTf.position += new Vector3(playerController.enemyController.damageVector.x * Time.timeScale, 0, 0);
    }
    public void SideA() {
        if (counter == 0) {
            Teach(5);
        }
        if (counter == 10) {
            BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, Quaternion.identity, 1.0f);
        }
    }
    public void SideA_Air() {
        if (counter == 0) {
            Teach(5);
        }
        if (counter == 8) {
            BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, Quaternion.identity, 1.0f);
        }
    }
    public void DownA() {
        if (counter == 0) {
            StartCoroutine(DownACoroutine());
            Teach(7);
        }
    }
    public void DownA_Air() {
        if (counter == 0) {
            StartCoroutine(DownACoroutine());
            Teach(7);
        }
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && playerTf.position.y != 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
    }
    IEnumerator DownACoroutine() {
        float speed = playerController.animator.speed;
        yield return new WaitForSeconds(14.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position, Quaternion.identity, 1.0f);
        Instantiate(playerController.HibiPref[Random.Range(0, playerController.HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
        BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        yield return 0;
    }
    public void UpA() {
        if (counter == 0) {
            Teach(9);
        }
    }
    public void LimitBreak() {
        if (counter == 0) {
            playerController.isLimitBreak = true;
            StartCoroutine(LimitBreakCoroutine());
            Teach(11, 10);
        }
    }
    IEnumerator LimitBreakCoroutine() {
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
        BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
        playerTf.position = new Vector3(playerTf.position.x, 0, playerTf.position.z);
        animator.speed *= 1.2f;
        if (playerController.playerNum == PlayerController.PlayerNum.player1) {
            playerTf.gameObject.GetComponent<SpriteRenderer>().material = redWhite;
        }
        if (playerController.playerNum == PlayerController.PlayerNum.player2) {
            playerTf.gameObject.GetComponent<SpriteRenderer>().material = blueBlack;
        }
    }
    public void Critical() {
        if (counter == 0) {
            playerTf.localScale = playerController.damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
            BattleMgr.Instance.ChangeTimeScale(0.05f, 0.5f);
            BattleMgr.Instance.ChangeToneDouble(0.5f, ((int)playerController.playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
            BattleMgr.Instance.ZoomInOutDouble(0.1f);
            BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + new Vector3(playerController.enemyTf.position.x < playerTf.position.x ? 3 : -3, 1, 0), Quaternion.identity, 1.0f);
            if (playerController.hp < playerController.maxhp * 0.25f)
                BattleMgr.Instance.CreateCrack(playerController.damageVector.x < 0);
            if (playerController.hp <= 0)
                Main.battleResult = Main.BattleResult.Finish;
        }
        playerTf.position += playerController.damageVector * Time.timeScale;
        playerTf.position += Vector3.down * counter * 0.002f * animator.speed;
        if (playerTf.position.y < 0.1f && 50 < counter * animator.speed) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            if (playerController.hp <= 0)
                animator.Play("Death");
            else
                animator.Play("CriticalEnd");
        }
    }
    public void CriticalUp() {
        if (counter == 0) {
            BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
        }
        playerTf.position += new Vector3(playerController.damageVector.x, 0, 0);
    }
    public void CriticalFall() {
        playerTf.position += new Vector3(playerController.damageVector.x, -counter * 0.1f * animator.speed, 0);
        if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            if (playerController.damageVector.x == 0) {
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
            BattleMgr.Instance.ChangeToneDouble(0.1f, ((int)playerController.playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
            BattleMgr.Instance.CreateVFX("CriticalDownWave", playerTf.position + Vector3.up * 2.0f, Quaternion.identity, 1.0f);
        }
        playerTf.position = new Vector3(playerTf.position.x + playerController.damageVector.x, playerTf.position.y + playerController.damageVector.y, 0) * animator.speed;
        if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.CreateVFX("LandWave", playerTf.position, Quaternion.identity, 1.0f);
            BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
            animator.Play("CriticalUp");
        }
    }
    public void CriticalEnd() {
        playerController.damageVector *= 0.9f;
        playerTf.position += new Vector3(playerController.damageVector.x, 0, 0);
        if (Mathf.Abs(playerController.damageVector.x) < 0.1f && 20 <= counter) {
            playerController.damageVector = Vector3.zero;
            animator.Play("Idle");
        }
    }
    public void Wince() {
        playerTf.position += new Vector3(playerController.damageVector.x, -counter * 0.1f * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
        }
    }
    public void Resistance() {
        if (counter == 0 && playerController.playerNum == PlayerController.PlayerNum.player1)
            BattleMgr.Instance.StartResistance();
        if (playerController.input.A || playerController.input.B) {
            if (playerController.playerNum == PlayerController.PlayerNum.player1)
                BattleMgr.Instance.resistCounter1P++;
            if (playerController.playerNum == PlayerController.PlayerNum.player2)
                BattleMgr.Instance.resistCounter2P++;
        }
        if (counter == 20) {
            BattleMgr.Instance.VibrateDouble(0.3f, 2.0f);
            BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (playerController.enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("Line", playerTf.position + (playerController.enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-70, 70)), 1.0f);
        }
        if (counter == 40) {
            BattleMgr.Instance.VibrateDouble(0.4f, 2.0f);
            BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (playerController.enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("Line", playerTf.position + (playerController.enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-70, 70)), 1.0f);
        }
        if (counter == 60) {
            playerController.isResistance = false;
            if ((BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical1P && playerController.playerNum == PlayerController.PlayerNum.player1) ||
                (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical2P && playerController.playerNum == PlayerController.PlayerNum.player2)) { //鍔迫り合いに負けた時
                BattleMgr.Instance.CreateVFX("Hit", playerTf.position, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("HitWave", playerTf.position, Quaternion.identity, 1.0f);
                playerController.hp -= playerController.resistDamage * 2.0f;
                playerController.mp += playerController.resistDamage;
                playerController.resistDamage = 0;
                animator.Play("Critical");
            }
            else if (BattleMgr.Instance.resistResult != BattleMgr.ResistResult.Wince) { //勝った時
                playerController.damageVector = Vector3.zero;
                playerController.mp += playerController.resistDamage * 0.5f;
                playerController.resistDamage = 0;
                if (playerController.stateInfo.fullPathHash == AnimState.NutralA_R)
                    animator.Play("NutralA_RW");
                else if (playerController.stateInfo.fullPathHash == AnimState.NutralA_Air)
                    animator.Play("NutralA_Air_R");
                else if (playerController.stateInfo.fullPathHash == AnimState.SideA_R)
                    animator.Play("SideA_RW");
                else if (playerController.stateInfo.fullPathHash == AnimState.SideA_Air_R)
                    animator.Play("SideA_Air_RW");
                else if (playerController.stateInfo.fullPathHash == AnimState.SideB_R)
                    animator.Play("SideB_RW");
                else if (playerController.stateInfo.fullPathHash == AnimState.SideB_Air_R)
                    animator.Play("SideB_Air_RW");
                else {
                    animator.Play("SideA_RW");
                    Debug.Log("NoneRW");
                }
            }
            else {
                BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone); //引き分け
                playerController.resistDamage = 0;
                playerController.damageVector = Vector3.zero;
                animator.Play("Wince");
            }
        }
    }
    public void GameEnd() {
        if (counter == 0) {
            BattleMgr.Instance.BattleEnd();
            if  (playerController.isAI && playerController.enemyController.isTeacher) {
                playerController.inputAI.UpdateCSV();
            }
            else if (playerController.isTeacher && playerController.enemyController.isAI) {
                playerController.enemyController.inputAI.UpdateCSV();
            }
        }
    }

    public virtual void NutralB() { }
    public virtual void NutralB_Air() { }
    public virtual void SideB() { }
    public virtual void SideB_Air() { }
    public virtual void UpB() { }
    public virtual void UpB_Fall() { }
    public virtual void DownB() { }
    public virtual void DownB_Air_Fall() { }

    protected void Teach(int n) {
        if (playerController.isTeacher && playerController.enemyController.isAI) {
            playerController.enemyController.inputAI.inputValues[n].deltaX[playerController.dx] += 1;
            playerController.enemyController.inputAI.inputValues[n].deltaY[playerController.dy] += 1;
        }
    }
    protected void Teach(int n, int m) {
        if (playerController.isTeacher && playerController.enemyController.isAI) { 
            playerController.enemyController.inputAI.inputValues[n].deltaX[playerController.dx] += m;
            playerController.enemyController.inputAI.inputValues[n].deltaY[playerController.dy] += m;
        }
    }
}
