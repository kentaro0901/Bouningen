using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputJoycon : InputMethod {

    int num;
    private void Start() {
        num = (int)controller.playerNum;
    }

    private void Update() {
        xAxis = (num == 0 ? -1 : 1) * Main.joycon[num].GetStick()[1];
        yAxis = (num == 0 ? 1 : -1) * Main.joycon[num].GetStick()[0];
        A = Main.joycon[num].GetButton(num == 0 ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP);
        B = Main.joycon[num].GetButton(num == 0 ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT);
        X = Main.joycon[num].GetButton(num == 0 ? Joycon.Button.DPAD_RIGHT : Joycon.Button.DPAD_LEFT);
        Y = Main.joycon[num].GetButton(num == 0 ? Joycon.Button.DPAD_UP : Joycon.Button.DPAD_DOWN);
        R = Main.joycon[num].GetButton(Joycon.Button.SR);
        L = Main.joycon[num].GetButton(Joycon.Button.SL);
    }
}
