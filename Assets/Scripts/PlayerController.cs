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
    public enum MyChara {
        Sword,
        Fighter
    }
    public MyChara myChara = MyChara.Sword;
    public GameObject swordPref;
    public GameObject fighterPref;
    public GameObject trailPref;
    public GameObject characterIns;
    Character character;

    public float maxhp = 100.0f;
    public float hp = 100.0f;

    public int counter = 0;

    public ChaseCamera chaseCamera;
    public CameraEffect cameraEffect;

    public Transform playerTf;

    public PlayerController enemyController;
    public Transform enemyTf;

    AnimatorStateInfo preStateInfo;
    public AnimatorStateInfo stateInfo;
    public Animator animator;

    public float dashspeed;
    public float airspeed;
    public float vectorspeed;

    float xAxisD;
    float yAxisD;
    Vector3 prePos;
    Vector2 vector;
    public Vector3 damageVector = Vector3.zero;

    public bool isResistance = false;
    bool isLimitBreak = false;

    [SerializeField] SoundMgr soundMgr;

    void Awake() { 
        //キャラの生成
        switch (myChara) {
            case MyChara.Sword:
                characterIns = Instantiate(swordPref, playerNum == PlayerNum.player1 ? new Vector3(-15,0,0): new Vector3(15,0,0), new Quaternion(0, 0, 0, 0));
                character = characterIns.GetComponent<Sword>();
                maxhp = Sword.maxhp;
                break;
            case MyChara.Fighter:
                characterIns = Instantiate(fighterPref, playerNum == PlayerNum.player1 ? new Vector3(-15, 0, 0) : new Vector3(15, 0, 0), new Quaternion(0, 0, 0, 0));
                character = characterIns.GetComponent<Fighter>();
                maxhp = Fighter.maxhp;
                break;
            default:
                break;
        }

        playerTf = characterIns.transform;
        chaseCamera.playerTf = characterIns.transform;
    }

    void Start() {
        hp = maxhp;
        character.playerController = this;
        animator = characterIns.GetComponent<Animator>();
        enemyTf = enemyController.characterIns.transform;
    }

    void Update() {
        vector = playerTf.position - prePos;

        xAxisD = Input.GetAxis("DPad_XAxis_" + (int)playerNum);
        yAxisD = Input.GetAxis("DPad_YAxis_" + (int)playerNum);

        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //カウンターリセット
        if (stateInfo.fullPathHash != preStateInfo.fullPathHash) {
            counter = 0;
        }

        //状態分岐
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Side.SideA")){
            if(counter == 0) {
                Debug.Log("SideA");
            }
        }
        if (stateInfo.IsName("Start")) {
            if (counter == 0) {
                BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            }
        }
        if (stateInfo.IsName("Idle")) {
            if(animator.GetBool("ButtonL") && !isLimitBreak) {
                animator.Play("LimitBreak");
            }
        }
        if (stateInfo.IsName("Dash")) {
            playerTf.position += Vector3.right * dashspeed * xAxisD;
        }
        if (stateInfo.IsName("Fall")) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.1f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            }
        }
        if (stateInfo.IsName("Landing")) {
            if (counter == 0) {
                BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
            }
        }
        if (stateInfo.IsName("LightningStart")) {
            if (counter == 0) {
                BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone);
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);             
            }
        }
        if (stateInfo.IsName("Lightning")) {
            if (counter == 0) {
                BattleMgr.Instance.VibrateDouble(0.8f, 1.0f);
                GameObject g = Instantiate(trailPref, new Vector3(playerTf.position.x, playerTf.position.y + 1.5f, 0), Quaternion.identity);
                g.transform.parent = playerTf;
            }
            if(counter <= 10) {
                playerTf.position += (enemyTf.position + 3 * (enemyTf.position.x > playerTf.position.x ? Vector3.left : Vector3.right) - playerTf.position) / 2; //相手の3m前に移動
                playerTf.localScale = enemyTf.position.x > playerTf.position.x ? Vector3.one : new Vector3(-1, 1, 1);
            }
        }
        if (stateInfo.IsName("LightningAttack")) {
        }
        if (stateInfo.IsName("SideA")) {
        }
        if (stateInfo.IsName("DownA")) {
            playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f), 0);
            if (playerTf.position.y < 0.1f && playerTf.position.y != 0) {
                playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            }
            if(counter == 14) {
                BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            }
        }
        if (stateInfo.IsName("Critical")) {
            if (counter == 0) {
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
            }
            if(Time.timeScale == 1.0f) {
                playerTf.position += damageVector;
                playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.005f), 0);
                if (playerTf.position.y < 0.1f && 60 < counter) {
                    playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                    animator.Play("CriticalEnd");
                }
            }
        }
        if (stateInfo.IsName("CriticalFall")) {
            if (counter == 0) {
                playerTf.localScale = damageVector.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;
                BattleMgr.Instance.ChangeTimeScale(0.0f, 0.5f);
                BattleMgr.Instance.ChangeToneDouble(0.5f, ((int)playerNum == 2 ? CameraEffect.ToneName.redBlack : CameraEffect.ToneName.blueBlack));
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
            }
            if (Time.timeScale == 1.0f) {
                playerTf.position += damageVector;
                playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.05f), 0);
                if (playerTf.position.y < 0.1f) {
                    playerTf.position = new Vector3(playerTf.position.x, 0, 0);
                    animator.Play("CriticalEnd");
                }
            }
        }
        if (stateInfo.IsName("CriticalEnd")) {
            //playerTf.position += new Vector3(damageVector.x, 0,0);
            //damageVector = 0.9f * damageVector;
        }
        if (stateInfo.IsName("LimitBreak")) {
            switch (counter) {
                case 0:
                    isLimitBreak = true;
                    BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone); break;
                case 20:
                    BattleMgr.Instance.VibrateDouble(0.8f, 1.0f); break;
                case 40:
                    soundMgr.PlaySFX(SoundMgr.Clip.LimitBreak);
                    BattleMgr.Instance.ChangeToneDouble(0.1f, CameraEffect.ToneName.reverseTone); break;
                case 60:
                    BattleMgr.Instance.VibrateDouble(1.5f, 2.0f);
                    BattleMgr.Instance.ChangeToneDouble(0.2f, CameraEffect.ToneName.whiteWhite); break;
                case 70:
                    BattleMgr.Instance.ChangeToneDouble(3.0f, CameraEffect.ToneName.reverseTone); break;               
                default: break;
            }
        }
        if (!stateInfo.IsName("LimitBreak")) {
            if (preStateInfo.IsName("LimitBreak")) {
                BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
                character.LimitBreak();
                playerTf.position = new Vector3(playerTf.position.x, 0, playerTf.position.z);
                animator.speed = 1.2f;
            }
        }
        if (stateInfo.IsName("SideA_R") || stateInfo.IsName("NutralA_R")) {
            if (counter == 0) {
                BattleMgr.Instance.resistCounter1P = 0;
                BattleMgr.Instance.resistCounter2P = 0;
                BattleMgr.Instance.ChangeTimeScale(0.0f, 0.3f);
                BattleMgr.Instance.ChangeToneDouble(2.0f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ZoomInOutDouble(0.1f);
            }
            if (playerNum == PlayerNum.player1 && Input.GetButtonDown("ButtonA_" + (int)playerNum)) {
                BattleMgr.Instance.resistCounter1P++;
            }
            else if (Input.GetButtonDown("ButtonA_" + (int)playerNum)) {
                BattleMgr.Instance.resistCounter2P++;
            }    
            if(counter == 60) {
                isResistance = false;
                if (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical1P) {
                    if(playerNum == PlayerNum.player1) {
                        BattleMgr.Instance.ChangeTimeScale(0.0f, 0.5f);
                        BattleMgr.Instance.ChangeToneDouble(0.5f, CameraEffect.ToneName.redBlack);
                        animator.Play("Critical");
                    }
                    else {
                        animator.Play("Idle");//
                    }
                }
                else if (BattleMgr.Instance.resistResult == BattleMgr.ResistResult.Critical2P) {
                    if(playerNum == PlayerNum.player2) {
                        BattleMgr.Instance.ChangeTimeScale(0.0f, 0.5f);
                        BattleMgr.Instance.ChangeToneDouble(0.5f, CameraEffect.ToneName.blueBlack);
                        animator.Play("Critical");
                    }
                    else {
                        animator.Play("Idle");//
                    }
                }
                else {
                    BattleMgr.Instance.ChangeToneDouble(0.0f, CameraEffect.ToneName.NormalTone);
                    animator.Play("CriticalEnd");
                }
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

        counter++;
        prePos = playerTf.position;
        preStateInfo = stateInfo;
    }
}
