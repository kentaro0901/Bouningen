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

    static AudioSource bgm; //BGM
    public AudioClip inferno; //とりあえず

    public bool isMultiDisplays = true;
    Camera camera1;
    Camera camera2;
    public bool isDynamicCamera = false;

    void Awake() {
        if (this != Instance) { //２つ目以降のインスタンスは破棄
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        bgm = this.GetComponent<AudioSource>();
        bgm.clip = inferno;
    }

    //タイトルへ
    public static void Init() {
        bgm.Stop();
        bgm.Play();
        state = State.Title;
        SceneManager.LoadScene("Title");
    }

    //カメラ
    public static void CameraSetting() {
        Main.Instance.camera1 = GameObject.FindGameObjectWithTag("Camera1").GetComponent<Camera>();
        Main.Instance.camera2 = GameObject.FindGameObjectWithTag("Camera2").GetComponent<Camera>();
        if (Main.Instance.isMultiDisplays) { //マルチディスプレイ
            Main.Instance.camera1.rect = new Rect(0, 0, 1, 1);
            Main.Instance.camera1.targetDisplay = 0;
            Main.Instance.camera1.orthographicSize = 5;
            Main.Instance.camera2.rect = new Rect(0, 0, 1, 1);
            Main.Instance.camera2.targetDisplay = 1;
            Main.Instance.camera2.orthographicSize = 5;
        }
        else { //シングルディスプレイ
            Main.Instance.camera1.rect = new Rect(0, 0, 0.5f, 1);
            Main.Instance.camera1.targetDisplay = 0;
            Main.Instance.camera1.orthographicSize = 7;
            Main.Instance.camera2.rect = new Rect(0.5f, 0, 0.5f, 1);
            Main.Instance.camera2.targetDisplay = 0;
            Main.Instance.camera2.orthographicSize = 7;
        }
    }

    void Update() {

        //F5でタイトルに戻る
        if (Input.GetKeyDown(KeyCode.F5)) {
            Init();
        }

        //escで強制終了
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
