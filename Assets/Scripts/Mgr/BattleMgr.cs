using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMgr : MonoBehaviour {

    [SerializeField] CameraEffect cameraEffect1;
    [SerializeField] CameraEffect cameraEffect2;

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

    public void VibrateDouble(float seconds, float range) {
        cameraEffect1.Vibrate(seconds, range);
        cameraEffect2.Vibrate(seconds, range);
    }
    public void ZoomInOutDouble(float seconds) {
        cameraEffect1.ZoomInOut(seconds);
        cameraEffect2.ZoomInOut(seconds);
    }
    public void ChangeToneDouble(float seconds, CameraEffect.ToneName name) {
        cameraEffect1.ChangeTone(seconds, name);
        cameraEffect2.ChangeTone(seconds, name);
    }
}
