using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour{

    public Transform chaseTf;
    Vector3[] chaseTfPrePos;
    Transform chaseCameraTf;
    Vector3 chaseCameraInitPos;

    void Start(){
        chaseTfPrePos = new Vector3[3];
        chaseCameraTf = this.gameObject.transform;
        chaseCameraInitPos = chaseCameraTf.position;
    }

    void Update(){
        if(chaseTf != null) {
            if(chaseTf.position.y < 4) {
                chaseCameraTf.position = chaseCameraInitPos + new Vector3(chaseTfPrePos[2].x, 0, 0);
            }
            else {
                chaseCameraTf.position = chaseCameraInitPos + new Vector3(chaseTfPrePos[2].x, chaseTf.position.y - 4, 0);
            }

            chaseTfPrePos[2] = chaseTfPrePos[1];
            chaseTfPrePos[1] = chaseTfPrePos[0];
            chaseTfPrePos[0] = chaseTf.position;
        }
    }
}
