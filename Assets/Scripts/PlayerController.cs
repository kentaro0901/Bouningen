using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public enum PlayerNum {
        zero,
        player1,
        player2
    }
    public PlayerNum playerNum;
    public Main.Chara myChara = Main.Chara.Sword;
    [SerializeField] GameObject swordPref;
    [SerializeField] GameObject fighterPref;
    GameObject characterIns;
    Character character;

    public float maxhp = 100.0f;
    public float hp = 100.0f;
    public float mp = 0.0f;

    public int counter = 0;

    public ChaseCamera chaseCamera;
    public CameraEffect cameraEffect;

    public Transform playerTf;

    public PlayerController enemyController;
    public Transform enemyTf;

    AnimatorStateInfo preStateInfo;
    public AnimatorStateInfo stateInfo;
    public Animator animator;

    static float dashspeed = 0.9f;
    static float airspeed = 0.25f; 
    static float vectorspeed = 3.0f;

    float xAxisD;
    float yAxisD;
    Vector3 prePos;
    Vector2 vector;
    public Vector3 damageVector = Vector3.zero;
    public Vector3 resistVector = Vector3.zero;
    public float resistDamage = 0.0f;

    public bool isResistance = false;
    public bool isLimitBreak = false;
    Vector3 lightningPos = Vector3.zero;

    public GameObject[] HibiPref;

    void Awake() { 
        //キャラの生成
        switch (myChara) {
            case Main.Chara.Sword:
                characterIns = Instantiate(swordPref, playerNum == PlayerNum.player1 ? new Vector3(-15,0,0): new Vector3(15,0,0), new Quaternion(0, 0, 0, 0));
                character = characterIns.GetComponent<Sword>();
                maxhp = Sword.maxhp;
                break;
            case Main.Chara.Fighter:
                characterIns = Instantiate(fighterPref, playerNum == PlayerNum.player1 ? new Vector3(-15, 0, 0) : new Vector3(15, 0, 0), new Quaternion(0, 0, 0, 0));
                character = characterIns.GetComponent<Fighter>();
                maxhp = Fighter.maxhp;
                break;
            default:
                break;
        }

        playerTf = characterIns.transform;
        chaseCamera.playerTf = playerTf;
    }

    void Start() {
        hp = maxhp;
        character.playerController = this;
        animator = characterIns.GetComponent<Animator>();
        enemyTf = enemyController.playerTf;
        animator.speed = Main.Instance.gameSpeed;
    }

    void Update() {
        vector = playerTf.position - prePos;

        //X
        if (Mathf.Abs(Input.GetAxis("DPad_XAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("L_XAxis_" + (int)playerNum)) &&
            Mathf.Abs(Input.GetAxis("DPad_XAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_XAxis_" + (int)playerNum))) {
            xAxisD = Input.GetAxis("DPad_XAxis_" + (int)playerNum);
        }
        else if (Mathf.Abs(Input.GetAxis("L_XAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_XAxis_" + (int)playerNum))) {
            xAxisD = Input.GetAxis("L_XAxis_" + (int)playerNum);
        }
        else {
            xAxisD = Input.GetAxis("R_XAxis_" + (int)playerNum);
        }
        //Y
        if (Mathf.Abs(Input.GetAxis("DPad_YAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("L_YAxis_" + (int)playerNum)) &&
            Mathf.Abs(Input.GetAxis("DPad_YAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_YAxis_" + (int)playerNum))) {
            yAxisD = Input.GetAxis("DPad_YAxis_" + (int)playerNum);
        }
        else if (Mathf.Abs(Input.GetAxis("L_YAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_YAxis_" + (int)playerNum))) {
            yAxisD = Input.GetAxis("L_YAxis_" + (int)playerNum);
        }
        else {
            yAxisD = Input.GetAxis("R_YAxis_" + (int)playerNum);
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //カウンターリセット
        if (stateInfo.fullPathHash != preStateInfo.fullPathHash) counter = 0;

        //状態分岐
        if (stateInfo.fullPathHash == AnimState.Instance.Start) {
            if (counter == 0) {
                BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
                animator.speed = Main.Instance.gameSpeed;
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Idle) {
            if (animator.GetBool("ButtonL") && !isLimitBreak && 100 <= mp) {
                animator.Play("LimitBreak");
                mp = 0;
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Dash) {
            playerTf.position += Vector3.right * dashspeed * xAxisD * animator.speed;
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.JumpStart ||
            stateInfo.fullPathHash == AnimState.Instance.Jump ||
            stateInfo.fullPathHash == AnimState.Instance.JumpEnd) {
            if (yAxisD < 0 && Input.GetButtonDown("ButtonB_" + (int)playerNum)) {
                animator.Play("DownB_Air");//
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Fall) {
            playerTf.position += new Vector3(0, - (counter * 0.1f), 0) * animator.speed;
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Landing) {
            if (counter == 0) BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningStart) {
            if (counter == 0) {
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);      
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Lightning) {
            if (counter == 0) {
                BattleMgr.Instance.VibrateDouble(0.8f, 1.0f);
            }
            if (counter <= 5) {
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + Vector3.up, 1.0f);
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
                lightningPos = 5 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right);
                Vector3 ofs = Vector3.zero;
                if (xAxisD > 0) ofs = 1 * Vector3.right;
                else if (xAxisD < 0) ofs = 1 * Vector3.left;
                if (yAxisD > 0) ofs = 5 * Vector3.up;
                else if (yAxisD < 0) ofs = 5 * Vector3.down;
                playerTf.position += (enemyTf.position + lightningPos + ofs - playerTf.position) / (counter == 5 ? 1:2);            
            }
            else playerTf.position += new Vector3(enemyController.damageVector.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningAttack) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningEnd) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningAttackDown) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, - (counter * 0.2f) * animator.speed, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.SideA) {
            if (counter == 10) {
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.SideA_Air) {
            if (counter == 10) {
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.SideB) {
            if(counter == 13) BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.SideB_Air) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.5f) * animator.speed, 0);
            if (playerTf.position.y < 0.05f && !animator.GetBool("isLand")) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
                Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.DownA) {
            if (counter == 0) StartCoroutine(character.DownA());
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.05f && playerTf.position.y !=0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.DownB) {
            if (counter == 0) {
                StartCoroutine(character.DownB());             
            }
            if (counter % 4 == 0 && counter <= 8) {
                BattleMgr.Instance.CreateVFX("FallSunder", playerTf.position, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.DownB_Air_Fall) {
            if (counter == 0) {
                if (Mathf.Abs(enemyTf.position.x -playerTf.position.x) <= 1.5f) {
                    BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, 1.0f);
                    BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, 1.0f);
                    BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                    BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
                }          
            }
            if (counter % 3 == 0 && counter <= 9) {
                BattleMgr.Instance.CreateVFX("CriticalDownWave", playerTf.position + Vector3.up * 2, 1.0f);
            }
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.05f && !animator.GetBool("isLand")) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
                BattleMgr.Instance.CreateVFX("LandWave", playerTf.position, 1.0f);
                BattleMgr.Instance.VibrateDouble(1.0f, 1.5f);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.UpB) {
            if (counter == 0) {
                //BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.UpB_Fall) {
            if (counter == 0) {
                if (enemyController.stateInfo.fullPathHash == AnimState.Instance.CriticalUp ||
                    enemyController.stateInfo.fullPathHash == AnimState.Instance.CriticalFall) {
                    BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, 1.0f);
                    BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, 1.0f);
                    BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                    BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
                    BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, 1.0f);
                }
            }
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.05f && !animator.GetBool("isLand")) { //着地寸前
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
                Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Critical) { //クリティカル
            if (counter == 0) {
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
                BattleMgr.Instance.ChangeTimeScale(0.05f, 0.5f);
                BattleMgr.Instance.ChangeToneDouble(0.5f, ((int)playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + new Vector3((enemyTf.position.x < playerTf.position.x ? 3 : -3), 1, 0), 1.0f);
                if (hp < maxhp * 0.25f ) BattleMgr.Instance.CreateCrack(damageVector.x < 0);
                if (hp <= 0) Main.battleResult = Main.BattleResult.Finish;
            }
            playerTf.position += damageVector * Time.timeScale;
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.002f), 0);
            if (playerTf.position.y < 0.1f && 40 < counter * animator.speed) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                if (hp <= 0)
                    animator.Play("Death");
                else
                    animator.Play("CriticalEnd");
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalEnd ||
            stateInfo.fullPathHash == AnimState.Instance.CriticalFallEnd ||
            stateInfo.fullPathHash == AnimState.Instance.CriticalNA) {
            damageVector *= 0.9f;
            playerTf.position += new Vector3(damageVector.x, 0, 0);
            if (Mathf.Abs(damageVector.x) < 0.1f && 20 <= counter) {
                damageVector = Vector3.zero;
                animator.Play("Idle");
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalUp) {
            if (counter == 0) {
                BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position + Vector3.up, 1.0f);
            }
            playerTf.position += new Vector3(damageVector.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalFall) {
            playerTf.position += new Vector3(damageVector.x,  - counter * 0.1f * animator.speed, 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                if (damageVector.x == 0) {
                    BattleMgr.Instance.VibrateDouble(0.5f, 0.3f);
                    animator.Play("CriticalFallEnd");
                }
                else {
                    BattleMgr.Instance.VibrateDouble(0.5f, 0.7f);
                    animator.Play("CriticalEnd");
                }
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalDown) {
            if (counter == 0) {
                BattleMgr.Instance.ChangeToneDouble(0.1f, ((int)playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
                BattleMgr.Instance.CreateVFX("CriticalDownWave", playerTf.position + Vector3.up * 2.0f, 1.0f);
            }
            playerTf.position = new Vector3(playerTf.position.x + damageVector.x, playerTf.position.y + damageVector.y, 0) * animator.speed;
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
                animator.Play("CriticalUp");
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Wince) {
            playerTf.position += new Vector3(damageVector.x, -counter * 0.1f * animator.speed, 0);
            if (playerTf.position.y < 0.05f && !animator.GetBool("isLand")) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LimitBreak) {
            if (counter == 0) {
                isLimitBreak = true;
                StartCoroutine(character.LimitBreak());
            }
        }
        if (stateInfo.fullPathHash != AnimState.Instance.LimitBreak && preStateInfo.fullPathHash == AnimState.Instance.LimitBreak) {
            BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
            character.LimitBreakEnd();
            playerTf.position = new Vector3(playerTf.position.x, 0, playerTf.position.z);
            animator.speed *= 1.2f;
        }
        if (stateInfo.fullPathHash == AnimState.Instance.SideA_R || //鍔迫り合い
            stateInfo.fullPathHash == AnimState.Instance.NutralA_R || 
            stateInfo.fullPathHash == AnimState.Instance.SideB_R || 
            stateInfo.fullPathHash == AnimState.Instance.SideA_Air_R ||
            stateInfo.fullPathHash == AnimState.Instance.NutralA_Air_R) {
            if (counter == 0 && playerNum == PlayerNum.player1)  BattleMgr.Instance.StartResistance();
            if (Input.GetButtonDown("ButtonA_" + (int)playerNum) || Input.GetButtonDown("ButtonB_" + (int)playerNum)) {
                if (playerNum == PlayerNum.player1) BattleMgr.Instance.resistCounter1P++;
                if (playerNum == PlayerNum.player2) BattleMgr.Instance.resistCounter2P++;
            }
            playerTf.position += resistVector;
            if (counter == 20) {
                BattleMgr.Instance.VibrateDouble(0.3f, 2.0f);
                BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (enemyTf.position - playerTf.position) / 2 +Vector3.up * 2, 1.0f);
            }
            if (counter == 40) {
                BattleMgr.Instance.VibrateDouble(0.4f, 2.0f);
                BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, 1.0f);
            }
            if (counter == 60) {
                isResistance = false;
                resistVector = Vector3.zero;
                if ((BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical1P && playerNum == PlayerNum.player1) ||
                    (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical2P && playerNum == PlayerNum.player2)) { //鍔迫り合いに負けた時
                    BattleMgr.Instance.ChangeTimeScale(0.05f, 0.5f);
                    BattleMgr.Instance.ChangeToneDouble(0.5f, playerNum == PlayerNum.player1 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack);
                    BattleMgr.Instance.CreateVFX("Hit", playerTf.position, 1.0f);
                    BattleMgr.Instance.CreateVFX("HitWave", playerTf.position, 1.0f);
                    hp -= resistDamage * 2.0f;
                    resistDamage = 0;
                    animator.Play("Critical");
                }
                else if (BattleMgr.Instance.resistResult != BattleMgr.ResistResult.Wince) { //勝った時
                    damageVector = Vector3.zero;
                    resistDamage = 0;
                    if (stateInfo.fullPathHash == AnimState.Instance.NutralA_R)
                        animator.Play("NutralA_RW");
                    else if (stateInfo.fullPathHash == AnimState.Instance.SideA_R)
                        animator.Play("SideA_RW");
                    else if (stateInfo.fullPathHash == AnimState.Instance.SideB_R)
                        animator.Play("SideB_RW");
                    else if (stateInfo.fullPathHash == AnimState.Instance.SideA_Air_R)
                        animator.Play("SideA_Air_RW");
                    else if (stateInfo.fullPathHash == AnimState.Instance.NutralA_Air_R)
                        animator.Play("NutralA_Air_RW");
                    else
                        animator.Play("SideA_RW");
                }
                else {
                    BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone); //引き分け
                    resistDamage = 0;
                    damageVector = Vector3.zero;
                    animator.Play("Wince");
                }
            }
        }

        //ゲーム終了
        if (stateInfo.fullPathHash == AnimState.Instance.GameEnd) {
            if (counter == 0) BattleMgr.Instance.BattleEnd();
        }

        //地面判定（仮）
        if (playerTf.position.y < 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }

        //空中制御
        if (stateInfo.fullPathHash == AnimState.Instance.JumpStart ||
            stateInfo.fullPathHash == AnimState.Instance.Jump ||
            stateInfo.fullPathHash == AnimState.Instance.JumpEnd ||
            stateInfo.fullPathHash == AnimState.Instance.Fall ||
            stateInfo.fullPathHash == AnimState.Instance.CriticalFall ||
            stateInfo.fullPathHash == AnimState.Instance.UpB_Fall) {
            if (0 < playerTf.position.y) playerTf.position += Vector3.right * (vectorspeed * vector.x + airspeed * xAxisD) * animator.speed;
        }

        //反転
        if (stateInfo.fullPathHash == AnimState.Instance.Start ||
            stateInfo.fullPathHash == AnimState.Instance.Idle ||
            stateInfo.fullPathHash == AnimState.Instance.CriticalUp ||
            stateInfo.fullPathHash == AnimState.Instance.CriticalFall) {
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Step ||
            stateInfo.fullPathHash == AnimState.Instance.StepEnd ||
            stateInfo.fullPathHash == AnimState.Instance.DashEnd ||
            stateInfo.fullPathHash == AnimState.Instance.JumpStart ||
            stateInfo.fullPathHash == AnimState.Instance.Jump ||
            stateInfo.fullPathHash == AnimState.Instance.JumpEnd ||
            stateInfo.fullPathHash == AnimState.Instance.Fall ) {
            if (xAxisD > 0) playerTf.localScale = new Vector3(1, 1, 1);
            if (xAxisD < 0) playerTf.localScale = new Vector3(-1, 1, 1);
        }

        if (Main.battleResult == Main.BattleResult.Battle) {
            animator.SetBool("UpArrow", yAxisD > 0);
            animator.SetBool("RightArrow", xAxisD > 0);
            animator.SetBool("LeftArrow", xAxisD < 0);
            animator.SetBool("DownArrow", yAxisD < 0);

            animator.SetBool("ButtonA", Input.GetButton("ButtonA_" + (int)playerNum) ||
                Mathf.Abs(Input.GetAxis("R_XAxis_" + (int)playerNum)) > 0 || 
                Mathf.Abs(Input.GetAxis("R_YAxis_" + (int)playerNum)) > 0);
            animator.SetBool("ButtonB", Input.GetButton("ButtonB_" + (int)playerNum));
            animator.SetBool("ButtonY", Input.GetButton("ButtonY_" + (int)playerNum));
            animator.SetBool("ButtonX", Input.GetButton("ButtonX_" + (int)playerNum));
            animator.SetBool("ButtonR", Input.GetButton("ButtonR_" + (int)playerNum) || Input.GetButton("ButtonZR_" + (int)playerNum));
            animator.SetBool("ButtonL", Input.GetButton("ButtonL_" + (int)playerNum) || Input.GetButton("ButtonZL_" + (int)playerNum));
        }
        else { //バトル中でないならすべてのパッド入力を無効にする
            animator.SetBool("UpArrow", false);
            animator.SetBool("RightArrow", false);
            animator.SetBool("LeftArrow", false);
            animator.SetBool("DownArrow", false);

            animator.SetBool("ButtonA", false);
            animator.SetBool("ButtonB", false);
            animator.SetBool("ButtonY", false);
            animator.SetBool("ButtonX", false);
            animator.SetBool("ButtonR", false);
            animator.SetBool("ButtonL", false);
        }
        
        animator.SetBool("isLand", playerTf.position.y <= 0.05f);
        animator.SetBool("isRight", 0 < playerTf.localScale.x);

        counter++;
        prePos = playerTf.position;
        preStateInfo = stateInfo;
    }
}