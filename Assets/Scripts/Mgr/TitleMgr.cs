using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour {

    //シングルトン
    private static TitleMgr instance;
    public static TitleMgr Instance {
        get {
            if (instance == null) {
                instance = (TitleMgr)FindObjectOfType(typeof(TitleMgr));
                if (instance == null)
                    Debug.LogError(typeof(TitleMgr) + "is nothing");
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

    void Start() {
        Main.state = Main.State.Title;
        Main.CameraSetting();
    }

    void Update() {
        if (Input.GetButtonDown("ButtonA_0")) {
            FadeManager.Instance.LoadScene("Select", 0.5f);
            Main.state = Main.State.Select;
        }
    }
}
