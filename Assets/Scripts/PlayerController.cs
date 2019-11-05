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
    public bool isAI = false;
    public bool isTeacher = false;
    Vector2 dv;
    int dx;
    int dy;
    public InputAI inputAI;
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

    float xAxis = 0.0f;
    float yAxis = 0.0f;
    bool A = false;
    bool B = false;
    bool X = false;
    bool Y = false;
    bool L = false;
    bool R = false;
    Vector3 prePos;
    Vector2 vector;
    public Vector3 damageVector = Vector3.zero;
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
        if (isAI) {
            inputAI = gameObject.AddComponent<InputAI>();
        }
    }

    void Update() {
        vector = playerTf.position - prePos;


        //Input
        if (isAI) {
            xAxis = inputAI.AxisX;
            yAxis = inputAI.AxisY;
            A = inputAI.A;
            B = inputAI.B;
            X = inputAI.X;
            Y = inputAI.Y;
            R = inputAI.R;
            L = inputAI.L;
        }
        else {
            switch (Main.controller[(int)(playerNum - 1)]) {
                case Main.Controller.Elecom:
                    if (Mathf.Abs(Input.GetAxis("DPad_XAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("L_XAxis_" + (int)playerNum)) && Mathf.Abs(Input.GetAxis("DPad_XAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_XAxis_" + (int)playerNum))) {
                        xAxis = Input.GetAxis("DPad_XAxis_" + (int)playerNum);
                    } else if (Mathf.Abs(Input.GetAxis("L_XAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_XAxis_" + (int)playerNum))) {
                        xAxis = Input.GetAxis("L_XAxis_" + (int)playerNum);
                    } else {
                        xAxis = Input.GetAxis("R_XAxis_" + (int)playerNum);
                    }
                    if (Mathf.Abs(Input.GetAxis("DPad_YAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("L_YAxis_" + (int)playerNum)) && Mathf.Abs(Input.GetAxis("DPad_YAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_YAxis_" + (int)playerNum))) {
                        yAxis = Input.GetAxis("DPad_YAxis_" + (int)playerNum);
                    } else if (Mathf.Abs(Input.GetAxis("L_YAxis_" + (int)playerNum)) > Mathf.Abs(Input.GetAxis("R_YAxis_" + (int)playerNum))) {
                        yAxis = Input.GetAxis("L_YAxis_" + (int)playerNum);
                    } else {
                        yAxis = Input.GetAxis("R_YAxis_" + (int)playerNum);
                    }
                    A = (Input.GetButton("ButtonA_" + (int)playerNum) || Mathf.Abs(Input.GetAxis("R_XAxis_" + (int)playerNum)) > 0 || Mathf.Abs(Input.GetAxis("R_YAxis_" + (int)playerNum)) > 0);
                    B = Input.GetButton("ButtonB_" + (int)playerNum);
                    Y = Input.GetButton("ButtonY_" + (int)playerNum);
                    X = Input.GetButton("ButtonX_" + (int)playerNum);
                    R = Input.GetButton("ButtonR_" + (int)playerNum) || Input.GetButton("ButtonZR_" + (int)playerNum);
                    L = Input.GetButton("ButtonL_" + (int)playerNum) || Input.GetButton("ButtonZL_" + (int)playerNum); break;
                case Main.Controller.Joycon:
                    xAxis = (playerNum == PlayerNum.player1 ? -1 : 1) * Main.joycon[(int)(playerNum - 1)].GetStick()[1];
                    yAxis = (playerNum == PlayerNum.player1 ? 1 : -1) * Main.joycon[(int)(playerNum - 1)].GetStick()[0];
                    A = Main.joycon[(int)(playerNum - 1)].GetButton(playerNum == PlayerNum.player1 ? Joycon.Button.DPAD_DOWN: Joycon.Button.DPAD_UP);
                    B = Main.joycon[(int)(playerNum - 1)].GetButton(playerNum == PlayerNum.player1 ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT);
                    X = Main.joycon[(int)(playerNum - 1)].GetButton(playerNum == PlayerNum.player1 ? Joycon.Button.DPAD_RIGHT : Joycon.Button.DPAD_LEFT);
                    Y = Main.joycon[(int)(playerNum - 1)].GetButton(playerNum == PlayerNum.player1 ? Joycon.Button.DPAD_UP : Joycon.Button.DPAD_DOWN);
                    R = Main.joycon[(int)(playerNum - 1)].GetButton(Joycon.Button.SR);
                    L = Main.joycon[(int)(playerNum - 1)].GetButton(Joycon.Button.SL); break;
                default: break;
            }
        }

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //カウンターリセット
        if (stateInfo.fullPathHash != preStateInfo.fullPathHash) counter = 0;

        //AI強化用(仮)
        dv = enemyTf.position - playerTf.position;
        dx = Mathf.Min(Mathf.Abs((int)dv.x), InputAI.xDataNum-1);
        dy = ((int)Mathf.Abs(dv.y) < (int)Mathf.Floor(InputAI.yDataNum / 2) ? (int)dv.y : (dv.y < 0 ? -1 : 1) * (int)Mathf.Floor(InputAI.yDataNum / 2)) + (int)Mathf.Floor(InputAI.yDataNum / 2);

        //状態分岐
        if (stateInfo.fullPathHash == Prepare) {}
        else if (stateInfo.fullPathHash == StartGame) {
            if (counter == 0) {
                BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
                animator.speed = Main.Instance.gameSpeed;
            }
        }
        else if (stateInfo.fullPathHash == Idle) {
            if (animator.GetBool(ButtonL) && !isLimitBreak && 100 <= mp) {
                animator.Play("LimitBreak");
                mp = 0;
            }
        }
        else if (stateInfo.fullPathHash == Dash) {
            playerTf.position += Vector3.right * dashspeed * xAxis * animator.speed;
            Teach(0);
        }
        else if (stateInfo.fullPathHash == JumpStart) {
            if (counter == 0) Teach(1);
        }
        else if (stateInfo.fullPathHash == Fall) {
            playerTf.position += Vector3.down * counter * 0.1f * animator.speed;
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == Landing) {
            if (counter == 0) {
                BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
                if(Main.controller[(int)playerNum-1] == Main.Controller.Joycon && !isAI) {//
                    Main.joycon[(int)playerNum - 1].SetRumble(160, 320, 0.3f, 100);
                }
            }
        }
        else if (stateInfo.fullPathHash == LightningStart) {
            if (counter == 0) {
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
                Teach(2);
            }
        }
        else if (stateInfo.fullPathHash == Lightning) {
            if (counter == 0) {
                BattleMgr.Instance.VibrateDouble(0.8f, 1.0f);
                BattleMgr.Instance.CreateVFX("HitWave", playerTf.position, Quaternion.identity, 1.0f);
                GameObject g = BattleMgr.Instance.CreateVFX("LandKick", playerTf.position, Quaternion.identity, 1.0f);
                if (0 < playerTf.localScale.x) g.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (counter <= 5) {
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + Vector3.up,Quaternion.identity, 1.0f);
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
                lightningPos = 5 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right);
                Vector3 ofs = Vector3.zero;
                if (xAxis > 0) ofs = 1 * Vector3.right;
                else if (xAxis < 0) ofs = 1 * Vector3.left;
                if (yAxis > 0) ofs = 5 * Vector3.up;
                else if (yAxis < 0) ofs = 5 * Vector3.down;
                playerTf.position += (enemyTf.position + lightningPos + ofs - playerTf.position) / (counter == 5 ? 1:2);            
            }
            else playerTf.position += new Vector3(enemyController.damageVector.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == LightningAttack) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, 0, 0);
        }
        else if (stateInfo.fullPathHash == LightningEnd) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, 0, 0);
        }
        else if (stateInfo.fullPathHash == LightningAttackDown) {
            playerTf.position += new Vector3(enemyController.damageVector.x * Time.timeScale, - (counter * 0.2f) * animator.speed, 0);
        }
        else if (stateInfo.fullPathHash == NutralA) {
            if (counter == 0) {
                Teach(3);
            }
        }
        else if (stateInfo.fullPathHash == NutralA_Air) {
            if (counter == 0) {
                Teach(3);
            }
        }
        else if (stateInfo.fullPathHash == NutralB) {
            if (counter == 0) {
                Teach(4);
            }
        }
        else if (stateInfo.fullPathHash == NutralB_Air) {
            if (counter == 0) {
                Teach(4);
            }     
        }
        else if (stateInfo.fullPathHash == SideA) {
            if(counter == 0) {
                Teach(5);
            }
            if (counter == 10) {
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, Quaternion.identity, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == SideA_Air) {
            if (counter == 0) {
                Teach(5);
            }
            if (counter == 8) {
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, Quaternion.identity , 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == SideB) {
            if (counter == 0) {
                Teach(6);
            }
            if(counter == 13) BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
        else if (stateInfo.fullPathHash == SideB_Air) {
            if(counter == 0) {
                Teach(6);
            }
            playerTf.position += Vector3.down * counter * 0.03f * animator.speed;
            if (playerTf.position.y < 0.05f && !animator.GetBool(isLand)) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
                Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
            }
        }
        else if (stateInfo.fullPathHash == DownA) {
            if (counter == 0) {
                StartCoroutine(character.DownA());
                Teach(7);
            }
        }
        else if (stateInfo.fullPathHash == DownA_Air) {
            if (counter == 0) {
                StartCoroutine(character.DownA());
                Teach(7);
            }
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.05f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            }
        }
        else if (stateInfo.fullPathHash == DownB) {
            if (counter == 0) {
                StartCoroutine(character.DownB());
                Teach(8);
            }
            if (counter % 4 == 0 && counter <= 8) {
                BattleMgr.Instance.CreateVFX("FallSunder", playerTf.position, Quaternion.identity, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == DownB_Air_Fall) {
            if (counter == 0) {
                if (Mathf.Abs(enemyTf.position.x -playerTf.position.x) <= 1.5f) {
                    BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                    BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                    BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                    BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
                }
                Teach(8);
            }
            if (counter % 3 == 0 && counter <= 9) {
                BattleMgr.Instance.CreateVFX("CriticalDownWave", playerTf.position + Vector3.up * 2, Quaternion.identity, 1.0f);
            }
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.05f && !animator.GetBool(isLand)) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
                BattleMgr.Instance.CreateVFX("LandWave", playerTf.position, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position, Quaternion.identity, 1.0f);
                BattleMgr.Instance.VibrateDouble(1.0f, 1.5f);
            }
        }
        else if (stateInfo.fullPathHash == UpA) {
            if (counter == 0) {
                Teach(9);
            }
        }
        else if (stateInfo.fullPathHash == UpB) {
            if (counter == 0) {
                //BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, 1.0f);
                Teach(10);
            }
        }
        else if (stateInfo.fullPathHash == UpB_Fall) {
            if (counter == 0) {
                if (enemyController.stateInfo.fullPathHash == CriticalUp ||
                    enemyController.stateInfo.fullPathHash == CriticalFall) {
                    BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                    BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                    BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                    BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
                    BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, Quaternion.identity, 1.0f);
                }
            }
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
            if (playerTf.position.y < 0.05f && !animator.GetBool(isLand)) { //着地寸前
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
                Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], new Vector3(playerTf.position.x, 0, 0), Quaternion.identity);
            }
        }
        else if (stateInfo.fullPathHash == Critical) { //クリティカル
            if (counter == 0) {
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
                BattleMgr.Instance.ChangeTimeScale(0.05f, 0.5f);
                BattleMgr.Instance.ChangeToneDouble(0.5f, ((int)playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
                BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + new Vector3((enemyTf.position.x < playerTf.position.x ? 3 : -3), 1, 0), Quaternion.identity, 1.0f);
                if (hp < maxhp * 0.25f ) BattleMgr.Instance.CreateCrack(damageVector.x < 0);
                if (hp <= 0) Main.battleResult = Main.BattleResult.Finish;
            }
            playerTf.position += damageVector * Time.timeScale;
            playerTf.position += Vector3.down * counter * 0.002f * animator.speed;
            if (playerTf.position.y < 0.1f && 40 < counter * animator.speed) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                if (hp <= 0)
                    animator.Play("Death");
                else
                    animator.Play("CriticalEnd");
            }
        }
        else if (stateInfo.fullPathHash == CriticalEnd ||
            stateInfo.fullPathHash == CriticalFallEnd ||
            stateInfo.fullPathHash == CriticalNA) {
            damageVector *= 0.9f;
            playerTf.position += new Vector3(damageVector.x, 0, 0);
            if (Mathf.Abs(damageVector.x) < 0.1f && 20 <= counter) {
                damageVector = Vector3.zero;
                animator.Play("Idle");
            }
        }
        else if (stateInfo.fullPathHash == CriticalUp) {
            if (counter == 0) {
                BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
            }
            playerTf.position += new Vector3(damageVector.x, 0, 0);
        }
        else if (stateInfo.fullPathHash == CriticalFall) {
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
        else if (stateInfo.fullPathHash == CriticalDown) {
            if (counter == 0) {
                BattleMgr.Instance.ChangeToneDouble(0.1f, ((int)playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
                BattleMgr.Instance.CreateVFX("CriticalDownWave", playerTf.position + Vector3.up * 2.0f, Quaternion.identity, 1.0f);
            }
            playerTf.position = new Vector3(playerTf.position.x + damageVector.x, playerTf.position.y + damageVector.y, 0) * animator.speed;
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.CreateVFX("LandWave", playerTf.position, Quaternion.identity, 1.0f);
                BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
                animator.Play("CriticalUp");
            }
        }
        else if (stateInfo.fullPathHash == Wince) {
            playerTf.position += new Vector3(damageVector.x, -counter * 0.1f * animator.speed, 0);
            if (playerTf.position.y < 0.05f && !animator.GetBool("isLand")) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                BattleMgr.Instance.VibrateDouble(1.0f, 1.0f);
            }
        }
        else if (stateInfo.fullPathHash == LimitBreak) {
            if (counter == 0) {
                isLimitBreak = true;
                StartCoroutine(character.LimitBreak());
                 if (isTeacher && enemyController.isAI) { //入力回数が少ないので別処理
                    enemyController.inputAI.inputValues[11].deltaX[dx] += 10;
                    enemyController.inputAI.inputValues[11].deltaY[dy] += 10;
                 }
            }
        }
        if (stateInfo.fullPathHash != LimitBreak && preStateInfo.fullPathHash == LimitBreak) {
            BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
            character.LimitBreakEnd();
            playerTf.position = new Vector3(playerTf.position.x, 0, playerTf.position.z);
            animator.speed *= 1.2f;
        }
        if (stateInfo.fullPathHash == SideA_R || //鍔迫り合い
            stateInfo.fullPathHash == NutralA_R || 
            stateInfo.fullPathHash == SideB_R || 
            stateInfo.fullPathHash == SideA_Air_R ||
            stateInfo.fullPathHash == NutralA_Air_R) {
            if (counter == 0 && playerNum == PlayerNum.player1)  BattleMgr.Instance.StartResistance();
            if (A || B) {
                if (playerNum == PlayerNum.player1) BattleMgr.Instance.resistCounter1P++;
                if (playerNum == PlayerNum.player2) BattleMgr.Instance.resistCounter2P++;
            }
            if (counter == 20) {
                BattleMgr.Instance.VibrateDouble(0.3f, 2.0f);
                BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (enemyTf.position - playerTf.position) / 2 +Vector3.up * 2, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("Line", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.Euler(0,0, Random.Range(-70,70)), 1.0f);
            }
            if (counter == 40) {
                BattleMgr.Instance.VibrateDouble(0.4f, 2.0f);
                BattleMgr.Instance.CreateVFX("Stone", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("Line", playerTf.position + (enemyTf.position - playerTf.position) / 2 + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-70, 70)), 1.0f);
            }
            if (counter == 60) {
                isResistance = false;
                if ((BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical1P && playerNum == PlayerNum.player1) ||
                    (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical2P && playerNum == PlayerNum.player2)) { //鍔迫り合いに負けた時
                    BattleMgr.Instance.CreateVFX("Hit", playerTf.position, Quaternion.identity, 1.0f);
                    BattleMgr.Instance.CreateVFX("HitWave", playerTf.position, Quaternion.identity, 1.0f);
                    hp -= resistDamage * 2.0f;
                    mp += resistDamage;
                    resistDamage = 0;
                    animator.Play("Critical");
                }
                else if (BattleMgr.Instance.resistResult != BattleMgr.ResistResult.Wince) { //勝った時
                    damageVector = Vector3.zero;
                    mp += resistDamage * 0.5f;
                    resistDamage = 0;
                    if (stateInfo.fullPathHash == NutralA_R) animator.Play("NutralA_RW");
                    else if (stateInfo.fullPathHash == NutralA_Air) animator.Play("NutralA_Air_R");
                    else if (stateInfo.fullPathHash == SideA_R) animator.Play("SideA_RW");
                    else if (stateInfo.fullPathHash == SideA_Air_R) animator.Play("SideA_Air_RW");
                    else if (stateInfo.fullPathHash == SideB_R) animator.Play("SideB_RW");
                    else if (stateInfo.fullPathHash == SideB_Air_R) animator.Play("SideB_Air_RW");              
                    else {
                        animator.Play("SideA_RW");
                        Debug.Log("NoneRW");
                    }
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
        if (stateInfo.fullPathHash == GameEnd) {
            if (counter == 0) {
                BattleMgr.Instance.BattleEnd();
                if (isAI && enemyController.isTeacher) {
                    inputAI.UpdateCSV();
                }
                else if (isTeacher && enemyController.isAI) {
                    enemyController.inputAI.UpdateCSV();
                }
            }
        }

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
            if (0 < playerTf.position.y) playerTf.position += Vector3.right * (vectorspeed * vector.x + airspeed * xAxis) * animator.speed;
        }

        //反転
        if (stateInfo.fullPathHash == StartGame ||
            stateInfo.fullPathHash == Idle ||
            stateInfo.fullPathHash == CriticalUp ||
            stateInfo.fullPathHash == CriticalFall) {
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
        }
        else if (stateInfo.fullPathHash == Step ||
            stateInfo.fullPathHash == StepEnd ||
            stateInfo.fullPathHash == DashEnd ||
            stateInfo.fullPathHash == JumpStart ||
            stateInfo.fullPathHash == Jump ||
            stateInfo.fullPathHash == ShortJump ||
            stateInfo.fullPathHash == JumpEnd ||
            stateInfo.fullPathHash == Fall ) {
            if (xAxis > 0) playerTf.localScale = new Vector3(1, 1, 1);
            if (xAxis < 0) playerTf.localScale = new Vector3(-1, 1, 1);
        }

        if (Main.battleResult == Main.BattleResult.Battle) {
            animator.SetBool(UpArrow, yAxis > 0);
            animator.SetBool(RightArrow, xAxis > 0);
            animator.SetBool(LeftArrow, xAxis < 0);
            animator.SetBool(DownArrow, yAxis < 0);

            animator.SetBool(ButtonA, A);
            animator.SetBool(ButtonB, B);
            animator.SetBool(ButtonY, Y);
            animator.SetBool(ButtonX, X);
            animator.SetBool(ButtonR, R);
            animator.SetBool(ButtonL, L);
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

    void Teach(int n) {
        if (isTeacher && enemyController.isAI) { //強化用
            enemyController.inputAI.inputValues[n].deltaX[dx] += 1;
            enemyController.inputAI.inputValues[n].deltaY[dy] += 1;
        }
    }
}