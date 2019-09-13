using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMgr : MonoBehaviour {

    void Start() {
        Main.state = Main.State.Result;
        Main.CameraSetting();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            Main.Init(true);
        }
    }
}
