using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootMgr : MonoBehaviour {

    //シングルトン
    private static RootMgr instance;
    public static RootMgr Instance {
        get {
            if (instance == null) {
                instance = (RootMgr)FindObjectOfType(typeof(RootMgr));
                if (instance == null)
                    Debug.LogError(typeof(RootMgr) + "is nothing");
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

    void Start() {
        Main.state = Main.State.Root;
        Main.Instance.UICameraSetting();
        if (Main.Instance.isMultiDisplays) {
            StartMultiDisplays();
        }
        Main.Instance.Init(false);
    }

    //マルチディスプレイ
    void StartMultiDisplays() {
        if (Display.displays.Length > 1) {
            Display.displays[1].Activate();
        }
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }
}
