using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour {

    public Transform playerTf;
    public static bool isNear = false;
    Transform cameraTf;
    [SerializeField] Transform enemyCameraTf;
    public bool isLeft = false;
    Vector3 cameraInitPos;
    Camera _camera;
    public static float chaseRange = 1.0f;
    public float verticalRange = 1.0f;
    [SerializeField] GameObject BackLinePref;
    GameObject BackLineLeft;
    GameObject BackLineRight;

    void Start(){
        cameraTf = this.gameObject.transform;
        cameraInitPos = cameraTf.position;
        cameraTf.position = new Vector3(playerTf.position.x, cameraInitPos.y, cameraInitPos.z);
        _camera = GetComponentInChildren<Camera>();
        chaseRange = _camera.orthographicSize * ((float)Screen.width/ Screen.height) / (Main.Instance.isMultiDisplays? 2: 4);
        BackLineLeft = Instantiate(BackLinePref, new Vector3(cameraInitPos.x - chaseRange * 4, cameraInitPos.y, 0), Quaternion.identity);
        BackLineLeft.transform.parent = this.gameObject.transform;
        BackLineRight = Instantiate(BackLinePref, new Vector3(cameraInitPos.x + chaseRange * 4, cameraInitPos.y, 0), Quaternion.identity);
        BackLineRight.transform.parent = this.gameObject.transform;
    }

    public void NearCamera() {
        BackLineLeft.SetActive(false);
        BackLineRight.SetActive(false);
        isNear = true;
    }
    public void FarCamera(bool isLeft) {
        if (isLeft) { //左
            BackLineLeft.SetActive(true);
            BackLineRight.SetActive(false);
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
            BackLineLeft.SetActive(false);
            BackLineRight.SetActive(true);
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

    void LateUpdate(){ //動くけど無駄がある
        isLeft = cameraTf.position.x < enemyCameraTf.position.x;
        if (Main.Instance.isDynamicCamera) {
            if (!isNear) { //遠
                if (playerTf.position.x < cameraTf.position.x - chaseRange) { //左
                    cameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y + ((verticalRange < playerTf.position.y)? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
                }
                else if (cameraTf.position.x + chaseRange < playerTf.position.x) { //右
                    cameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y + ((verticalRange < playerTf.position.y) ? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
                }
                else { //真ん中
                    cameraTf.position = new Vector3(cameraTf.position.x, cameraInitPos.y + ((verticalRange < playerTf.position.y) ? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
                }
            }
            else { //近
                if (cameraTf.position.x < enemyCameraTf.position.x) { //カメラ左
                    if (playerTf.position.x < cameraTf.position.x - chaseRange) { //左
                        cameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y + ((verticalRange < playerTf.position.y) ? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
                        enemyCameraTf.position = new Vector3(cameraTf.position.x + chaseRange * 4, enemyCameraTf.position.y, cameraInitPos.z);
                    }
                    else if (enemyCameraTf.position.x + chaseRange < playerTf.position.x) { //右
                        enemyCameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y, cameraInitPos.z);
                        cameraTf.position = new Vector3(enemyCameraTf.position.x - chaseRange * 4, enemyCameraTf.position.y, cameraInitPos.z);
                    }
                    else { //真ん中
                        cameraTf.position = new Vector3(cameraTf.position.x, cameraInitPos.y, cameraInitPos.z);
                        enemyCameraTf.position = new Vector3(cameraTf.position.x + chaseRange * 4, enemyCameraTf.position.y, cameraInitPos.z);
                    }
                }
                if (cameraTf.position.x >= enemyCameraTf.position.x) { //カメラ右
                    if (enemyCameraTf.position.x - chaseRange > playerTf.position.x) { //左
                        enemyCameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y, cameraInitPos.z);
                        cameraTf.position = new Vector3(enemyCameraTf.position.x + chaseRange * 4, enemyCameraTf.position.y, cameraInitPos.z);
                    }
                    else if (cameraTf.position.x + chaseRange < playerTf.position.x) { //右
                        cameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y + ((verticalRange < playerTf.position.y) ? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
                        enemyCameraTf.position = new Vector3(cameraTf.position.x - chaseRange * 4, enemyCameraTf.position.y, cameraInitPos.z);
                    }
                    else { //真ん中
                        cameraTf.position = new Vector3(cameraTf.position.x, cameraInitPos.y, cameraInitPos.z);
                        enemyCameraTf.position = new Vector3(cameraTf.position.x - chaseRange * 4, enemyCameraTf.position.y, cameraInitPos.z);
                    }
                }
            }
        }
        else {
            if (playerTf.position.x < cameraTf.position.x - chaseRange) { //左
                cameraTf.position = new Vector3(playerTf.position.x + chaseRange, cameraInitPos.y + ((verticalRange < playerTf.position.y) ? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
            }
            else if (cameraTf.position.x + chaseRange < playerTf.position.x) { //右
                cameraTf.position = new Vector3(playerTf.position.x - chaseRange, cameraInitPos.y + ((verticalRange < playerTf.position.y) ? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
            }
            else { //真ん中
                    cameraTf.position = new Vector3(cameraTf.position.x, cameraInitPos.y + ((verticalRange < playerTf.position.y) ? playerTf.position.y - verticalRange : 0), cameraInitPos.z);
                }
        }
    }
}
