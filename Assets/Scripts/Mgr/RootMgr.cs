using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootMgr : MonoBehaviour {

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
        Main.gameState = Main.GameState.Root;
        Main.Instance.TitleCameraSetting();
        StartMultiDisplays();
        Main.Instance.Init(false);
    }

    void StartMultiDisplays() {
        if (Display.displays.Length > 1) {
            Display.displays[1].Activate();
            if(Main.Instance.isAutoMultiDisplays) Main.Instance.isMultiDisplays = true;
        }
        else {
           if(Main.Instance.isAutoMultiDisplays) Main.Instance.isMultiDisplays = false;
        }
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }
}
