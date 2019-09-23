using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectMgr : MonoBehaviour {

    //シングルトン
    private static SelectMgr instance;
    public static SelectMgr Instance {
        get {
            if (instance == null) {
                instance = (SelectMgr)FindObjectOfType(typeof(SelectMgr));
                if (instance == null)
                    Debug.LogError(typeof(SelectMgr) + "is nothing");
            }
            return instance;
        }
    }
    void Awake() {
        if (this != Instance) { //２つ目以降のインスタンスは破棄
            Destroy(this.gameObject);
            return;
        }
    }


    [SerializeField] RectTransform Frame1PRTf;
    [SerializeField] RectTransform Frame2PRTf;
    int count1 = 1;
    int count2 = 1;
    bool isReleseAxis1 = true;
    bool isReleseAxis2 = true;
    bool isMovable1 = true;
    bool isMovable2 = true;

    [SerializeField] GameObject settingPanel;

    void Start() {
        Main.state = Main.State.Select;
        Main.CameraSetting();
        Frame1PRTf.sizeDelta = new Vector2(Screen.width / 2, Frame1PRTf.sizeDelta.y);
        Frame1PRTf.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, Frame1PRTf.sizeDelta.y);
        Frame2PRTf.sizeDelta = new Vector2(Screen.width / 2, Frame2PRTf.sizeDelta.y);
        Frame2PRTf.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, Frame2PRTf.sizeDelta.y);
        Frame1PRTf.localPosition = new Vector3(Frame1PRTf.localPosition.x + Screen.width / 4, Frame1PRTf.localPosition.y);
        Frame2PRTf.localPosition = new Vector3(Frame2PRTf.localPosition.x + Screen.width / 4, Frame2PRTf.localPosition.y);
        settingPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Toggle>().Select();//初期
    }

    void Update() {
        //1P
        if (Input.GetAxis("DPad_XAxis_1") > 0 && isReleseAxis1 && count1 < 3 && isMovable1) {
            isReleseAxis1 = false;
            count1++;
            iTween.MoveBy(Frame1PRTf.gameObject, iTween.Hash("x",Screen.width / 4, "time", 0.3f, "oncomplete", "MoveEnd1", "oncompletetarget", gameObject));          
        }
        else if (Input.GetAxis("DPad_XAxis_1") < 0 && isReleseAxis1 && -1 < count1 && isMovable1) {
            isReleseAxis1 = false;
            count1--;
            iTween.MoveBy(Frame1PRTf.gameObject, iTween.Hash("x", -Screen.width / 4, "time", 0.3f, "oncomplete", "MoveEnd1", "oncompletetarget", gameObject));
        }

        //2P
        if (Input.GetAxis("DPad_XAxis_2") > 0 && isReleseAxis2 && count2 < 2 && isMovable2) {
            isReleseAxis2 = false;
            count2++;
            iTween.MoveBy(Frame2PRTf.gameObject, iTween.Hash("x", Screen.width / 4, "time", 0.3f, "oncomplete", "MoveEnd2", "oncompletetarget", gameObject));
        }
        else if (Input.GetAxis("DPad_XAxis_2") < 0 && isReleseAxis2 && -1 < count2 && isMovable2) {
            isReleseAxis2 = false;
            count2--;
            iTween.MoveBy(Frame2PRTf.gameObject, iTween.Hash("x", -Screen.width / 4, "time", 0.3f, "oncomplete", "MoveEnd2", "oncompletetarget", gameObject));
        }

        if (Input.GetKeyDown(KeyCode.Space)) {//仮
            isMovable1 = false;
            isMovable2 = false;
            FadeManager.Instance.LoadScene("Battle", 0.5f);
            Main.state = Main.State.Battle;
        }
    }

    void MoveEnd1() {
        isReleseAxis1 = true;
    }
    void MoveEnd2() {
        isReleseAxis2 = true;
    }
}
