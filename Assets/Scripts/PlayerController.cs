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

    public float maxhp = 100.0f;
    public float hp = 100.0f;
    public bool isDamaged = false;
    public bool isCriticaled = false; //開始時1フレームのみ
    public bool isResistance = false; //同上

    int counter = 0;

    public ChaseCamera chaseCamera;
    public CameraEffect cameraEffect;
    public Slider hpBar;

    public Transform playerTf;

    public PlayerController enemyController;
    public Transform enemyTf;

    AnimatorStateInfo preStateInfo;
    AnimatorStateInfo stateInfo;
    public Animator animator;

    public float dashspeed;
    public float airspeed;
    public float vectorspeed;

    float xAxisD;
    float yAxisD;
    Vector3 prePos;
    Vector3 vector;
    public Vector3 damageVector = Vector3.zero;

    void Awake() { 
        //キャラの生成
        switch (myChara) {
            case MyChara.Sword:
                characterIns = Instantiate(swordPref, playerNum == PlayerNum.player1? new Vector3(-15,0,0): new Vector3(15,0,0), new Quaternion(0, 0, 0, 0));
                character = characterIns.GetComponent<Sword>();
                playerTf = characterIns.transform;
                chaseCamera.playerTf = characterIns.transform;
                maxhp = Sword.maxhp;
                break;
            default:
                break;
        }
    }

    void Start() {
        //参照の取得
        hp = maxhp;
        character.playerController = this;
        animator = characterIns.GetComponent<Animator>();
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
                cameraEffect.Vibrate(0.8f, 2.0f);
            }
        }
        if (stateInfo.IsName("Dash")) {
            playerTf.position += Vector3.right * dashspeed * xAxisD;
        }
        if (stateInfo.IsName("Fall")) {
            if (!preStateInfo.IsName("Fall")) {
                counter = 0;
            }
            counter++;
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.1f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            }
        }
        if (!stateInfo.IsName("Fall")) {
            if (preStateInfo.IsName("Fall")) {
                counter = 0;
            }
        }
        if (stateInfo.IsName("Landing")) {
            if (!preStateInfo.IsName("Landing")) {
                battleMgr.VibrateDouble(0.5f, 0.5f);
            }
        }
        if (stateInfo.IsName("LightningStart")) {
            if (!preStateInfo.IsName("LightningStart")) {
                battleMgr.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            }
        }
        if (stateInfo.IsName("Lightning")) {
            if (!preStateInfo.IsName("Lightning")) {
                battleMgr.VibrateDouble(0.8f, 1.0f);
            }
            playerTf.position += (enemyTf.position + 3 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right) - playerTf.position) / 2; //相手の3m前に移動
            playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
        }
        if (stateInfo.IsName("LightningAttack")) {
            if (!preStateInfo.IsName("LightningAttack")) {
                character.LightningAttack();
            }
        }
        if (stateInfo.IsName("SideA")) {
            if (!preStateInfo.IsName("SideA")) {
                character.SideA();
            }
        }
        if (stateInfo.IsName("DownA")) {
            if (!preStateInfo.IsName("DownA")) {
                counter = 0;
                character.DownA();
            }
            counter++;
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            }
            if(counter == 14) {
                battleMgr.VibrateDouble(0.8f, 2.0f);
            }
        }
        if (!stateInfo.IsName("DownA")) {
            if (preStateInfo.IsName("DownA")) {
                counter = 0;
            }
        }


        if (stateInfo.IsName("Critical")) {
            if (isCriticaled) {//1フレームだけ呼ばれる
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
                battleMgr.ChangeTimeScale(0.0f, 0.5f);
                battleMgr.ChangeToneDouble(0.5f, ((int)playerNum == 2? CameraEffect.ToneName.redBlack: CameraEffect.ToneName.blueBlack));
                battleMgr.ZoomInOutDouble(0.1f);
                isCriticaled = false;
                counter = 0;
            }
            if(Time.timeScale == 1.0f) {
                playerTf.position += damageVector;
                counter++;
                playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.01f), 0);
                if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                    playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                }
            }
        }
        if (!stateInfo.IsName("Critical")) {
            if (preStateInfo.IsName("Critical")) {
                counter = 0;
            }
        }
        if (stateInfo.IsName("CriticalEnd")) {
            if (!preStateInfo.IsName("CriticalEnd")) {
                counter = 0;
            }
            counter++;
            playerTf.position += damageVector;
            damageVector = 0.9f * damageVector;
        }
        if (!stateInfo.IsName("CriticalEnd")) {
            if (preStateInfo.IsName("CriticalEnd")) {
                counter = 0;
            }
        }

        animator.SetBool("isResistance", isResistance);//
        if (isResistance) {
            counter++;
            animator.speed = 1.0f;
            if(30 < counter) {
                isResistance = false;
            }
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
        animator.SetBool("isWince", false);
        animator.SetBool("isCritical", isCriticaled);

        prePos = playerTf.position;
        preStateInfo = stateInfo;
    }
}
