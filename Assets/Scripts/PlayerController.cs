﻿using System.Collections;
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

    static float dashspeed = 0.8f;
    static float airspeed = 0.15f; 
    static float vectorspeed = 3.0f;

    float xAxisD;
    float yAxisD;
    Vector3 prePos;
    Vector2 vector;
    public Vector3 damageVector = Vector3.zero;
    public Vector3 resistVector = Vector3.zero;

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
            playerTf.position += Vector3.right * dashspeed * xAxisD;
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Fall) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.1f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Landing) {
            if (counter == 0) BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningStart) {
            if (counter == 0) {
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);      
                lightningPos = 5 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right);
            }
            if (xAxisD > 0) lightningPos = 5 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right) + 2 * Vector3.right;
            else if (xAxisD < 0) lightningPos = 5 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right) + 2 * Vector3.left;
            if (yAxisD > 0) lightningPos = 7 * Vector3.up; 
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Lightning) {
            if (counter == 0) {
                BattleMgr.Instance.VibrateDouble(0.8f, 1.0f);
            }
            if (counter <= 5) {
                playerTf.position += (enemyTf.position + lightningPos - playerTf.position) / (counter == 5 ? 1:2);
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            }
            playerTf.position += new Vector3(enemyController.damageVector.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningAttack) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningAttackDown) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.SideB) {
            if(counter == 13) BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.DownA) {
            if (counter == 0) StartCoroutine(character.DownA());
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.DownB_Air) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.UpB_Fall) {
            if (counter == 0) StartCoroutine(character.UpB_Fall());
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Critical) { //クリティカル
            if (counter == 0) {
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
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
            playerTf.position = new Vector3(playerTf.position.x + damageVector.x, playerTf.position.y - 1.5f * animator.speed, 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
                animator.Play("CriticalUp");
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
            stateInfo.fullPathHash == AnimState.Instance.SideB_R) {
            if (counter == 0 && playerNum == PlayerNum.player1)  BattleMgr.Instance.StartResistance();
            if (Input.GetButtonDown("ButtonA_" + (int)playerNum) || Input.GetButtonDown("ButtonB_" + (int)playerNum)) {
                if (playerNum == PlayerNum.player1) BattleMgr.Instance.resistCounter1P++;
                if (playerNum == PlayerNum.player2) BattleMgr.Instance.resistCounter2P++;
            }
            playerTf.position += resistVector;
            if (counter == 20) BattleMgr.Instance.VibrateDouble(0.3f, 2.0f);
            if (counter == 40) BattleMgr.Instance.VibrateDouble(0.4f, 2.0f);
            if (counter == 60) {
                isResistance = false;
                resistVector = Vector3.zero;
                if (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical1P) {
                    if (playerNum == PlayerNum.player1) {
                        BattleMgr.Instance.ChangeTimeScale(0.05f, 0.5f);
                        BattleMgr.Instance.ChangeToneDouble(0.5f, CameraEffect.ToneName.blueBlack);
                        animator.Play("Critical");
                    }
                    else {
                        damageVector = Vector3.zero;
                        if (stateInfo.fullPathHash == AnimState.Instance.NutralA_R) animator.Play("NutralA_RW");
                        else if (stateInfo.fullPathHash == AnimState.Instance.SideA_R) animator.Play("SideA_RW");
                        else if (stateInfo.fullPathHash == AnimState.Instance.SideB_R) animator.Play("SideB_RW");
                        else animator.Play("Idle");                      
                    }
                }
                else if (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical2P) {
                    if (playerNum == PlayerNum.player2) {
                        BattleMgr.Instance.ChangeTimeScale(0.05f, 0.5f);
                        BattleMgr.Instance.ChangeToneDouble(0.5f, CameraEffect.ToneName.redBlack);
                        animator.Play("Critical");
                    }
                    else {
                        damageVector = Vector3.zero;
                        if (stateInfo.fullPathHash == AnimState.Instance.NutralA_R) animator.Play("NutralA_RW");
                        else if (stateInfo.fullPathHash == AnimState.Instance.SideA_R) animator.Play("SideA_RW");
                        else if (stateInfo.fullPathHash == AnimState.Instance.SideB_R) animator.Play("SideB_RW");
                        else animator.Play("Idle");
                    }
                }
                else {
                    BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
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
            if (0 < playerTf.position.y) playerTf.position += Vector3.right * (vectorspeed * vector.x + airspeed * xAxisD);
        }

        //自動反転
        if (stateInfo.fullPathHash == AnimState.Instance.Start ||
            stateInfo.fullPathHash == AnimState.Instance.Idle ||
            stateInfo.fullPathHash == AnimState.Instance.CriticalUp ||
            stateInfo.fullPathHash == AnimState.Instance.CriticalFall) {
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);//
        }

        //手動反転
        if (stateInfo.fullPathHash == AnimState.Instance.Step ||
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
        
        animator.SetBool("isLand", playerTf.position.y <= 0);
        animator.SetBool("isRight", 0 < playerTf.localScale.x);

        counter++;
        prePos = playerTf.position;
        preStateInfo = stateInfo;
    }
}