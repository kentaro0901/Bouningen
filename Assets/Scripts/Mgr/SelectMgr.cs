using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : MonoBehaviour {

    void Start() {
        Main.state = Main.State.Select;
        Main.CameraSetting();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {//仮
            //SceneManager.LoadScene("Battle");
            FadeManager.Instance.LoadScene("Battle", 1.0f);
            Main.state = Main.State.Battle;
        }
    }
}
