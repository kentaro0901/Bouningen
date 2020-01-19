﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleMgr : MonoBehaviour {

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
        if (this != Instance) {
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

    int groundLeft = -5;
    int groundRight = 5;
    int groundSpan = 20;

    void Start() {
        Main.gameState = Main.GameState.Battle;
        Main.battleResult = Main.BattleResult.Battle;
        Main.Instance.BattleCameraSetting();
        camera1Tf = chaseCamera1.transform;
        camera2Tf = chaseCamera2.transform;
        player1Tf = playerController1.playerTf;
        player2Tf = playerController2.playerTf;
        //地面
        CreateGround(groundLeft, groundRight);
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
        if (Main.Instance.isDemo) {
            c1Txt.text = "Demo";
            c1Txt.color = new Color(c1Txt.color.r, c1Txt.color.b, c1Txt.color.g, 0.5f);
            c2Txt.text = "Demo";
            c2Txt.color = new Color(c2Txt.color.r, c2Txt.color.b, c2Txt.color.g, 0.5f);
        }
        counter = 0;
    }
    void Update() {
        UpdateUI();
        TimeScaleCountDown();
        AnimeSpeedCountDown();
        if (Main.gameState == Main.GameState.Result) {
            counter++;
            if ((Input.GetButtonDown("ButtonA_0") ||
            ((Main.controller[0] == Main.Controller.Joycon) ? Main.joycon[0].GetButtonDown(Joycon.Button.DPAD_DOWN) : false) ||
            ((Main.controller[1] == Main.Controller.Joycon) ? Main.joycon[1].GetButtonDown(Joycon.Button.DPAD_UP) : false)) && 60 <= counter) {
                ChangeTimeScale(1.0f, 0);
                Main.Instance.Init(true);
            }
            if (Main.Instance.isDemo && counter == 60) { //デモ繰り返し
                ChangeTimeScale(1.0f, 0);
                Main.Instance.chara[0] = (Main.Chara)Random.Range(0, 3);
                Main.Instance.chara[1] = (Main.Chara)Random.Range(0, 3);
                FadeManager.Instance.LoadScene("Battle", 0.5f);
            }
        }
        if(Main.Instance.isDemo && (Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("ButtonA_0") || Input.GetButtonDown("ButtonA_1"))) { //デモ終了
            Main.Instance.isDemo = false;
            Main.Instance.Init(true);
        }
    }
    void LateUpdate() {
        if (Main.gameState == Main.GameState.Battle) ChangeCameraChaseMode();
        ResistMgr();
        float left = player1Tf.position.x < player2Tf.position.x ? player1Tf.position.x : player2Tf.position.x;
        float right = player1Tf.position.x <= player2Tf.position.x ? player2Tf.position.x : player1Tf.position.x;
        if(left/groundSpan < groundLeft) {
            CreateGround((int)(left / groundSpan)-1, groundLeft-1);
        }
        if(groundRight < right / groundSpan) {
            CreateGround(groundRight + 1, (int)(right / groundSpan) + 1);
        }
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
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20) + 0.2f);
                    c2LFRTf.offsetMax = Vector2.right * Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20) + 0.2f);
                    c2RFRTf.offsetMin = Vector2.zero;
                }
                else { //1P右
                    c1LFRTf.offsetMax = Vector2.right * Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20) + 0.2f);
                    c1RFRTf.offsetMin = Vector2.zero;
                    c2LFRTf.offsetMax = Vector2.zero;
                    c2RFRTf.offsetMin = Vector2.right * -Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 6) / (ChaseCamera.chaseRange * 20) + 0.2f);
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
    public void CreateHibi(Vector3 position) {
        for(int i=1; i <= Random.Range(1,4); i++) {
            GameObject g = Instantiate(HibiPref[Random.Range(0, HibiPref.Length)], position, Quaternion.Euler(0, 0, Random.Range(-10, 10)*i));
            g.transform.localScale = Vector3.one * Random.Range(1.5f, 3.0f) / i;
        }
    }
    void CreateGround(int left, int right) {
        for (int i = left; i <= right; i++) {
            Instantiate(GrandPref[Random.Range(0, GrandPref.Length)], new Vector3(i * groundSpan, -1.5f, 0), Quaternion.identity);
        }
        if (left < groundLeft) groundLeft = left;
        if (groundRight < right) groundRight = right;
    }
    public void BattleEnd() {
        if(playerController1.stateInfo.fullPathHash == AnimState.GameEnd && playerController2.stateInfo.fullPathHash != AnimState.GameEnd) {
            Debug.Log("2PWIN");
            Main.battleResult = Main.BattleResult.Win2P;
            c1Txt.text = "LOSE";
            c2Txt.text = "WIN";
        }
        else if(playerController2.stateInfo.fullPathHash == AnimState.GameEnd && playerController1.stateInfo.fullPathHash != AnimState.GameEnd) {
            Debug.Log("1PWIN");
            Main.battleResult = Main.BattleResult.Win1P;
            c1Txt.text = "WIN";
            c2Txt.text = "LOSE";
        }
        else {
            Debug.Log("DRAW");
            Main.battleResult = Main.BattleResult.Draw;
            c1Txt.text = "DRAW";
            c2Txt.text = "DRAW";
        }
        c1Txt.color = new Color(c1Txt.color.r, c1Txt.color.b, c1Txt.color.g, 1.0f);
        c2Txt.color = new Color(c2Txt.color.r, c2Txt.color.b, c2Txt.color.g, 1.0f);
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
        Main.gameState = Main.GameState.Result;
    }
}
