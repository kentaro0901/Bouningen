using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultMgr : MonoBehaviour {

    [SerializeField] Text winText;

    void Start() {
        Main.state = Main.State.Result;
        Main.CameraSetting();
        switch (Main.battleResult) {
            case Main.BattleResult.Win1P:
                winText.text = "1PWIN"; break;
            case Main.BattleResult.Win2P:
                winText.text = "2PWIN"; break;
            case Main.BattleResult.Default:
                winText.text = "DRAW"; break;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            Main.Init(true);
        }
    }
}
