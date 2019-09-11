using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMgr : MonoBehaviour {

    [SerializeField] ChaseCamera chaseCamera1;
    [SerializeField] ChaseCamera chaseCamera2;
    Transform camera1Tf;
    Transform camera2Tf;
    [SerializeField] CameraEffect cameraEffect1;
    [SerializeField] CameraEffect cameraEffect2;
    [SerializeField] PlayerController playerController1;
    [SerializeField] PlayerController playerController2;
    Transform player1Tf;
    Transform player2Tf;
    [SerializeField] RectTransform canvas1RTf;
    [SerializeField] RectTransform canvas2Rtf;

    float timeScaleSeconds = 0.0f;

    void Start() {
        Main.state = Main.State.Battle;
        Main.CameraSetting();
        camera1Tf = chaseCamera1.transform;
        camera2Tf = chaseCamera2.transform;
        player1Tf = playerController1.playerTf;
        player2Tf = playerController2.playerTf;
    }

    void Update() {

        TimeScaleCountDown();
        if (Main.Instance.isDynamicCamera) {
            if (!ChaseCamera.isNear && Mathf.Abs(camera1Tf.position.x - camera2Tf.position.x) < ChaseCamera.chaseRange * 8) { //近距離になったとき
                chaseCamera1.NearCamera();
                chaseCamera2.NearCamera();
            }
            if (ChaseCamera.isNear && Mathf.Abs(player1Tf.position.x - player2Tf.position.x) >= ChaseCamera.chaseRange * 10) { //遠距離になった時
                chaseCamera1.FarCamera(player1Tf.position.x < player2Tf.position.x);
                chaseCamera2.FarCamera(player1Tf.position.x >= player2Tf.position.x);
            }
        }
    
        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            FadeManager.Instance.LoadScene("Result", 0.5f);
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
