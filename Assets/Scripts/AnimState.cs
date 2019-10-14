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

    //状態
    public static int Prepare;
    public static int StartGame;
    public static int Idle;
    public static int LightningStart;
    public static int Lightning;
    public static int LightningEnd;
    public static int LightningAttack;
    public static int LightningAttackDown;
    public static int NutralA;
    public static int NutralB;
    public static int NutralA_R;
    public static int NutralA_RW;
    public static int LimitBreak;
    public static int Critical;
    public static int CriticalEnd;
    public static int CriticalNA;
    public static int CriticalUp;
    public static int CriticalFall;
    public static int CriticalFallEnd;
    public static int CriticalDown;
    public static int Death;
    public static int GameEnd;
    public static int Wince;
    public static int DownB;

    public static int Step;
    public static int StepEnd;
    public static int Dash;
    public static int DashEnd;
    public static int SideA;
    public static int SideA_R;
    public static int SideA_RW;
    public static int SideB;
    public static int SideB_R;
    public static int SideB_RW;

    public static int JumpStart;
    public static int Jump;
    public static int JumpEnd;
    public static int UpA;
    public static int UpB;
    public static int UpB_Fall;

    public static int Fall;
    public static int Landing;
    public static int DownA;
    public static int DownA_Air;
    public static int DownB_Air;
    public static int DownB_Air_Fall;
    public static int NutralA_Air;
    public static int NutralA_Air_R;
    public static int NutralA_Air_RW;
    public static int NutralB_Air;
    public static int SideA_Air;
    public static int SideA_Air_R;
    public static int SideA_Air_RW;
    public static int SideB_Air;
    public static int SideB_Air_R;
    public static int SideB_Air_RW;

    //パラメーター名
    public static string UpArrow = "UpArrow";
    public static string DownArrow = "DownArrow";
    public static string LeftArrow = "LeftArrow";
    public static string RightArrow = "RightArrow";
    public static string ButtonA = "ButtonA";
    public static string ButtonB = "ButtonB";
    public static string ButtonX = "ButtonX";
    public static string ButtonY = "ButtonY";
    public static string ButtonR = "ButtonR";
    public static string ButtonL = "ButtonL";
    public static string isRight = "isRight";
    public static string isLand = "isLand";
    public static string isStartResist = "isStartResist";
    public static string isWinResist = "isWinResist";
    public static string isLoseResist = "isLoseResist";
    public static string isDrawResist = "isDrawResist";
    public static string isCriticalSide = "isCriticalSide";
    public static string isCriticalUp = "isCriticalUp";
    public static string isCriticalDown = "isCriticalDown";
    public static string isCriticalEnd = "isCriticalEnd";
    public static string isCriticalFinish = "isCriticalFinish";

    //レイヤー名
    string BL = "Base Layer.";
    string Land = "Base Layer.Land.";
    string Air = "Base Layer.Air.";
    string Criticals = "Base Layer.Criticals.";
    string Lightnings = "Base Layer.Lightnings.";

    void Awake() {
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        //Base
        Prepare = Animator.StringToHash(BL+"Prepare");
        StartGame = Animator.StringToHash(BL+"Start");
        Idle = Animator.StringToHash(BL+"Idle");
        LimitBreak = Animator.StringToHash(BL+"LimitBreak");
        Death = Animator.StringToHash(BL+"Death");
        GameEnd = Animator.StringToHash(BL+"GameEnd");
          
        //Land
        Step = Animator.StringToHash(Land+"Step");
        StepEnd = Animator.StringToHash(Land+"StepEnd");
        Dash = Animator.StringToHash(Land+"Dash");
        DashEnd = Animator.StringToHash(Land+"DashEnd");
        NutralA = Animator.StringToHash(Land+"NutralA");
        NutralA_R = Animator.StringToHash(Land+"NutralA_R");
        NutralA_RW = Animator.StringToHash(Land+"NutralA_RW");
        NutralB = Animator.StringToHash(Land+"NutralB");
        SideA = Animator.StringToHash(Land+"SideA");
        SideA_R = Animator.StringToHash(Land+"SideA_R");
        SideA_RW = Animator.StringToHash(Land+"SideA_RW");
        SideB = Animator.StringToHash(Land+"SideB");
        SideB_R = Animator.StringToHash(Land+"SideB_R");
        SideB_RW = Animator.StringToHash(Land+"SideB_RW");
        DownA = Animator.StringToHash(Land+"DownA");
        DownB = Animator.StringToHash(Land+"DownB");

        //Air
        JumpStart = Animator.StringToHash(Air+"JumpStart");
        Jump = Animator.StringToHash(Air+"Jump");
        JumpEnd = Animator.StringToHash(Air+"JumpEnd");
        Fall = Animator.StringToHash(Air+"Fall");
        Landing = Animator.StringToHash(Air+"Landing");
        UpA = Animator.StringToHash(Air+"UpA");
        UpB = Animator.StringToHash(Air+"UpB");
        UpB_Fall = Animator.StringToHash(Air+"UpB_Fall");
        NutralA_Air = Animator.StringToHash(Air+"NutralA_Air");
        NutralA_Air_R = Animator.StringToHash(Air+"NutralA_Air_R");
        NutralA_Air_RW = Animator.StringToHash(Air+"NutralA_Air_RW");
        NutralB_Air = Animator.StringToHash(Air+"NutralB_Air");
        SideA_Air = Animator.StringToHash(Air+"SideA_Air");
        SideA_Air_R = Animator.StringToHash(Air+"SideA_Air_R");
        SideA_Air_RW = Animator.StringToHash(Air+"SideA_Air_RW");
        SideB_Air = Animator.StringToHash(Air+"SideB_Air");
        SideB_Air_R = Animator.StringToHash(Air + "SideB_Air_R");
        SideB_Air_RW = Animator.StringToHash(Air + "SideB_Air_RW");
        DownA_Air = Animator.StringToHash(Air+"DownA_Air");
        DownB_Air = Animator.StringToHash(Air+"DownB_Air");
        DownB_Air_Fall = Animator.StringToHash(Air+"DownB_Air_Fall");

        //Criticals
        Critical = Animator.StringToHash(Criticals+"Critical");
        CriticalEnd = Animator.StringToHash(Criticals+"CriticalEnd");
        CriticalNA = Animator.StringToHash(Criticals+"CriticalNA");
        CriticalUp = Animator.StringToHash(Criticals+"CriticalUp");
        CriticalFall = Animator.StringToHash(Criticals+"CriticalFall");
        CriticalFallEnd = Animator.StringToHash(Criticals+"CriticalFallEnd");
        CriticalDown = Animator.StringToHash(Criticals+"CriticalDown");
        Wince = Animator.StringToHash(Criticals+"Wince");

        //Lightnings
        LightningStart = Animator.StringToHash(Lightnings+"LightningStart");
        Lightning = Animator.StringToHash(Lightnings+"Lightning");
        LightningEnd = Animator.StringToHash(Lightnings+"LightningEnd");
        LightningAttack = Animator.StringToHash(Lightnings+"LightningAttack");
        LightningAttackDown = Animator.StringToHash(Lightnings+"LightningAttackDown");
    }
}
