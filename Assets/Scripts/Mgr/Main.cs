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
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public enum GameState{
        Root,
        Title,
        Select,
        Battle,
        Result,
        Loading
    }
    public static GameState gameState = GameState.Root;
    public enum Chara {
        Sword,
        Fighter,
        Hammer
    }
    public Chara[] chara = { Chara.Sword, Chara.Sword };
    public enum Controller {
        None,
        GamePad,
        Joycon
    }
    public static Controller[] controller = new Controller[2];
    public static List<Joycon> joycons;
    public static Joycon[] joycon = new Joycon[2];
    public enum PlayerType {
        None,
        Player,
        AI
    }
    public PlayerType[] playerType = new PlayerType[2];

    public AudioSource mainBgm;
    public AudioSource subBgm;
    [SerializeField] AudioClip mainMusic;
    [SerializeField] AudioClip subMusic;

    Camera[] _camera = new Camera[2];
    public bool isAutoMultiDisplays = true;
    public bool isMultiDisplays = true;
    public bool isDynamicCamera = false;
    public bool isVisibleBox = false;
    public bool isVisibleUI = true;
    public float cameraSize = 6.0f;
    public float gameSpeed = 1.0f;
    public bool isDemo = false;

    void Start() {
        mainBgm.clip = mainMusic;
        subBgm.clip = subMusic;
        CheckGamePad();
    }

    public void Init(bool isFade) {
        if (!isFade) {
            Instance.mainBgm.Stop();
            Instance.mainBgm.Play();
            Instance.subBgm.Stop();
            Instance.subBgm.Play();
        }
        gameState = GameState.Title;
        Time.timeScale = 1.0f;
        Instance.isDemo = false;
        Instance.playerType[0] = PlayerType.Player;
        Instance.playerType[1] = PlayerType.Player;
        if (isFade) {
            FadeManager.Instance.LoadScene("Title", 0.5f);
        }
        else {
            SceneManager.LoadScene("Title");
        }
    }

    public void UICameraSetting() {
        _camera[0] = GameObject.FindGameObjectWithTag("Camera1").GetComponent<Camera>();
        _camera[1] = GameObject.FindGameObjectWithTag("Camera2").GetComponent<Camera>();
        if (isMultiDisplays) {
            for(int i =0; i<2; i++) {
                _camera[i].transform.position = new Vector3(i * 17.8f, 0, -10);
                _camera[i].rect = new Rect(0, 0, 1, 1);
                _camera[i].targetDisplay = i;
                _camera[i].orthographicSize = 5.0f;
            }
        }
        else {
            _camera[0].transform.position = new Vector3(8, 0, -10);
            _camera[0].rect = new Rect(0, 0, 1, 1);
            _camera[0].targetDisplay = 0;
            _camera[0].orthographicSize = 8.0f;
            _camera[1].enabled = false;
        }
    }
    public void BattleCameraSetting() {
        _camera[0] = GameObject.FindGameObjectWithTag("Camera1").GetComponent<Camera>();
        _camera[1] = GameObject.FindGameObjectWithTag("Camera2").GetComponent<Camera>();
        if (Main.Instance.isMultiDisplays) { 
            for(int i=0; i<2; i++) {
                _camera[i].rect = new Rect(0, 0, 1, 1);
                _camera[i].targetDisplay = i;
                _camera[i].orthographicSize = Main.instance.cameraSize;
            }
        }
        else {
            for(int i=0; i<2; i++) {
                _camera[i].rect = new Rect(i * 0.5f, 0, 0.5f, 1);
                _camera[i].targetDisplay = 0;
                _camera[i].orthographicSize = Main.instance.cameraSize;
            }
        }
    }

    public void CheckGamePad() {
        joycons = JoyconManager.Instance.j;
        for (int i = 0; i < 2; i++) {
            joycon[i] = joycons.Find(c => i == 0 ? c.isLeft : !c.isLeft);
            controller[i] = Controller.None;
            playerType[i] = PlayerType.Player;
            if (joycons.Any(c => i == 0 ? c.isLeft : !c.isLeft)) {
                controller[i] = Controller.Joycon;
                Debug.Log(i + 1 + "P:Joycon");
            }
            else if (i < Input.GetJoystickNames().Length) {
                controller[i] = Controller.GamePad;
                Debug.Log(i + 1 + "P:GamePad");
            }
            else {
                Debug.Log(i + 1 + "P:None");
            }
        }
    }

    void Update() {
        //タイトルに戻る
        if (Input.GetKeyDown(KeyCode.F5)) {
            Init(false);
        }
        //強制終了
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
