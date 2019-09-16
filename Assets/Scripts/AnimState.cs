using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimState : MonoBehaviour {

    public static int Start;
    public static int Idle;

    void Awake() {
        Start = Animator.StringToHash("Base Layer.Start");
        Idle = Animator.StringToHash("Base Layer.Idle");
    }

}
