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

    //AI学習用
    public bool isAI = false;
    public bool isTeacher = false;
    public bool isLeveling = false;
    public int level = 50; //0-100
    public Vector2 dv;
    public int dx;
    public int dy;
    public InputAI inputAI;
    public string AIFileName;

    public InputMethod input;
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

    public Transform playerTf;

    public PlayerController enemyController;
    public Transform enemyTf;

    public AnimatorStateInfo preStateInfo;
    public AnimatorStateInfo stateInfo;
    public Animator animator;

    Vector3 prePos;
    Vector2 vector;
    public Vector3 damageVector = Vector3.zero;
    public float resistDamage = 0.0f;

    public bool isResistance = false;
    public bool isLimitBreak = false;

    void Awake() { 
        //キャラの生成
        switch (myChara) {
            case Main.Chara.Sword:
                characterIns = Instantiate(swordPref, playerNum == PlayerNum.player1 ? new Vector3(-20,0,0): new Vector3(20,0,0), new Quaternion(0, 0, 0, 0));
                character = characterIns.GetComponent<Sword>();
                maxhp = Sword.maxhp;
                break;
            case Main.Chara.Fighter:
                //characterIns = Instantiate(fighterPref, playerNum == PlayerNum.player1 ? new Vector3(-15, 0, 0) : new Vector3(15, 0, 0), new Quaternion(0, 0, 0, 0));
                //character = characterIns.GetComponent<Fighter>();
                //maxhp = Fighter.maxhp;
                break;
            default:
                break;
        }

        playerTf = characterIns.transform;
        chaseCamera.playerTf = playerTf;
    }

    void Start() {
        counter = 0;
        hp = maxhp;
        character.playerController = this;
        animator = characterIns.GetComponent<Animator>();
        enemyTf = enemyController.playerTf;
        animator.speed = Main.Instance.gameSpeed;
        if (isAI || Main.Instance.playerType[(int)playerNum - 1] == Main.PlayerType.AI) {
            inputAI = gameObject.AddComponent<InputAI>();
            input = inputAI;
        }
        else {
            switch (Main.controller[(int)(playerNum - 1)]) {
                case Main.Controller.GamePad: input = gameObject.AddComponent<InputGamePad>(); break;
                case Main.Controller.Joycon:  input = gameObject.AddComponent<InputJoycon>(); break;
                default: input = gameObject.AddComponent<InputGamePad>(); break;
            }
        }
    }

    void Update() {
        vector = playerTf.position - prePos;

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //カウンターリセット
        if (stateInfo.fullPathHash != preStateInfo.fullPathHash) counter = 0;
        character.counter = counter;

        //AI強化用(仮)
        dv = enemyTf.position - playerTf.position;
        dx = Mathf.Min(Mathf.Abs((int)dv.x), InputAI.xDataNum-1);
        dy = ((int)Mathf.Abs(dv.y) < (int)Mathf.Floor(InputAI.yDataNum / 2) ? (int)dv.y : (dv.y < 0 ? -1 : 1) * (int)Mathf.Floor(InputAI.yDataNum / 2)) + (int)Mathf.Floor(InputAI.yDataNum / 2);

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

        //地面判定（仮）
        if (playerTf.position.y < 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }

        //空中制御
        if (stateInfo.fullPathHash == JumpStart ||
            stateInfo.fullPathHash == ShortJump ||
            stateInfo.fullPathHash == Jump ||
            stateInfo.fullPathHash == JumpEnd ||
            stateInfo.fullPathHash == Fall ||
            stateInfo.fullPathHash == CriticalFall ||
            stateInfo.fullPathHash == UpB_Fall) {
            if (0 < playerTf.position.y) playerTf.position += Vector3.right * (Character.vectorspeed * vector.x + Character.airspeed * input.xAxis) * animator.speed;
        }

        //自動反転
        if (stateInfo.fullPathHash == StartGame || stateInfo.fullPathHash == Idle ||
            stateInfo.fullPathHash == SideA_R || stateInfo.fullPathHash == NutralA_R || stateInfo.fullPathHash == SideB_R ||
            stateInfo.fullPathHash == SideA_Air_R || stateInfo.fullPathHash == NutralA_Air_R) {
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
        }
        //手動反転
        else if (stateInfo.fullPathHash == Step ||
            stateInfo.fullPathHash == StepEnd ||
            stateInfo.fullPathHash == DashEnd ||
            stateInfo.fullPathHash == JumpStart ||
            stateInfo.fullPathHash == Jump ||
            stateInfo.fullPathHash == ShortJump ||
            stateInfo.fullPathHash == JumpEnd ||
            stateInfo.fullPathHash == Fall ) {
            if (input.xAxis > 0) playerTf.localScale = new Vector3(1, 1, 1);
            if (input.xAxis < 0) playerTf.localScale = new Vector3(-1, 1, 1);
        }

        if (Main.battleResult == Main.BattleResult.Battle) {
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

        counter++;
        prePos = playerTf.position;
        preStateInfo = stateInfo;
    }
}