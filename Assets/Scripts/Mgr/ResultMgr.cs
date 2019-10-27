using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultMgr : MonoBehaviour {

    //シングルトン
    private static ResultMgr instance;
    public static ResultMgr Instance {
        get {
            if (instance == null) {
                instance = (ResultMgr)FindObjectOfType(typeof(ResultMgr));
                if (instance == null)
                    Debug.LogError(typeof(ResultMgr) + "is nothing");
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

    [SerializeField] Text winText;

    void Start() {
        Main.state = Main.State.Result;
        Main.Instance.BattleCameraSetting();
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
            Main.Instance.Init(true);
        }
    }
}
