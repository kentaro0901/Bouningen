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

    [SerializeField] Animator animator;
    private int counter = 0;

    void Start() {
        Main.state = Main.State.Title;
        Main.Instance.UICameraSetting();
        counter = 0;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(0 < animator.speed) {
                animator.speed = 0;
            }
            else {
                animator.speed = 0.8f;
            }        
        }
        if (Input.GetKeyDown(KeyCode.D) || counter == 1200) { //デモ
            Main.Instance.isDemo = true;
            Main.Instance.playerType[0] = Main.PlayerType.AI;
            Main.Instance.playerType[1] = Main.PlayerType.AI;
            Main.Instance.chara1P = (Main.Chara)Random.Range(0, 3);
            Main.Instance.chara2P = (Main.Chara)Random.Range(0, 3);
            FadeManager.Instance.LoadScene("Battle", 0.5f);
        }
        if ((Input.GetButtonDown("ButtonA_0") ||
            ((Main.controller[0] == Main.Controller.Joycon) ? Main.joycon[0].GetButtonDown(Joycon.Button.DPAD_DOWN) : false) ||
            ((Main.controller[1] == Main.Controller.Joycon) ? Main.joycon[1].GetButtonDown(Joycon.Button.DPAD_UP) : false)) && 
            Main.state == Main.State.Title) {
            Main.Instance.isDemo = false;
            Main.state = Main.State.Select;
            FadeManager.Instance.LoadScene("Select", 0.5f);
        }
        counter++;
    }
}
