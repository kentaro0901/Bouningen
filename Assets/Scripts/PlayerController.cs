using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AnimState;

public class PlayerController : MonoBehaviour {

    public enum PlayerNum {
        zero,
        player1,
        player2
    }
    public PlayerNum playerNum;
    public string AIFileName;

    public InputMethod input;
    [SerializeField] GameObject swordPref;
    [SerializeField] GameObject fighterPref;
    [SerializeField] GameObject hammerPref;
    public Character character;

    public float maxhp = 100.0f;
    public float hp = 100.0f;
    public float mp = 0.0f;
    public ChaseCamera chaseCamera;
    public Transform playerTf;
    public PlayerController enemyController;
    public Transform enemyTf;
    public AnimatorStateInfo preStateInfo;
    public AnimatorStateInfo stateInfo;
    public Animator animator;
    public Vector3 damageVector = Vector3.zero;
    public float resistDamage = 0.0f;
    public bool isResistance = false;
    public bool isLimitBreak = false;

    void Awake() {

        Debug.Log((int)playerNum + "P : " + Main.Instance.chara[(int)playerNum-1]);
        GameObject charaPref = swordPref;
        switch (Main.Instance.chara[(int)playerNum-1]) {
            case Main.Chara.Sword: charaPref = swordPref; break;
            case Main.Chara.Fighter: charaPref = fighterPref; break;
            case Main.Chara.Hammer: charaPref = hammerPref; break;
            default: break;
        }
        GameObject characterIns = Instantiate(charaPref, playerNum == PlayerNum.player1 ? new Vector3(-20, 0, 0) : new Vector3(20, 0, 0), Quaternion.identity);
        character = characterIns.GetComponent<Character>();
        animator = characterIns.GetComponent<Animator>();
        playerTf = characterIns.transform;
        maxhp = character.status.maxhp;
        AIFileName = character.status.AIFileName;
        chaseCamera.playerTf = playerTf;
    }

    void Start() {
        hp = maxhp;
        character.controller = this;
        
        enemyTf = enemyController.playerTf;
        animator.speed = Main.Instance.gameSpeed;
        switch (Main.Instance.playerType[(int)playerNum - 1]) {
            case Main.PlayerType.Player: input = gameObject.AddComponent<InputGamePad>(); break;
            case Main.PlayerType.AI: input = gameObject.AddComponent<InputAI>(); break;
            default: input = gameObject.AddComponent<InputGamePad>(); break;
        }
    }

