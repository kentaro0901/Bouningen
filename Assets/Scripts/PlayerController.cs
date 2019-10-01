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

    [SerializeField] float dashspeed;
    [SerializeField] float airspeed;
    [SerializeField] float vectorspeed;

    float xAxisD;
    float yAxisD;
    Vector3 prePos;
    Vector2 vector;
    public Vector3 damageVector = Vector3.zero;

    public bool isResistance = false;
    bool isLimitBreak = false;

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

        xAxisD = Input.GetAxis("DPad_XAxis_" + (int)playerNum);
        yAxisD = Input.GetAxis("DPad_YAxis_" + (int)playerNum);

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //カウンターリセット
        if (stateInfo.fullPathHash != preStateInfo.fullPathHash) counter = 0;

        //状態分岐
        if (stateInfo.fullPathHash == AnimState.Instance.Start) {
            if (counter == 0) {
                BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Idle) {
            if(animator.GetBool("ButtonL") && !isLimitBreak) animator.Play("LimitBreak");
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
                //BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);             
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Lightning) {
            if (counter == 0) BattleMgr.Instance.VibrateDouble(0.8f, 1.0f);
            if (counter <= 10) {
                Vector3 ofs = 4 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right);
                if (xAxisD > 0) ofs += 2* Vector3.right;
                else if (xAxisD < 0) ofs += 2* Vector3.left;
                if (yAxisD > 0) ofs += 2* Vector3.up;
                else if (yAxisD < 0) ofs += 2* Vector3.down;
                playerTf.position += (enemyTf.position + ofs - playerTf.position) / 2; //相手の3m前に移動
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LightningAttack) {
            Debug.Log("LA");
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.SideB) {
            if(counter == 13) BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.DownA) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            if (counter == 14) BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.DownB_Air) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.UpB_Fall) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            if (counter == 10) BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.Critical) {
            if (counter == 0) {
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
                if (hp <= 0) Main.battleResult = Main.BattleResult.Finish;
            }
            if (Time.timeScale == 1.0f) {
                playerTf.position += damageVector;
                playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.002f), 0);
                if (playerTf.position.y < 0.1f && 45 < counter) {
                    playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                    if (hp <= 0) animator.Play("Death");
                    else animator.Play("CriticalEnd");                  
                }
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalEnd) {
            damageVector *= 0.9f;
            playerTf.position += damageVector;
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalNA) {
            damageVector *= 0.9f;
            playerTf.position += damageVector;
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalFall) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.1f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                animator.Play("CriticalFallEnd");
            }
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.CriticalFallEnd) {
            if (counter == 0) BattleMgr.Instance.VibrateDouble(0.5f, 0.3f);
        }
        else if (stateInfo.fullPathHash == AnimState.Instance.LimitBreak) {//
            switch (counter) {
                case 0:
                    isLimitBreak = true;
                    BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone); break;
                case 20:
                    BattleMgr.Instance.VibrateDouble(0.8f, 1.0f); break;
                case 40:
                    BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone); break;
                case 60:
                    BattleMgr.Instance.VibrateDouble(1.5f, 2.0f);
                    BattleMgr.Instance.ChangeToneDouble(0.2f, CameraEffect.ToneName.whiteWhite); break;
                case 70:
                    BattleMgr.Instance.ChangeToneDouble(3.0f, CameraEffect.ToneName.reverseTone); break;               
                default: break;
            }
        }
        if (stateInfo.fullPathHash != AnimState.Instance.LimitBreak && preStateInfo.fullPathHash == AnimState.Instance.LimitBreak) {
            BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
            character.LimitBreak();
            playerTf.position = new Vector3(playerTf.position.x, 0, playerTf.position.z);
            animator.speed = 1.2f;
        }
        if (stateInfo.fullPathHash == AnimState.Instance.SideA_R || 
            stateInfo.fullPathHash == AnimState.Instance.NutralA_R || 
            stateInfo.fullPathHash == AnimState.Instance.SideB_R) {
            if (counter == 0) {
                BattleMgr.Instance.resistCounter1P = 0;
                BattleMgr.Instance.resistCounter2P = 0;
                BattleMgr.Instance.ChangeTimeScale(0.0f, 0.3f);
                BattleMgr.Instance.ChangeToneDouble(2.0f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
            }
            if (playerNum == PlayerNum.player1 && Input.GetButtonDown("ButtonA_" + (int)playerNum)) BattleMgr.Instance.resistCounter1P++;
            else if (Input.GetButtonDown("ButtonA_" + (int)playerNum)) BattleMgr.Instance.resistCounter2P++;    
            if(counter == 60) {
                isResistance = false;
                if (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical1P) {
                    if (playerNum == PlayerNum.player1) {
                        BattleMgr.Instance.ChangeTimeScale(0.0f, 0.5f);
                        BattleMgr.Instance.ChangeToneDouble(0.5f, CameraEffect.ToneName.blueBlack);
                        animator.Play("Critical");
                    }
                    else  animator.Play("Idle");
                }
                else if (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical2P) {
                    if (playerNum == PlayerNum.player2) {
                        BattleMgr.Instance.ChangeTimeScale(0.0f, 0.5f);
                        BattleMgr.Instance.ChangeToneDouble(0.5f, CameraEffect.ToneName.redBlack);
                        animator.Play("Critical");
                    }
                    else animator.Play("Idle");
                }
                else {
                    BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
                    animator.Play("Wince");
                }
            }
        }
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

        if (Main.battleResult == Main.BattleResult.Default) {
            animator.SetBool("UpArrow", yAxisD > 0);
            animator.SetBool("RightArrow", xAxisD > 0);
            animator.SetBool("LeftArrow", xAxisD < 0);
            animator.SetBool("DownArrow", yAxisD < 0);

            animator.SetBool("ButtonA", Input.GetButton("ButtonA_" + (int)playerNum));
            animator.SetBool("ButtonB", Input.GetButton("ButtonB_" + (int)playerNum));
            animator.SetBool("ButtonY", Input.GetButton("ButtonY_" + (int)playerNum));
            animator.SetBool("ButtonX", Input.GetButton("ButtonX_" + (int)playerNum));
            animator.SetBool("ButtonR", Input.GetButton("ButtonR_" + (int)playerNum));
            animator.SetBool("ButtonL", Input.GetButton("ButtonL_" + (int)playerNum));
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