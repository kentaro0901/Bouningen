using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour {

    void Start() {
        Main.state = Main.State.Title;
        Main.CameraSetting();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            //SceneManager.LoadScene("Select");
            FadeManager.Instance.LoadScene("Select", 0.5f);
            Main.state = Main.State.Select;
        }
    }
}
