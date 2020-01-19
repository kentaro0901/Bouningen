using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour {

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
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }
    }

    [SerializeField] Animator animator;

    void Start() {
        Main.gameState = Main.GameState.Title;
        Main.Instance.TitleCameraSetting();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.D)) { //デモ
            Main.Instance.isDemo = true;
            for (int i = 0; i<2; i++) {
                Main.Instance.playerType[i] = Main.PlayerType.AI;
                Main.Instance.chara[i] = (Main.Chara)Random.Range(0, 3);
            }
            FadeManager.Instance.LoadScene("Battle", 0.5f);
        }
        if ((Input.GetButtonDown("ButtonA_0") || //セレクト画面へ
            ((Main.controller[0] == Main.Controller.Joycon) ? Main.joycon[0].GetButtonDown(Joycon.Button.DPAD_DOWN) : false) ||
            ((Main.controller[1] == Main.Controller.Joycon) ? Main.joycon[1].GetButtonDown(Joycon.Button.DPAD_UP) : false)) && 
            Main.gameState == Main.GameState.Title) {
            Main.Instance.isDemo = false;
            Main.gameState = Main.GameState.Select;
            FadeManager.Instance.LoadScene("Select", 0.5f);
        }
    }
}
