using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour {

    public Transform chaseTf;
    Transform chaseCameraTf;
    Vector3 chaseCameraInitPos;
    Camera _camera;
    [SerializeField] float chaseRange = 1.0f;

    void Start(){
        chaseCameraTf = this.gameObject.transform;
        chaseCameraInitPos = chaseCameraTf.position;
        _camera = GetComponentInChildren<Camera>();
        chaseRange = -_camera.ScreenToWorldPoint(new Vector3(Screen.width / 4, Screen.height / 2, -10)).x;
    }

    void Update(){
        if(chaseCameraTf.position.x + chaseRange < chaseTf.position.x) { //右
            chaseCameraTf.position = new Vector3(chaseTf.position.x - chaseRange, chaseCameraInitPos.y, chaseCameraInitPos.z);
        }
        if (chaseTf.position.x < chaseCameraTf.position.x -chaseRange) { //左
            chaseCameraTf.position = new Vector3(chaseTf.position.x + chaseRange, chaseCameraInitPos.y, chaseCameraInitPos.z);
        }

    }
}
