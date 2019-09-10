using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour {

    public Transform playerTf;
    public static bool isNear = false;
    Transform cameraTf;
    [SerializeField] Transform enemyCameraTf;
    Vector3 cameraInitPos;
    Camera _camera;
    public static float chaseRange = 1.0f;
    float alpha = -0.105f; //補正
    float beta = -0.205f;

    void Start(){
        cameraTf = this.gameObject.transform;
        cameraInitPos = cameraTf.position;
        _camera = GetComponentInChildren<Camera>();
        chaseRange = _camera.orthographicSize * (Screen.width / Screen.height) / (Main.Instance.isMultiDisplays? 2: 4) + (Main.Instance.isMultiDisplays? beta:alpha);
    }

    public void NearCamera() {
        isNear = true;
    }
    public void FarCamera(bool isLeft) {
        if (isLeft) { //左
            if (Main.Instance.isMultiDisplays) {
                _camera.rect = new Rect(0, 0, 1, 1);
                _camera.targetDisplay = 0;
            }
            else {
                _camera.rect = new Rect(0, 0, 0.5f, 1);
                _camera.targetDisplay = 0;
            }
        }
        else { //右
            if (Main.Instance.isMultiDisplays) {
                _camera.rect = new Rect(0, 0, 1, 1);
                _camera.targetDisplay = 1;
            }
            else {
                _camera.rect = new Rect(0.5f, 0, 0.5f, 1);
                _camera.targetDisplay = 0;
            }
        }
        isNear = false;
    }

    void Update(){
        if (Main.Instance.isDynamicCamera) {
            if (!isNear) { //遠
                if (playerTf.position.x < cameraTf.position.x - chaseRange) { //左
                    cameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y, cameraInitPos.z);
                }
                if (cameraTf.position.x + chaseRange < playerTf.position.x) { //右
                    cameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y, cameraInitPos.z);
                }
            }
            else { //近
                if (cameraTf.position.x < enemyCameraTf.position.x) { //カメラ左
                    if (playerTf.position.x < cameraTf.position.x - chaseRange) { //左
                        cameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y, cameraInitPos.z);
                        enemyCameraTf.position = new Vector3(cameraTf.position.x + chaseRange * 8, cameraInitPos.y, cameraInitPos.z);
                    }
                    if (enemyCameraTf.position.x + chaseRange < playerTf.position.x) { //右
                        enemyCameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y, cameraInitPos.z);
                        cameraTf.position = new Vector3(enemyCameraTf.position.x - chaseRange * 8, cameraInitPos.y, cameraInitPos.z);
                    }
                }
                if (cameraTf.position.x >= enemyCameraTf.position.x) { //カメラ右
                    if (enemyCameraTf.position.x - chaseRange > playerTf.position.x) { //左
                        enemyCameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y, cameraInitPos.z);
                        cameraTf.position = new Vector3(enemyCameraTf.position.x + chaseRange * 8, cameraInitPos.y, cameraInitPos.z);
                    }
                    if (cameraTf.position.x + chaseRange < playerTf.position.x) { //右
                        cameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y, cameraInitPos.z);
                        enemyCameraTf.position = new Vector3(cameraTf.position.x - chaseRange * 8, cameraInitPos.y, cameraInitPos.z);
                    }
                }
            }
        }
        else {
            if (playerTf.position.x < cameraTf.position.x - chaseRange) { //左
                cameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y, cameraInitPos.z);
            }
            if (cameraTf.position.x + chaseRange < playerTf.position.x) { //右
                cameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y, cameraInitPos.z);
            }
        }
    }
}
