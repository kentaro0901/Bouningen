using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGamePad : InputMethod {

    int num;
    private void Start() {
        num = Main.Instance.isSwapController? (int)controller.playerNum+1 : (int)controller.playerNum;
    }

    void Update() {
        if (Mathf.Abs(Input.GetAxis("DPad_XAxis_" + num)) > Mathf.Abs(Input.GetAxis("L_XAxis_" + num)) && Mathf.Abs(Input.GetAxis("DPad_XAxis_" + num)) > Mathf.Abs(Input.GetAxis("R_XAxis_" + num))) {
            xAxis = Input.GetAxis("DPad_XAxis_" + num);
        }
        else if (Mathf.Abs(Input.GetAxis("L_XAxis_" + num)) > Mathf.Abs(Input.GetAxis("R_XAxis_" + num))) {
            xAxis = Input.GetAxis("L_XAxis_" + num);
        }
        else {
            xAxis = Input.GetAxis("R_XAxis_" + num);
        }
        if (Mathf.Abs(Input.GetAxis("DPad_YAxis_" + num)) > Mathf.Abs(Input.GetAxis("L_YAxis_" + num)) && Mathf.Abs(Input.GetAxis("DPad_YAxis_" + num)) > Mathf.Abs(Input.GetAxis("R_YAxis_" + num))) {
            yAxis = Input.GetAxis("DPad_YAxis_" + num);
        }
        else if (Mathf.Abs(Input.GetAxis("L_YAxis_" + num)) > Mathf.Abs(Input.GetAxis("R_YAxis_" + num))) {
            yAxis = Input.GetAxis("L_YAxis_" + num);
        }
        else {
            yAxis = Input.GetAxis("R_YAxis_" + num);
        }
        A = (Input.GetButton("ButtonA_" + num) || Mathf.Abs(Input.GetAxis("R_XAxis_" + num)) > 0 || Mathf.Abs(Input.GetAxis("R_YAxis_" + num)) > 0);
        B = Input.GetButton("ButtonB_" + num);
        Y = Input.GetButton("ButtonY_" + num);
        X = Input.GetButton("ButtonX_" + num);
        R = Input.GetButton("ButtonR_" + num) || Input.GetButton("ButtonZR_" + num);
        L = Input.GetButton("ButtonL_" + num) || Input.GetButton("ButtonZL_" + num);
    }
}
