using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : MonoBehaviour {

    //シングルトン
    private static SelectMgr instance;
    public static SelectMgr Instance {
        get {
            if (instance == null) {
                instance = (SelectMgr)FindObjectOfType(typeof(SelectMgr));
                if (instance == null)
                    Debug.LogError(typeof(SelectMgr) + "is nothing");
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
        Main.state = Main.State.Select;
        Main.CameraSetting();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {//仮
            //SceneManager.LoadScene("Battle");
            FadeManager.Instance.LoadScene("Battle", 0.5f);
            Main.state = Main.State.Battle;
        }
    }
}
