using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    //シングルトン
    private static Main instance;
    public static Main Instance {
        get {
            if (instance == null) {
                instance = (Main)FindObjectOfType(typeof(Main));
                if (instance == null)
                    Debug.LogError(typeof(Main) + "is nothing");
            }
            return instance;
        }
    }
    void Awake() {
        if (this != Instance) { //２つ目以降のインスタンスは破棄
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public enum State{
        Root,
        Title,
        Select,
        Battle,
        Result
    }
    public static State state = State.Root;
    public enum Chara {
        Sword,
        Fighter
    }
    public Chara chara1P = Chara.Sword;
    public Chara chara2P = Chara.Sword;
    public enum Controller {
        None,
        Elecom,
        Joycon
    }
    public Controller controller1 = Controller.None;
    public Controller controller2 = Controller.None;

    public enum BattleResult {
        Default,
        Battle,
        Finish,
        Draw,
        Win1P,
        Win2P
    }
    public static BattleResult battleResult = BattleResult.Default;

    public AudioSource mainBgm;
    public AudioSource subBgm;
    [SerializeField] AudioClip mainMusic;
    [SerializeField] AudioClip subMusic;

    Camera camera1;
    Camera camera2;
    public bool isMultiDisplays = true;
    public bool isDynamicCamera = false;
    public bool isVisibleBox = false;
    public bool isVisibleUI = true;
    public float cameraSize = 6.0f;
    public float gameSpeed = 1.0f;

    void Start() {
        mainBgm.clip = mainMusic;
        subBgm.clip = subMusic;
    }

    //タイトルへ
    public void Init(bool isFade) {
        Instance.mainBgm.Stop();
        Instance.mainBgm.Play();
        Instance.subBgm.Stop();
        Instance.subBgm.Play();
        state = State.Title;
        battleResult = BattleResult.Default;
        if (isFade) {
            FadeManager.Instance.LoadScene("Title", 0.5f);
        }
        else {
            SceneManager.LoadScene("Title");
        }
    }

    //カメラ
    public void UICameraSetting() {
        camera1 = GameObject.FindGameObjectWithTag("Camera1").GetComponent<Camera>();
        camera2 = GameObject.FindGameObjectWithTag("Camera2").GetComponent<Camera>();
        if (isMultiDisplays) { //マルチディスプレイ
            camera1.transform.position = new Vector3(0, 0, -10);
            camera1.rect = new Rect(0, 0, 1, 1);
            camera1.targetDisplay = 0;
            camera1.orthographicSize = 5.0f;
            camera2.transform.position = new Vector3(17.8f, 0, -10);//
            camera2.rect = new Rect(0, 0, 1, 1);
            camera2.targetDisplay = 1;
            camera2.orthographicSize = 5.0f;
        }
        else { //シングルディスプレイ
            camera1.transform.position = new Vector3(8, 0, -10);
            camera1.rect = new Rect(0, 0, 1, 1);
            camera1.targetDisplay = 0;
            camera1.orthographicSize = 8.0f;
            camera2.enabled = false;
        }
    }
    public void BattleCameraSetting() {
        camera1 = GameObject.FindGameObjectWithTag("Camera1").GetComponent<Camera>();
        camera2 = GameObject.FindGameObjectWithTag("Camera2").GetComponent<Camera>();
        if (Main.Instance.isMultiDisplays) { //マルチディスプレイ
            camera1.rect = new Rect(0, 0, 1, 1);
            camera1.targetDisplay = 0;
            camera1.orthographicSize = Main.instance.cameraSize;
            camera2.rect = new Rect(0, 0, 1, 1);
            camera2.targetDisplay = 1;
            camera2.orthographicSize = Main.instance.cameraSize;
        }
        else { //シングルディスプレイ
            camera1.rect = new Rect(0, 0, 0.5f, 1);
            camera1.targetDisplay = 0;
            camera1.orthographicSize = Main.instance.cameraSize;
            camera2.rect = new Rect(0.5f, 0, 0.5f, 1);
            camera2.targetDisplay = 0;
            camera2.orthographicSize = Main.instance.cameraSize;
        }
    }

    //ゲームパッド
    public void CheckGamePad() {
        //Debug.Log("Num:" + Input.GetJoystickNames().Length);
        if (Input.GetJoystickNames()[0] == "PC Game Controller       ") {
            controller1 = Controller.Elecom;
            Debug.Log("1P:Elecom");
        }
        else if (Input.GetJoystickNames()[0] == "Wireless Gamepad") {
            controller1 = Controller.Joycon;
            Debug.Log("1P:Joycon");
        }
        else {
            controller1 = Controller.None;
            Debug.Log("1P:None");
            Debug.Log("1P:" + Input.GetJoystickNames()[0]);
        }
        if (Input.GetJoystickNames()[1] == "PC Game Controller       ") {
            controller2 = Controller.Elecom;
            Debug.Log("2P:Elecom");
        }
        else if (Input.GetJoystickNames()[1] == "Wireless Gamepad") {
            controller2 = Controller.Joycon;
            Debug.Log("2P:Joycon");
        }
        else {
            controller2 = Controller.None;
            Debug.Log("2P:None");
            Debug.Log("2P:" + Input.GetJoystickNames()[1]);
        }
    }

    void Update() {
        
        //タイトルに戻る
        if (Input.GetKeyDown(KeyCode.F5) || 
            (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.D))) {
            Init(false);
        }
        //強制終了
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
