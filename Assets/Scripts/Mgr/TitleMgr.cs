using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour {
 
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            SceneManager.LoadScene("Select");
            Main.state = Main.State.Select;
        }
    }
}
