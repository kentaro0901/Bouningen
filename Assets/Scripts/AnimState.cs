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

    //試し
    public enum State {
        Prepare = 1152175762,
        Start = -604752631,
        Idle = 1432961145,
        LightningStart = 46210609,
        Lightning = -30349352,
        LightningAttack = 1803815931,
        NutralA = 1904456718,
        NutralB = -393550412,
        NutralA_R = -1424996316,
        LimitBreak = 1845850332,
        Critical = 1141500039,
        CriticalEnd = 867219792,
        CriticalUp = 201583133,
        CriticalFall = 745423208,
        CriticalFallEnd = 1025367550,
        Death = -1546996312,
        GameEnd = -1709935748,
        Step = 67434498,
        StepEnd = -532869481,
        Dash = -590095554,
        DashEnd = 1800544941,
        SideA = -1445464002,
        SideA_R = 188525368,
        SideB = 819898756,
        SideB_R = 158995809,
        JumpStart = 7176671,
        Jump = 1346490490,
        JumpEnd = -516674402,
        UpA = 320752333,
        UpB = -1978197129,
        Fall = -1898804835,
        Landing = -54786951,
        DownA = -441210437,
        DownB = 2092627969,
        NutralA_Air = -508124560,
        NutralB_Air = -1508452192,
        SideA_Air = -131337290,
        SideB_Air = -1081374362
    }

    public int Prepare;
    public int Start;
    public int Idle;
    public int LightningStart;
    public int Lightning;
    public int LightningAttack;
    public int NutralA;
    public int NutralB;
    public int NutralA_R;
    public int LimitBreak;
    public int Critical;
    public int CriticalEnd;
    public int CriticalUp;
    public int CriticalFall;
    public int CriticalFallEnd;
    public int Death;
    public int GameEnd;

    public int Step;
    public int StepEnd;
    public int Dash;
    public int DashEnd;
    public int SideA;
    public int SideA_R;
    public int SideB;
    public int SideB_R;

    public int JumpStart;
    public int Jump;
    public int JumpEnd;
    public int UpA;
    public int UpB;

    public int Fall;
    public int Landing;
    public int DownA;
    public int DownB;
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
        LightningAttack = Animator.StringToHash("Base Layer.LightningAttack");
        NutralA = Animator.StringToHash("Base Layer.NutralA");
        NutralB = Animator.StringToHash("Base Layer.NutralB");
        NutralA_R = Animator.StringToHash("Base Layer.NutralA_R");
        LimitBreak = Animator.StringToHash("Base Layer.LimitBreak");
        Critical = Animator.StringToHash("Base Layer.Critical");
        CriticalEnd = Animator.StringToHash("Base Layer.CriticalEnd");
        CriticalUp = Animator.StringToHash("Base Layer.CriticalUp");
        CriticalFall = Animator.StringToHash("Base Layer.CriticalFall");
        CriticalFallEnd = Animator.StringToHash("Base Layer.CriticalFallEnd");
        Death = Animator.StringToHash("Base Layer.Death");
        GameEnd = Animator.StringToHash("Base Layer.GameEnd");

        Step = Animator.StringToHash("Base Layer.Side.Step");
        StepEnd = Animator.StringToHash("Base Layer.Side.StepEnd");
        Dash = Animator.StringToHash("Base Layer.Side.Dash");
        DashEnd = Animator.StringToHash("Base Layer.Side.DashEnd");
        SideA = Animator.StringToHash("Base Layer.Side.SideA");
        SideA_R = Animator.StringToHash("Base Layer.Side.SideA_R");
        SideB = Animator.StringToHash("Base Layer.Side.SideB");
        SideB_R = Animator.StringToHash("Base Layer.Side.SideB_R");

        JumpStart = Animator.StringToHash("Base Layer.Up.JumpStart");
        Jump = Animator.StringToHash("Base Layer.Up.Jump");
        JumpEnd = Animator.StringToHash("Base Layer.Up.JumpEnd");
        UpA = Animator.StringToHash("Base Layer.Up.UpA");
        UpB = Animator.StringToHash("Base Layer.Up.UpB");

        Fall = Animator.StringToHash("Base Layer.Down.Fall");
        Landing = Animator.StringToHash("Base Layer.Down.Landing");
        DownA = Animator.StringToHash("Base Layer.Down.DownA");
        DownB = Animator.StringToHash("Base Layer.Down.DownB");
        NutralA_Air = Animator.StringToHash("Base Layer.Down.NutralA_Air");
        NutralB_Air = Animator.StringToHash("Base Layer.Down.NutralB_Air");
        SideA_Air = Animator.StringToHash("Base Layer.Down.SideA_Air");
        SideB_Air = Animator.StringToHash("Base Layer.Down.SideB_Air");
    }

}
