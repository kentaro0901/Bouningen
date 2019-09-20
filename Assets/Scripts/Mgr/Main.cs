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
        fIghter
    }
    public Chara chara1P = Chara.Sword;
    public Chara chara2P = Chara.Sword;

    public enum BattleResult {
        Default,
        Win1P,
        Win2P
    }
    public static BattleResult battleResult = BattleResult.Default;

    static AudioSource bgm;
    public AudioClip mainMusic;

    Camera camera1;
    Camera camera2;
    public bool isMultiDisplays = true;
    public bool isDynamicCamera = false;
    public float cameraSize = 5.0f;
    public bool isVisibleBox = false;

    void Awake() {
        if (this != Instance) { //２つ目以降のインスタンスは破棄
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        bgm = this.GetComponent<AudioSource>();
        bgm.clip = mainMusic;
    }

    //タイトルへ
    public static void Init(bool isFade) {
        bgm.Stop();
        bgm.Play();
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
    public static void CameraSetting() {
        Main.Instance.camera1 = GameObject.FindGameObjectWithTag("Camera1").GetComponent<Camera>();
        Main.Instance.camera2 = GameObject.FindGameObjectWithTag("Camera2").GetComponent<Camera>();
        if (Main.Instance.isMultiDisplays) { //マルチディスプレイ
            Main.Instance.camera1.rect = new Rect(0, 0, 1, 1);
            Main.Instance.camera1.targetDisplay = 0;
            Main.Instance.camera1.orthographicSize = Main.instance.cameraSize;
            Main.Instance.camera2.rect = new Rect(0, 0, 1, 1);
            Main.Instance.camera2.targetDisplay = 1;
            Main.Instance.camera2.orthographicSize = Main.instance.cameraSize;
        }
        else { //シングルディスプレイ
            Main.Instance.camera1.rect = new Rect(0, 0, 0.5f, 1);
            Main.Instance.camera1.targetDisplay = 0;
            Main.Instance.camera1.orthographicSize = Main.instance.cameraSize;
            Main.Instance.camera2.rect = new Rect(0.5f, 0, 0.5f, 1);
            Main.Instance.camera2.targetDisplay = 0;
            Main.Instance.camera2.orthographicSize = Main.instance.cameraSize;
        }
    }

    void Update() {

        //タイトルに戻る
        if (Input.GetKeyDown(KeyCode.F5) || 
            (Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.N) && Input.GetKeyDown(KeyCode.D))) {
            Init(false);
        }

        //強制終了
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
