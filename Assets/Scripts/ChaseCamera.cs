using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour {

    public Transform playerTf;
    public static bool isNear = false;
    Transform cameraTf;
    Transform enemyCameraTf;
    Vector3 cameraInitPos;
    Camera _camera;
    public static float chaseRange = 1.0f;

    void Start(){
        cameraTf = this.gameObject.transform;
        cameraInitPos = cameraTf.position;
        _camera = GetComponentInChildren<Camera>();
        chaseRange = _camera.orthographicSize * (Screen.width / Screen.height) /2;
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
        if (!isNear) {
            if (playerTf.position.x < cameraTf.position.x - chaseRange) { //左
                cameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y, cameraInitPos.z);
            }
            if (cameraTf.position.x + chaseRange < playerTf.position.x) { //右
                cameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y, cameraInitPos.z);
            }
        }
        else {

        }
    }
}
