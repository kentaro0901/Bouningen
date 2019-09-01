using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [SerializeField] BattleMgr battleMgr;

    public enum PlayerNum {
        zero,
        player1,
        player2
    }
    public PlayerNum playerNum;
    public enum MyChara {
        Sword,
        Gun
    }
    public MyChara myChara = MyChara.Sword;
    public GameObject swordPref;
    GameObject characterIns;
    Character character; //characterInsに付いてるSwordとか

    public bool isVisibleBox = false; //isVisibleBoxを変更するならAwake

    float maxhp;
    public float hp = 100.0f;
    public bool isDamaged = false;
    public bool isCriticaled = false; //開始時1フレームのみ

    public ChaseCamera chaseCamera;
    public CameraEffect cameraEffect;
    public Slider hpBar;

    public Transform playerTf;

    public PlayerController enemyController;
    public Transform enemyTf;

    AnimatorStateInfo preStateInfo;
    AnimatorStateInfo stateInfo;
    Animator animator;

    public float dashspeed;
    public float airspeed;
    public float vectorspeed;

    float xAxisD;
    float yAxisD;
    Vector3 prePos;
    Vector3 vector;
    public Vector3 damageVector = Vector3.zero;

    void Awake() { 
        switch (myChara) {
            case MyChara.Sword:
                characterIns = Instantiate(swordPref, playerNum == PlayerNum.player1? new Vector3(-10,0,0): new Vector3(10,0,0), new Quaternion(0, 0, 0, 0));
                character = characterIns.GetComponent<Sword>();
                maxhp = Sword.maxhp;
                break;
            default:
                break;
        }
    }

    void Start() {
        hp = maxhp;
        chaseCamera.chaseTf = characterIns.transform;
        character.playerController = this;
        animator = characterIns.GetComponent<Animator>();
        playerTf = characterIns.transform;
        enemyTf = enemyController.characterIns.transform;
    }

    void Update() {
        vector = playerTf.position - prePos;
        hpBar.value = hp / maxhp;

        xAxisD = Input.GetAxis("DPad_XAxis_" + (int)playerNum);
        yAxisD = Input.GetAxis("DPad_YAxis_" + (int)playerNum);

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Start")) {
            if (!preStateInfo.IsName("Start")) {
                battleMgr.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                battleMgr.ZoomInOutDouble(0.05f);
                //battleMgr.ChangeTimeScale(0.5f, 1.0f);
                cameraEffect.Vibrate(0.8f, 2.0f);
            }
        }
        if (stateInfo.IsName("Dash")) {
            playerTf.position += Vector3.right * dashspeed * xAxisD;
        }
        if (stateInfo.IsName("LightningStart")) {
            if (!preStateInfo.IsName("LightningStart")) {
                cameraEffect.ChangeTone(0.1f, CameraEffect.ToneName.reverseTone);
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            }
        }
        if (stateInfo.IsName("Lightning")) {
            if (!preStateInfo.IsName("Lightning")) {
                cameraEffect.Vibrate(0.8f, 1.0f);
            }
            playerTf.position += (enemyTf.position + (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right) - playerTf.position) / 2;
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
        }
        if (stateInfo.IsName("LightningAttack")) {
            character.LightningAttack();
        }
        if (stateInfo.IsName("SideA")) character.SideA();

        if (stateInfo.IsName("Critical")) {
            if (isCriticaled) {//1フレームだけ呼ばれる
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
                battleMgr.ChangeTimeScale(0.0f, 1.0f);
                battleMgr.ChangeToneDouble(1.0f, CameraEffect.ToneName.redBlack);
                battleMgr.ZoomInOutDouble(0.05f);
                isCriticaled = false;
            }
            if(Time.timeScale == 1.0f) {
                    playerTf.position += damageVector;
            }
        }
        if (stateInfo.IsName("CriticalEnd")) {
            playerTf.position += damageVector;
            damageVector = 0.9f * damageVector;
        }

        //地面判定（仮）
        if (playerTf.position.y < 0) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }

        //空中制御&自動反転
        if(stateInfo.IsName("JumpStart") ||
            stateInfo.IsName("Jump") ||
            stateInfo.IsName("JumpEnd") ||
            stateInfo.IsName("Fall")) {
            playerTf.position += Vector3.right * (vectorspeed * vector.x + airspeed * xAxisD);
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);//
        }

        //自動反転
        if (stateInfo.IsName("Start") ||
            stateInfo.IsName("Idle")) {
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);//
        }

        //左右反転
        if (stateInfo.IsName("Step") ||
            stateInfo.IsName("StepEnd") ||
            stateInfo.IsName("DashEnd")) {
            if (Input.GetAxis("DPad_XAxis_" + (int)playerNum) > 0)
                playerTf.localScale = new Vector3(1, 1, 1);
            if (Input.GetAxis("DPad_XAxis_" + (int)playerNum) < 0)
                playerTf.localScale = new Vector3(-1, 1, 1);
        }


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

        animator.SetBool("isLand", playerTf.position.y <= 0);
        animator.SetBool("isRight", 0 < playerTf.localScale.x);
        animator.SetBool("isResistance", false);
        animator.SetBool("isWince", false);
        animator.SetBool("isCritical", isCriticaled);

        prePos = playerTf.position;
        preStateInfo = stateInfo;
    }
}
