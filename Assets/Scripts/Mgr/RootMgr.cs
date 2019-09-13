using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootMgr : MonoBehaviour {

    void Start() {
        Main.state = Main.State.Root;
        Main.CameraSetting();
        if (Main.Instance.isMultiDisplays) {
            StartMultiDisplays();
        }
        Main.Init(false);
    }

    //マルチディスプレイ
    void StartMultiDisplays() {
        if (Display.displays.Length > 1) {
            Display.displays[1].Activate();
        }
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }
}
