using System.Collections;
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

    [System.Serializable]
    public class PlayerStatus { //各プレイヤーのステータス類
        public ChaseCamera chaseCamera;
        public CameraEffect cameraEffect;
        public PlayerController controller;
        public Transform cameraTf;
        public Transform playerTf;
        public float preHp;
        public int resistCount;
    }
    [System.Serializable]
    public class CanvasStatus { //各キャンバスのステータス類
        public RectTransform rect;
        public RectTransform leftFlame;
        public RectTransform rightFlame;
        public Text text;
        public Image[] hpBars;
        public Image[] reserveBars;
    }
    public PlayerStatus[] players;
    public CanvasStatus[] canvases;
    [SerializeField] GameObject[] GrandPrefs;
    [SerializeField] GameObject[] CrackPrefs;
    [SerializeField] GameObject[] HibiPrefs;
    [SerializeField] GameObject VFXPref;

    public enum BattleResult {
        Default,
        Battle,
        Finish,
        Draw,
        Win1P,
        Win2P
    }
    public enum ResistResult {
        Critical1P,
        Critical2P,
        Wince
    }
    public BattleResult battleResult = BattleResult.Default;
    public ResistResult resistResult = ResistResult.Wince;
    float timeScaleSeconds = 0.0f;
    float animeSpeedSeconds = 0.0f;
    int counter = 0;
    int groundLeft = -5;
    int groundRight = 5;
    int groundSpan = 20;

    void Start() {
        Main.gameState = Main.GameState.Battle;
        battleResult = BattleResult.Battle;
        Main.Instance.BattleCameraSetting();
        CreateGround(groundLeft, groundRight);
        counter = 0;
        for (int i = 0; i < 2; i++) {
            players[i].cameraTf = players[i].chaseCamera.transform;
            players[i].playerTf = players[i].controller.playerTf;
            players[i].preHp = players[i].controller.hp;
            if (Main.Instance.isDemo) {
                canvases[i].text.text = "Demo";
                Color c = canvases[i].text.color;
                canvases[i].text.color = new Color(c.r, c.g, c.b, 0.5f);
            }
            for (int j = 0; j < 2; j++) {
                if (Main.Instance.isVisibleUI) {
                    canvases[j].hpBars[i].fillAmount = canvases[j].reserveBars[i].fillAmount = players[i].controller.hp / players[i].controller.maxhp;                   
                }
                else {
                    canvases[j].hpBars[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
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
        float left = players[0].playerTf.position.x < players[1].playerTf.position.x ? players[0].playerTf.position.x : players[1].playerTf.position.x;
        float right = players[0].playerTf.position.x <= players[1].playerTf.position.x ? players[1].playerTf.position.x : players[0].playerTf.position.x;
        if(left/groundSpan < groundLeft) {
            CreateGround((int)(left / groundSpan)-1, groundLeft-1);
        }
        if(groundRight < right / groundSpan) {
            CreateGround(groundRight + 1, (int)(right / groundSpan) + 1);
        }
    }

    private void UpdateUI() {
        for(int i= 0; i < 2; i++) {
            if (Main.Instance.isVisibleUI && players[i].preHp != players[i].controller.hp) {
                canvases[0].hpBars[i].fillAmount = canvases[1].hpBars[i].fillAmount = players[i].controller.hp / players[i].controller.maxhp;
                StartCoroutine(EasingBar(i, players[i].preHp, players[i].controller.hp, players[i].controller.maxhp));
            }
            players[i].preHp = players[i].controller.hp;
        }       
    }
    IEnumerator EasingBar(int num, float pre, float now, float max) {
        yield return new WaitForSeconds(0.4f);
        float time = 0.0f;
        float f = canvases[0].reserveBars[num].fillAmount;
        while (time <= 0.5f) {
            canvases[0].reserveBars[num].fillAmount = canvases[1].reserveBars[num].fillAmount = Easing.ExpOut(time, 0.5f, pre, now)/max;
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        yield return 0;
    }
    private void ResistMgr() {
        if (players[0].resistCount - players[1].resistCount >= 2) {
            resistResult = ResistResult.Critical2P;
        }
        else if (players[1].resistCount - players[0].resistCount >= 2) {
            resistResult = ResistResult.Critical1P;
        }
        else {
            resistResult = ResistResult.Wince;
        }
    }
    private void ChangeCameraChaseMode() {
        if (Main.Instance.isDynamicCamera) {
            if (Mathf.Abs(players[0].cameraTf.position.x - players[1].cameraTf.position.x) < ChaseCamera.personalCameraHalfWidth * 2) { //近距離になったとき
                for(int i = 0; i < 2; i++) {
                    if (!ChaseCamera.isNear) {
                        players[i].chaseCamera.NearCamera();
                    }
                    canvases[i].leftFlame.offsetMax = Vector2.zero;
                    canvases[i].rightFlame.offsetMin = Vector2.zero;
                }            
            }
            if (Mathf.Abs(players[0].playerTf.position.x - players[1].playerTf.position.x) >= ChaseCamera.personalCameraHalfWidth*2 + ChaseCamera.chaseRange * 2) { //遠距離
                if (ChaseCamera.isNear) {
                    players[0].chaseCamera.FarCamera(players[0].playerTf.position.x < players[1].playerTf.position.x);
                    players[1].chaseCamera.FarCamera(players[0].playerTf.position.x >= players[1]. playerTf.position.x);
                }
                Vector2 v = Vector2.right * -Screen.width / (Main.Instance.isMultiDisplays ? 40 : 80)
                        * Mathf.Min(1.0f, (Mathf.Abs(players[0].playerTf.position.x - players[1].playerTf.position.x) - (ChaseCamera.personalCameraHalfWidth * 2 + ChaseCamera.chaseRange * 2)) / (ChaseCamera.personalCameraHalfWidth * 10) + 0.2f);
                bool isleft1P = players[0].playerTf.position.x < players[1].playerTf.position.x;
                canvases[0].leftFlame.offsetMax = isleft1P? Vector2.zero:v;
                canvases[0].rightFlame.offsetMin = isleft1P? v:Vector2.zero;
                canvases[1].leftFlame.offsetMax = isleft1P? v:Vector2.zero;
                canvases[1].rightFlame.offsetMin = isleft1P? Vector2.zero:v;     
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
        for(int i=0;i<2;i++) players[i].controller.animator.speed = speed;
        animeSpeedSeconds = seconds;
    }
    private void AnimeSpeedCountDown() {
        if (animeSpeedSeconds > 0) {
            animeSpeedSeconds -= Time.unscaledDeltaTime;
        }
        else {
            animeSpeedSeconds = 0.0f;
            for(int i=0;i<2;i++) players[i].controller.animator.speed = Main.Instance.gameSpeed * (players[i].controller.isLimitBreak ? 1.2f : 1.0f);
        }
    }
    public void VibrateDouble(float seconds, float range) {
        for (int i=0;i<2;i++) players[i].cameraEffect.Vibrate(seconds, range);
    }
    public void ZoomInOutDouble(float seconds) {
        for(int i=0;i<2;i++) players[i].cameraEffect.ZoomInOut(seconds);
    }
    public void ChangeToneDouble(float seconds, CameraEffect.ToneName name) {
        for (int i = 0; i < 2; i++) players[i].cameraEffect.ChangeTone(seconds, name);
    }
    public void StartResistance() {
        for(int i=0;i<2;i++) players[i].resistCount = 0;
        ChangeTimeScale(0.05f, 0.5f);
        ChangeToneDouble(2.0f, CameraEffect.ToneName.reverseTone);
        Instance.ZoomInOutDouble(0.1f);
        CreateVFX("Sunder", players[0].playerTf.position - (players[0].playerTf.position - players[1].playerTf.position) / 2 + Vector3.up, Quaternion.identity, 1.0f);
    }
    public GameObject CreateVFX(string name, Vector3 position, Quaternion rotation,float lifeTime) {
        GameObject vfx = Instantiate(VFXPref, position, rotation);
        vfx.GetComponent<Animator>().Play(name);
        vfx.GetComponent<DestroyParticle>().lifeTime = lifeTime;
        return vfx;
    }
    public GameObject CreateCrack(bool isLeftCanvas) {
        RectTransform r = Instantiate(CrackPrefs[Random.Range(0, CrackPrefs.Length)], Main.Instance.isMultiDisplays? (isLeftCanvas? canvases[0].rect: canvases[1].rect): canvases[0].rect).GetComponent<RectTransform>();
        r.localPosition = new Vector3(isLeftCanvas ? Random.Range(-Screen.width/2, -Screen.width/16*7) : Random.Range(Screen.width/16*7, Screen.width/2), 
            Random.Range(-Screen.height / 4, Screen.height / 4), 0);
        r.localRotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));
        return r.gameObject;
    }
    public void CreateHibi(Vector3 position) {
        for(int i=1; i <= Random.Range(1,4); i++) {
            GameObject g = Instantiate(HibiPrefs[Random.Range(0, HibiPrefs.Length)], position, Quaternion.Euler(0, 0, Random.Range(-10, 10)*i));
            g.transform.localScale = Vector3.one * Random.Range(1.5f, 3.0f) / i;
        }
    }
    void CreateGround(int left, int right) {
        for (int i = left; i <= right; i++) {
            Instantiate(GrandPrefs[Random.Range(0, GrandPrefs.Length)], new Vector3(i * groundSpan, -1.5f, 0), Quaternion.identity);
        }
        if (left < groundLeft) groundLeft = left;
        if (groundRight < right) groundRight = right;
    }
    public void BattleEnd() {
        if(players[0].controller.stateInfo.fullPathHash == AnimState.GameEnd && players[1].controller.stateInfo.fullPathHash != AnimState.GameEnd) {
            Debug.Log("2PWIN");
            battleResult = BattleResult.Win2P;
            canvases[0].text.text = "LOSE";
            canvases[1].text.text = "WIN";
        }
        else if(players[1].controller.stateInfo.fullPathHash == AnimState.GameEnd && players[0].controller.stateInfo.fullPathHash != AnimState.GameEnd) {
            Debug.Log("1PWIN");
            battleResult = BattleResult.Win1P;
            canvases[0].text.text = "WIN";
            canvases[1].text.text = "LOSE";
        }
        else {
            Debug.Log("DRAW");
            battleResult = BattleResult.Draw;
            canvases[0].text.text = "DRAW";
            canvases[1].text.text = "DRAW";
        }
        for(int i = 0; i < 2; i++) {
            Color c = canvases[i].text.color;
            canvases[i].text.color = new Color(c.r, c.g, c.b, 1.0f);
            Destroy(canvases[i].leftFlame.gameObject);
            Destroy(canvases[i].rightFlame.gameObject);
            players[i].cameraEffect.ChangeTone(0, i == 0 ?CameraEffect.ToneName.redBlack:CameraEffect.ToneName.blueBlack);
            for (int j = 0; j < 2; j++) {
                canvases[j].hpBars[i].transform.parent.gameObject.SetActive(false);
                //canvases[j].mpBars[i].gameObject.SetActive(false);
            }
        }
        ChangeTimeScale(0, 1000);
        VibrateDouble(0.5f, 1.0f);
        Main.gameState = Main.GameState.Result;
    }
}
