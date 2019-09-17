using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimState : MonoBehaviour {

    //シングルトン
    private static AnimState instance;
    public static AnimState Instance {
        get {
            if (instance == null) {
                instance = (AnimState)FindObjectOfType(typeof(AnimState));
                if (instance == null)
                    Debug.LogError(typeof(AnimState) + "is nothing");
            }
            return instance;
        }
    }

    public int Start;
    public int Idle;
    public int SideA;

    void Awake() {
        if (this != Instance) { //２つ目以降のインスタンスは破棄
            Destroy(this.gameObject);
            return;
        }
        Start = Animator.StringToHash("Base Layer.Start");
        Idle = Animator.StringToHash("Base Layer.Idle");
        SideA = Animator.StringToHash("Base Layer.Side.SideA");
    }

}