    void Update() {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //カウンターリセット
        if (stateInfo.fullPathHash != preStateInfo.fullPathHash) character.counter = 0;
        //状態分岐
        if (stateInfo.fullPathHash == Prepare) {}
        else if (stateInfo.fullPathHash == StartGame) { character.StartGame(); }
        else if (stateInfo.fullPathHash == Idle) { character.Idle(); }
        else if (stateInfo.fullPathHash == Dash) { character.Dash();}
        else if (stateInfo.fullPathHash == JumpStart) { character.JumpStart(); }
        else if (stateInfo.fullPathHash == Fall) { character.Fall();}
        else if (stateInfo.fullPathHash == Landing) { character.Landing();}
        else if (stateInfo.fullPathHash == LightningStart) { character.LightningStart();}
        else if (stateInfo.fullPathHash == Lightning) { character.Lightning();}
        else if (stateInfo.fullPathHash == LightningAttack) { character.LightningAttack();}
        else if (stateInfo.fullPathHash == LightningEnd) { character.LightningEnd();}
        else if (stateInfo.fullPathHash == NutralA) { character.NutralA();}
        else if (stateInfo.fullPathHash == NutralA_Air) { character.NutralA_Air();}
        else if (stateInfo.fullPathHash == NutralB) { character.NutralB();}
        else if (stateInfo.fullPathHash == NutralB_Air) { character.NutralB_Air();}
        else if (stateInfo.fullPathHash == SideA) { character.SideA();}
        else if (stateInfo.fullPathHash == SideA_Air) { character.SideA_Air(); }
        else if (stateInfo.fullPathHash == SideB) { character.SideB();}
        else if (stateInfo.fullPathHash == SideB_Air) { character.SideB_Air();}
        else if (stateInfo.fullPathHash == DownA) { character.DownA();}
        else if (stateInfo.fullPathHash == DownA_Air) { character.DownA_Air();}
        else if (stateInfo.fullPathHash == DownB) { character.DownB();}
        else if (stateInfo.fullPathHash == DownB_Fall) { character.DownB_Fall(); }
        else if (stateInfo.fullPathHash == DownB_Air) { character.DownB_Air(); }
        else if (stateInfo.fullPathHash == DownB_Air_Fall) { character.DownB_Air_Fall();}
        else if (stateInfo.fullPathHash == UpA) { character.UpA();}
        else if (stateInfo.fullPathHash == UpB) { character.UpB();}
        else if (stateInfo.fullPathHash == UpB_Fall) { character.UpB_Fall();}
        else if (stateInfo.fullPathHash == Critical) { character.Critical();}
        else if (stateInfo.fullPathHash == CriticalEnd) { character.CriticalEnd();}
        else if (stateInfo.fullPathHash == CriticalFallEnd) { character.CriticalEnd();}
        else if (stateInfo.fullPathHash == CriticalNA) { character.CriticalEnd(); }
        else if (stateInfo.fullPathHash == CriticalUp) { character.CriticalUp();}
        else if (stateInfo.fullPathHash == CriticalFall) { character.CriticalFall();}
        else if (stateInfo.fullPathHash == CriticalDown) { character.CriticalDown();}
        else if (stateInfo.fullPathHash == Wince) { character.Wince();}
        else if (stateInfo.fullPathHash == LimitBreak) { character.LimitBreak();}
        if (stateInfo.fullPathHash != LimitBreak && preStateInfo.fullPathHash == LimitBreak) { character.LimitBreakEnd();}
        if (stateInfo.fullPathHash == SideA_R || stateInfo.fullPathHash == NutralA_R || stateInfo.fullPathHash == SideB_R || 
            stateInfo.fullPathHash == SideA_Air_R || stateInfo.fullPathHash == NutralA_Air_R) { character.Resistance();}
        if (stateInfo.fullPathHash == GameEnd) { character.GameEnd();}

        //地面判定
        if (playerTf.position.y <= 0.05f) { character.JustLanding(); }
        if (playerTf.position.y < 0) { character.GroundCollision();}
        //空中制御
        if (0 < playerTf.position.y &&
            (stateInfo.fullPathHash == JumpStart || stateInfo.fullPathHash == ShortJump || stateInfo.fullPathHash == Jump ||
            stateInfo.fullPathHash == JumpEnd || stateInfo.fullPathHash == Fall || stateInfo.fullPathHash == CriticalFall ||
            stateInfo.fullPathHash == UpB_Fall || stateInfo.fullPathHash == DownB_Fall ||stateInfo.fullPathHash == DownB_Air_Fall)) {
            character.AirControll();
        }
        //自動反転
        if (stateInfo.fullPathHash == StartGame || stateInfo.fullPathHash == Idle ||
            stateInfo.fullPathHash == SideA_R || stateInfo.fullPathHash == NutralA_R || stateInfo.fullPathHash == SideB_R ||
            stateInfo.fullPathHash == SideA_Air_R || stateInfo.fullPathHash == NutralA_Air_R ||
            stateInfo.fullPathHash == SideA_RW|| stateInfo.fullPathHash == NutralA_RW|| stateInfo.fullPathHash == SideB_RW||
            stateInfo.fullPathHash == SideA_Air_RW|| stateInfo.fullPathHash == NutralA_Air_RW){
            character.AutoInvert();
        }
        //手動反転
        else if (stateInfo.fullPathHash == Step || stateInfo.fullPathHash == StepEnd || stateInfo.fullPathHash == DashEnd ||
            stateInfo.fullPathHash == JumpStart || stateInfo.fullPathHash == Jump || stateInfo.fullPathHash == ShortJump ||
            stateInfo.fullPathHash == JumpEnd || stateInfo.fullPathHash == Fall ) {
            character.ManualInvert();
        }

        if (BattleMgr.Instance.battleResult == BattleMgr.BattleResult.Battle) {
            animator.SetBool(UpArrow, input.yAxis > 0);
            animator.SetBool(RightArrow, input.xAxis > 0);
            animator.SetBool(LeftArrow, input.xAxis < 0);
            animator.SetBool(DownArrow, input.yAxis < 0);

            animator.SetBool(ButtonA, input.A);
            animator.SetBool(ButtonB, input.B);
            animator.SetBool(ButtonY, input.Y);
            animator.SetBool(ButtonX, input.X);
            animator.SetBool(ButtonR, input.R);
            animator.SetBool(ButtonL, input.L);
        }
        else { //バトル中でないならすべてのパッド入力を無効にする
            animator.SetBool(UpArrow, false);
            animator.SetBool(RightArrow, false);
            animator.SetBool(LeftArrow, false);
            animator.SetBool(DownArrow, false);

            animator.SetBool(ButtonA, false);
            animator.SetBool(ButtonB, false);
            animator.SetBool(ButtonY, false);
            animator.SetBool(ButtonX, false);
            animator.SetBool(ButtonR, false);
            animator.SetBool(ButtonL, false);
        }
        
        animator.SetBool(isLand, playerTf.position.y <= 0.05f);
        animator.SetBool(isRight, 0 < playerTf.localScale.x);

        character.counter++;
        preStateInfo = stateInfo;
    }
}