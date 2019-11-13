using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleMgr : MonoBehaviour {

    //シングルトン
    private static BattleMgr instance;
    public static BattleMgr Instance {
        get {
            if (instance == null) {
                instance = (BattleMgr)FindObjectOfType(typeof(BattleMgr));
                if (instance == null)
                    Debug.LogError(typeof(BattleMgr) + "is nothing");
            }
            return instance;
        }
    }
    void Awake() {
        if (this != Instance) { //２つ目以降のインスタンスは破棄
            Destroy(this.gameObject);
            return;
        }
    }

    [SerializeField] ChaseCamera chaseCamera1;
    [SerializeField] ChaseCamera chaseCamera2;
    Transform camera1Tf;
    Transform camera2Tf;
    [SerializeField] CameraEffect cameraEffect1;
    [SerializeField] CameraEffect cameraEffect2;
    [SerializeField] PlayerController playerController1;
    [SerializeField] PlayerController playerController2;
    public Transform player1Tf;
    public Transform player2Tf;
    [SerializeField] RectTransform c1LFRTf;
    [SerializeField] RectTransform c1RFRTf;
    [SerializeField] RectTransform c2LFRTf;
    [SerializeField] RectTransform c2RFRTf;
    [SerializeField] Text c1Txt;
    [SerializeField] Text c2Txt;
    [SerializeField] RectTransform c1;
    [SerializeField] RectTransform c2;
    [SerializeField] Slider c1hpBar1;
    [SerializeField] Slider c1hpBar2;
    [SerializeField] Slider c2hpBar1;
    [SerializeField] Slider c2hpBar2;
    [SerializeField] Slider c1mpBar1;
    [SerializeField] Slider c1mpBar2;
    [SerializeField] Slider c2mpBar1;
    [SerializeField] Slider c2mpBar2;

    [SerializeField] GameObject[] GrandPref;
    [SerializeField] GameObject VFXPref;
    [SerializeField] GameObject[] CrackPref;
    [SerializeField] GameObject[] HibiPref;

    public int resistCounter1P = 0;
    public int resistCounter2P = 0;
    public enum ResistResult {
        Critical1P,
        Critical2P,
        Wince
    }
    public ResistResult resistResult = ResistResult.Wince;

    float timeScaleSeconds = 0.0f;
    float animeSpeedSeconds = 0.0f;

    int counter = 0;

    float preHP1;
    float preHP2;
    float preMP1;
    float preMP2;

    void Start() {
        Main.state = Main.State.Battle;
        Main.battleResult = Main.BattleResult.Battle;
        Main.Instance.BattleCameraSetting();
        //Main.Instance.CheckGamePad();
        camera1Tf = chaseCamera1.transform;
        camera2Tf = chaseCamera2.transform;
        player1Tf = playerController1.playerTf;
        player2Tf = playerController2.playerTf;
        for (int i = -150; i <= 150; i++){
            GameObject g = Instantiate(GrandPref[(int)Random.Range(0, GrandPref.Length)], new Vector3(i * 20, -1.5f, 0), Quaternion.identity);
        }
        preHP1 = playerController1.hp;
        preHP2 = playerController2.hp;
        if (Main.Instance.isVisibleUI) {
            c1hpBar1.value = c2hpBar1.value = playerController1.hp / playerController1.maxhp;
            c1hpBar2.value = c2hpBar2.value = playerController2.hp / playerController2.maxhp;
            c1mpBar1.value = c2mpBar1.value = playerController1.mp / 100;
            c1mpBar2.value = c2mpBar2.value = playerController2.mp / 100;
        }
        else {
            c1hpBar1.gameObject.SetActive(false);
            c1hpBar2.gameObject.SetActive(false);
            c1mpBar1.gameObject.SetActive(false);
            c1mpBar2.gameObject.SetActive(false);
            c2hpBar1.gameObject.SetActive(false);
            c2hpBar2.gameObject.SetActive(false);
            c2mpBar1.gameObject.SetActive(false);
            c2mpBar2.gameObject.SetActive(false);
        }
        counter = 0;
    }
    void Update() {
        UpdateUI();
        TimeScaleCountDown();
        AnimeSpeedCountDown();
        if (Main.state == Main.State.Result) {
            counter++;
            if ((Input.GetButtonDown("ButtonA_0") ||
            ((Main.controller[0] == Main.Controller.Joycon) ? Main.joycon[0].GetButtonDown(Joycon.Button.DPAD_DOWN) : false) ||
            ((Main.controller[1] == Main.Controller.Joycon) ? Main.joycon[1].GetButtonDown(Joycon.Button.DPAD_UP) : false)) && 60 <= counter) {
                ChangeTimeScale(1.0f, 0);
                Main.Instance.Init(true);
            }
            if (60 <= counter && (playerController1.isLeveling || playerController2.isLeveling)) { //自動学習
                ChangeTimeScale(1.0f, 0);
                SceneManager.LoadScene("Battle");
            }
        }
    }
    void LateUpdate() {
        if (Main.state == Main.State.Battle) ChangeCameraChaseMode();
        ResistMgr();
    }

    private void UpdateUI() {
        if (Main.Instance.isVisibleUI) {
            if (preHP1 != playerController1.hp) StartCoroutine(EasingBar(c1hpBar1, c2hpBar1, preHP1, playerController1.hp, playerController1.maxhp));
            if (preHP2 != playerController2.hp) StartCoroutine(EasingBar(c1hpBar2, c2hpBar2, preHP2, playerController2.hp, playerController2.maxhp));
            if (preMP1 != playerController1.mp) StartCoroutine(EasingBar(c1mpBar1, c2mpBar1, preMP1, playerController1.mp, 100));            
            if (preMP2 != playerController2.mp) StartCoroutine(EasingBar(c1mpBar2, c2mpBar2, preMP2, playerController2.mp, 100));
        }
        if (preMP1 < 100 && 100 <= playerController1.mp) {
            GameObject g = instance.CreateVFX("MaxChargeR", player1Tf.position, Quaternion.identity, 1000);
            g.transform.parent = player1Tf;
        }
        if (preMP2 < 100 && 100 <= playerController2.mp) {
            GameObject g = instance.CreateVFX("MaxChargeB", player2Tf.position,Quaternion.identity ,1000);
            g.transform.parent = player2Tf;
        }
        preHP1 = playerController1.hp;
        preHP2 = playerController2.hp;
        preMP1 = playerController1.mp;
        preMP2 = playerController2.mp;
    }
    IEnumerator EasingBar(Slider c1slider, Slider c2slider, float pre, float now, float max) {
        float time = 0.0f;
        while (time <= 0.3f) {
            c1slider.value = c2slider.value = Easing.ExpOut(time, 0.3f, pre, now) / max;
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        yield return 0;
    }
    private void ResistMgr() {
        if(resistCounter1P - resistCounter2P >= 2) {
            resistResult = ResistResult.Critical2P;
        }
        else if(resistCounter2P - resistCounter1P >= 2) {
            resistResult = ResistResult.Critical1P;
        }
        else {
            resistResult = ResistResult.Wince;
        }
    }
    private void ChangeCameraChaseMode() {
        if (Main.Instance.isDynamicCamera) {
            if (Mathf.Abs(camera1Tf.position.x - camera2Tf.position.x) < ChaseCamera.chaseRange * 4) { //近距離になったとき
                if (!ChaseCamera.isNear) {
                    chaseCamera1.NearCamera();
                    chaseCamera2.NearCamera();
                }
                c1LFRTf.offsetMax = Vector2.zero;
                c1RFRTf.offsetMin = Vector2.zero;
                c2LFRTf.offsetMax = Vector2.zero;
                c2RFRTf.offsetMin = Vector2.zero;
            }
            if (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) >= ChaseCamera.chaseRange * 6) { //遠距離
                if (ChaseCamera.isNear) {
                    chaseCamera1.FarCamera(player1Tf.position.x < player2Tf.position.x);
                    chaseCamera2.FarCamera(player1Tf.position.x >= player2Tf.position.x);
                }
                if(player1Tf.position.x < player2Tf.position.x) {//1P左
                    c1LFRTf.offsetMax = Vector2.zero;
                    c1RFRTf.offsetMin = Vector2.right * -Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80) 
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20));
                    c2LFRTf.offsetMax = Vector2.right * Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20));
                    c2RFRTf.offsetMin = Vector2.zero;
                }
                else { //1P右
                    c1LFRTf.offsetMax = Vector2.right * Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20));
                    c1RFRTf.offsetMin = Vector2.zero;
                    c2LFRTf.offsetMax = Vector2.zero;
                    c2RFRTf.offsetMin = Vector2.right * -Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20));
                }        
            }
        }
    }
    public void ChangeTimeScale(float speed, float seconds) {
        Time.timeScale = speed;
        timeScaleSeconds = seconds;
    }
    private void TimeScaleCountDown() {
        if (timeScaleSeconds > 0) {
            timeScaleSeconds -= Time.unscaledDeltaTime;
        }
        else {
            timeScaleSeconds = 0.0f;
            Time.timeScale = 1.0f;
        }
    }
    public void ChangeAnimeSpeedDouble(float speed, float seconds) {
        playerController1.animator.speed = speed;
        playerController2.animator.speed = speed;
        animeSpeedSeconds = seconds;
    }
    private void AnimeSpeedCountDown() {
        if (animeSpeedSeconds > 0) {
            animeSpeedSeconds -= Time.unscaledDeltaTime;
        }
        else {
            animeSpeedSeconds = 0.0f;
            playerController1.animator.speed = Main.Instance.gameSpeed * (playerController1.isLimitBreak ? 1.2f : 1.0f);
            playerController2.animator.speed = Main.Instance.gameSpeed * (playerController2.isLimitBreak ? 1.2f : 1.0f);
        }
    }
    public void VibrateDouble(float seconds, float range) {
        cameraEffect1.Vibrate(seconds, range);
        cameraEffect2.Vibrate(seconds, range);
    }
    public void ZoomInOutDouble(float seconds) {
            cameraEffect1.ZoomInOut(seconds);
            cameraEffect2.ZoomInOut(seconds);
    }
    public void ChangeToneDouble(float seconds, CameraEffect.ToneName name) {
        cameraEffect1.ChangeTone(seconds, name);
        cameraEffect2.ChangeTone(seconds, name);
    }
    public void StartResistance() {
        resistCounter1P = 0;
        resistCounter2P = 0;
        ChangeTimeScale(0.05f, 0.5f);
        ChangeToneDouble(2.0f, CameraEffect.ToneName.reverseTone);
        Instance.ZoomInOutDouble(0.1f);
        CreateVFX("Sunder", player1Tf.position - (player1Tf.position - player2Tf.position) / 2 + Vector3.up, Quaternion.identity, 1.0f);
    }
    public GameObject CreateVFX(string name, Vector3 position, Quaternion rotation,float lifeTime) {
        GameObject vfx = Instantiate(VFXPref, position, rotation);
        vfx.GetComponent<Animator>().Play(name);
        vfx.GetComponent<DestroyParticle>().lifeTime = lifeTime;
        return vfx;
    }
    public GameObject CreateCrack(bool isLeftCanvas) {
        RectTransform r = Instantiate(CrackPref[Random.Range(0, CrackPref.Length)], Main.Instance.isMultiDisplays? (isLeftCanvas? c1: c2): c1).GetComponent<RectTransform>();
        r.localPosition = new Vector3(isLeftCanvas ? Random.Range(-Screen.width/2, -Screen.width/16*7) : Random.Range(Screen.width/16*7, Screen.width/2), 
            Random.Range(-Screen.height / 4, Screen.height / 4), 0);
        r.localRotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));
        return r.gameObject;
    }
    public void CreateHibi(Vector3 position ) {
        Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], position, Quaternion.identity);
    }
    public void BattleEnd() {
        if(playerController1.hp <= 0 && playerController2.hp > 0) {
            Main.battleResult = Main.BattleResult.Win2P;
            c1Txt.text = "LOSE";
            c2Txt.text = "WIN";
        }
        else if(playerController2.hp <= 0 && playerController1.hp > 0) {
            Main.battleResult = Main.BattleResult.Win1P;
            c1Txt.text = "WIN";
            c2Txt.text = "LOSE";
        }
        else {
            Main.battleResult = Main.BattleResult.Draw;
            c1Txt.text = "DRAW";
            c2Txt.text = "DRAW";
        }
        Destroy(c1LFRTf.gameObject);
        Destroy(c1RFRTf.gameObject);
        Destroy(c2LFRTf.gameObject);
        Destroy(c2RFRTf.gameObject);
        ChangeTimeScale(0, 1000);
        VibrateDouble(0.5f, 1.0f);
        cameraEffect1.ChangeTone(0, CameraEffect.ToneName.redBlack);
        cameraEffect2.ChangeTone(0, CameraEffect.ToneName.blueBlack);
        c1hpBar1.gameObject.SetActive(false);
        c1hpBar2.gameObject.SetActive(false);
        c1mpBar1.gameObject.SetActive(false);
        c1mpBar2.gameObject.SetActive(false);
        c2hpBar1.gameObject.SetActive(false);
        c2hpBar2.gameObject.SetActive(false);
        c2mpBar1.gameObject.SetActive(false);
        c2mpBar2.gameObject.SetActive(false);
        Main.state = Main.State.Result;
    }
}
