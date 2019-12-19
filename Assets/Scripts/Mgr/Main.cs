using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Fighter,
        Hammer
    }
    public Chara chara1P = Chara.Sword;
    public Chara chara2P = Chara.Sword;
    public enum Controller {
        None,
        GamePad,
        Joycon
    }
    public static Controller[] controller = new Controller[2];
    public static List<Joycon> joycons;
    public static Joycon[] joycon = new Joycon[2];
    public bool isSwapController = false;
    public enum PlayerType {
        None,
        Player,
        AI,
        Online
    }
    public PlayerType[] playerType = new PlayerType[2];
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
    public bool isAutoMultiDisplays = true;
    public bool isMultiDisplays = true;
    public bool isDynamicCamera = false;
    public bool isVisibleBox = false;
    public bool isVisibleUI = true;
    public float cameraSize = 6.0f;
    public float gameSpeed = 1.0f;

    void Start() {
        mainBgm.clip = mainMusic;
        subBgm.clip = subMusic;
        CheckGamePad();
    }

    //タイトルへ
    public void Init(bool isFade) {
        if (!isFade) {
            Instance.mainBgm.Stop();
            Instance.mainBgm.Play();
            Instance.subBgm.Stop();
            Instance.subBgm.Play();
        }
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
        joycons = JoyconManager.Instance.j;
        joycon[0] = joycons.Find(c => c.isLeft); // Joy-Con (L)
        joycon[1] = joycons.Find(c => !c.isLeft); // Joy-Con (R)
        controller[0] = Controller.None;
        controller[1] = Controller.None;
        playerType[0] = PlayerType.Player;
        playerType[1] = PlayerType.Player;

        if (joycons.Any(c => c.isLeft)) {
            controller[0] = Controller.Joycon;
            Debug.Log("1P:JoyconL");
        }
        else if (0 < Input.GetJoystickNames().Length) {
            if (Input.GetJoystickNames()[0] == "PC Game Controller       ") {
                controller[0] = Controller.GamePad;
                Debug.Log("1P:GamePad");
            }
            else Debug.Log("1P:None");
        }
        else {
            Debug.Log("1P:None");
        }

        if (joycons.Any(c => !c.isLeft)) {
            controller[1] = Controller.Joycon;
            Debug.Log("2P:JoyconR");
        }
        else if(1 < Input.GetJoystickNames().Length) {
            if (Input.GetJoystickNames()[1] == "PC Game Controller       ") {
                controller[1] = Controller.GamePad;
                Debug.Log("2P:GamePad");
            }
            else Debug.Log("2P:None");
        }
        else {
            //playerType[1] = PlayerType.AI;
            Debug.Log("2P:None");
        }
        //スワップ
        if (controller[0] == Controller.None && controller[1] != Controller.None) {
            isSwapController = true;
            controller[0] = controller[1];
            controller[1] = Controller.None;
            //playerType[1] = PlayerType.AI;
            Debug.Log("Controller swapped");
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
