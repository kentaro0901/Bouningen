using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMgr : MonoBehaviour {

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            SceneManager.LoadScene("Result");
            Main.state = Main.State.Result;
        }
    }
}
