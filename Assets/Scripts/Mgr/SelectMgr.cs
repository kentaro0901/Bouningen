using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : MonoBehaviour {

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {//仮
            SceneManager.LoadScene("Battle");
            Main.state = Main.State.Battle;
        }
    }
}
