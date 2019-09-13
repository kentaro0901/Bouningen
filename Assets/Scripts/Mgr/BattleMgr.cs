using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] RectTransform c1LFRTf;
    [SerializeField] RectTransform c1RFRTf;
    [SerializeField] RectTransform c2LFRTf;
    [SerializeField] RectTransform c2RFRTf;
    [SerializeField] Slider c1hpBar1;
    [SerializeField] Slider c1hpBar2;
    [SerializeField] Slider c2hpBar1;
    [SerializeField] Slider c2hpBar2;

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
        UpdateUI();
        TimeScaleCountDown();
        ChangeCameraChaseMode();  
        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            FadeManager.Instance.LoadScene("Result", 0.5f);
            Main.state = Main.State.Result;
        }
    }
    private void UpdateUI() {
        c1hpBar1.value = c2hpBar1.value = playerController1.hp / playerController1.maxhp;
        c1hpBar2.value = c2hpBar2.value = playerController2.hp / playerController2.maxhp;
    }
    private void ChangeCameraChaseMode() {
        /*
        transLeftD1.offsetMax = Vector2.right * (-Screen.width + Screen.width / 2 * fadeValue);
        transRightD1.offsetMin = Vector2.right * (Screen.width - Screen.width / 2 * fadeValue);
        transLeftD2.offsetMax = Vector2.right * (-Screen.width + Screen.width / 2 * fadeValue);
        transRightD2.offsetMin = Vector2.right * (Screen.width - Screen.width / 2 * fadeValue);
        */
        if (Main.Instance.isDynamicCamera) {
            if (Mathf.Abs(camera1Tf.position.x - camera2Tf.position.x) < ChaseCamera.chaseRange * 8) { //近距離になったとき
                if (!ChaseCamera.isNear) {
                    chaseCamera1.NearCamera();
                    chaseCamera2.NearCamera();
                }
                c1LFRTf.offsetMax = Vector2.zero;
                c1RFRTf.offsetMin = Vector2.zero;
                c2LFRTf.offsetMax = Vector2.zero;
                c2RFRTf.offsetMin = Vector2.zero;
            }
            if (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) >= ChaseCamera.chaseRange * 10) { //遠距離
                if (ChaseCamera.isNear) {
                    chaseCamera1.FarCamera(player1Tf.position.x < player2Tf.position.x);
                    chaseCamera2.FarCamera(player1Tf.position.x >= player2Tf.position.x);
                }
                if(player1Tf.position.x < player2Tf.position.x) {//1P左
                    c1LFRTf.offsetMax = Vector2.zero;
                    c1RFRTf.offsetMin = Vector2.right * -Screen.width / (Main.Instance.isMultiDisplays ? 20 : 40) 
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 10) / (ChaseCamera.chaseRange * 20));
                    c2LFRTf.offsetMax = Vector2.right * Screen.width / (Main.Instance.isMultiDisplays ? 20 : 40)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 10) / (ChaseCamera.chaseRange * 20));
                    c2RFRTf.offsetMin = Vector2.zero;
                }
                else { //1P右
                    c1LFRTf.offsetMax = Vector2.right * Screen.width / (Main.Instance.isMultiDisplays ? 20 : 40)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 10) / (ChaseCamera.chaseRange * 20));
                    c1RFRTf.offsetMin = Vector2.zero;
                    c2LFRTf.offsetMax = Vector2.zero;
                    c2RFRTf.offsetMin = Vector2.right * -Screen.width / (Main.Instance.isMultiDisplays ? 20 : 40)
                        * Mathf.Min(1.0f, (Mathf.Abs(player1Tf.position.x - player2Tf.position.x) - ChaseCamera.chaseRange * 10) / (ChaseCamera.chaseRange * 20));
                }
              
            }
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
