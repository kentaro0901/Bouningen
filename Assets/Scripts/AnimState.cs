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

    public int Prepare;
    public int Start;
    public int Idle;
    public int LightningStart;
    public int Lightning;
    public int LightningEnd;
    public int LightningAttack;
    public int LightningAttackDown;
    public int NutralA;
    public int NutralB;
    public int NutralA_R;
    public int NutralA_RW;
    public int LimitBreak;
    public int Critical;
    public int CriticalEnd;
    public int CriticalNA;
    public int CriticalUp;
    public int CriticalFall;
    public int CriticalFallEnd;
    public int CriticalDown;
    public int Death;
    public int GameEnd;
    public int Wince;
    public int DownB;

    public int Step;
    public int StepEnd;
    public int Dash;
    public int DashEnd;
    public int SideA;
    public int SideA_R;
    public int SideA_RW;
    public int SideB;
    public int SideB_R;
    public int SideB_RW;

    public int JumpStart;
    public int Jump;
    public int JumpEnd;
    public int UpA;
    public int UpB;
    public int UpB_Fall;

    public int Fall;
    public int Landing;
    public int DownA;
    public int DownB_Air;
    public int NutralA_Air;
    public int NutralB_Air;
    public int SideA_Air;
    public int SideB_Air;

    void Awake() {
        if (this != Instance) { //２つ目以降のインスタンスは破棄
            Destroy(this.gameObject);
            return;
        }
        Prepare = Animator.StringToHash("Base Layer.Prepare");
        Start = Animator.StringToHash("Base Layer.Start");
        Idle = Animator.StringToHash("Base Layer.Idle");
        LightningStart = Animator.StringToHash("Base Layer.LightningStart");
        Lightning = Animator.StringToHash("Base Layer.Lightning");
        LightningEnd = Animator.StringToHash("Base Layer.LightningEnd");
        LightningAttack = Animator.StringToHash("Base Layer.LightningAttack");
        LightningAttackDown = Animator.StringToHash("Base Layer.LightningAttackDown");
        NutralA = Animator.StringToHash("Base Layer.NutralA");
        NutralB = Animator.StringToHash("Base Layer.NutralB");
        NutralA_R = Animator.StringToHash("Base Layer.NutralA_R");
        NutralA_RW = Animator.StringToHash("Base Layer.NutralA_RW");
        LimitBreak = Animator.StringToHash("Base Layer.LimitBreak");
        Critical = Animator.StringToHash("Base Layer.Critical");
        CriticalEnd = Animator.StringToHash("Base Layer.CriticalEnd");
        CriticalNA = Animator.StringToHash("Base Layer.CriticalNA");
        CriticalUp = Animator.StringToHash("Base Layer.CriticalUp");
        CriticalFall = Animator.StringToHash("Base Layer.CriticalFall");
        CriticalFallEnd = Animator.StringToHash("Base Layer.CriticalFallEnd");
        CriticalDown = Animator.StringToHash("Base Layer.CriticalDown");
        Death = Animator.StringToHash("Base Layer.Death");
        GameEnd = Animator.StringToHash("Base Layer.GameEnd");
        Wince = Animator.StringToHash("Base Layer.Wince");
        DownB = Animator.StringToHash("Base Layer.DownB");

        Step = Animator.StringToHash("Base Layer.Side.Step");
        StepEnd = Animator.StringToHash("Base Layer.Side.StepEnd");
        Dash = Animator.StringToHash("Base Layer.Side.Dash");
        DashEnd = Animator.StringToHash("Base Layer.Side.DashEnd");
        SideA = Animator.StringToHash("Base Layer.Side.SideA");
        SideA_R = Animator.StringToHash("Base Layer.Side.SideA_R");
        SideA_RW = Animator.StringToHash("Base Layer.Side.SideA_RW");
        SideB = Animator.StringToHash("Base Layer.Side.SideB");
        SideB_R = Animator.StringToHash("Base Layer.Side.SideB_R");
        SideB_RW = Animator.StringToHash("Base Layer.Side.SideB_RW");

        JumpStart = Animator.StringToHash("Base Layer.Up.JumpStart");
        Jump = Animator.StringToHash("Base Layer.Up.Jump");
        JumpEnd = Animator.StringToHash("Base Layer.Up.JumpEnd");
        UpA = Animator.StringToHash("Base Layer.Up.UpA");
        UpB = Animator.StringToHash("Base Layer.Up.UpB");
        UpB_Fall = Animator.StringToHash("Base Layer.Up.UpB_Fall");

        Fall = Animator.StringToHash("Base Layer.Down.Fall");
        Landing = Animator.StringToHash("Base Layer.Down.Landing");
        DownA = Animator.StringToHash("Base Layer.Down.DownA");
        DownB_Air = Animator.StringToHash("Base Layer.Down.DownB_Air");
        NutralA_Air = Animator.StringToHash("Base Layer.Down.NutralA_Air");
        NutralB_Air = Animator.StringToHash("Base Layer.Down.NutralB_Air");
        SideA_Air = Animator.StringToHash("Base Layer.Down.SideA_Air");
        SideB_Air = Animator.StringToHash("Base Layer.Down.SideB_Air");
    }

}
