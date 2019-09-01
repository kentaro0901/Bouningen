using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMgr : MonoBehaviour {

    float timeScaleSeconds = 0.0f;

    void Update() {

        TimeScaleCountDown();

        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            SceneManager.LoadScene("Result");
            Main.state = Main.State.Result;
        }
    }


    public void ChangeTimeScale(float speed, float seconds) {
        Time.timeScale = speed;
        timeScaleSeconds = seconds;
    }
    private void TimeScaleCountDown() {
        if (timeScaleSeconds > 0) {
            timeScaleSeconds -= Time.unscaledDeltaTime;
        }
        else {
            timeScaleSeconds = 0.0f;
            Time.timeScale = 1.0f;
        }
    }
}
