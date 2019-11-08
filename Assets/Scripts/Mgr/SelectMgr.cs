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

    enum SelectState {
        Loading,
        Select,
        Setting,
        Manual,
        Ready,
    }
    SelectState selectState1 = SelectState.Loading;
    SelectState selectState2 = SelectState.Loading;

    //入力
    float[] xAxis = {0.0f, 0.0f};
    bool[] a = {false, false};
    bool[] b = {false, false};
    bool[] x = { false, false };

    [SerializeField] RectTransform Frame1PRTf;
    [SerializeField] RectTransform Frame2PRTf;
    [SerializeField] RectTransform Frame1PS;
    [SerializeField] RectTransform Frame2PS;
    int count1 = 1;
    int count2 = 1;
    bool isReleseAxis1 = true;
    bool isReleseAxis2 = true;

    [SerializeField] GameObject settingPanel;
    [SerializeField] Toggle multiDisplays;
    [SerializeField] Toggle singleDisplay;
    [SerializeField] Toggle dynamicCamera;
    [SerializeField] Toggle normalCamera;
    [SerializeField] Toggle visibleBox;
    [SerializeField] Toggle invisibleBox;
    [SerializeField] Toggle visibleUI;
    [SerializeField] Toggle invisibleUI;
    [SerializeField] Slider cameraSize;
    [SerializeField] Slider volume;
    [SerializeField] Slider subVolume;
    [SerializeField] Slider gameSpeed;
    [SerializeField] Text cameraSizeValue;
    [SerializeField] Text volumeValue;
    [SerializeField] Text subVolumeValue;
    [SerializeField] Text gameSpeedValue;
    bool isMultiDisplays; //経由しないとうまくいかない
    bool isDynamicCamera;
    bool isVisibleBox;
    bool isVisibleUI;

    [SerializeField] GameObject[] manualPanel1;
    [SerializeField] GameObject[] manualPanel2;
    int manualCount1 = 0;
    int manualCount2 = 0;

    [SerializeField] GameObject readyPanel1;
    [SerializeField] GameObject readyPanel2;
    Vector2 readyPanelInitSize1 = Vector2.one;
    Vector2 readyPanelInitSize2 = Vector2.one;
    [SerializeField] Image sumiCircle1;
    [SerializeField] Image sumiCircle2;
    [SerializeField] GameObject ready1S;
    [SerializeField] GameObject ready2S;
    [SerializeField] Image sumiCircleS;
    int readyCount = 0;

    [SerializeField] Material red;
    [SerializeField] Material blue;
    [SerializeField] Material gray;

    void Start() {
        Main.state = Main.State.Select;
        Main.Instance.UICameraSetting();
        if (!Main.Instance.isMultiDisplays) { //シングル用
            Frame1PRTf.transform.parent.gameObject.SetActive(false);
            Frame1PRTf = Frame1PS;
            Frame2PRTf = Frame2PS;
            readyPanel1.SetActive(false);
            readyPanel2.SetActive(false);
            readyPanel1 = ready1S;
            readyPanel2 = ready2S;
            sumiCircle1 = sumiCircleS;
        }
        else {
            Frame1PS.transform.parent.gameObject.SetActive(false);
            ready1S.SetActive(false);
            ready2S.SetActive(false);
        }

        if(Main.Instance.playerType[1] == Main.PlayerType.AI) {//2P
            Frame2PRTf.gameObject.GetComponent<Image>().material = gray;
            readyPanel2.gameObject.GetComponent<Image>().material = gray;
        }

        Frame1PRTf.sizeDelta = new Vector2(Screen.width / 2, Frame1PRTf.sizeDelta.y);
        Frame1PRTf.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, Frame1PRTf.sizeDelta.y);
        Frame2PRTf.sizeDelta = new Vector2(Screen.width / 2, Frame2PRTf.sizeDelta.y);
        Frame2PRTf.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, Frame2PRTf.sizeDelta.y);
        Frame1PRTf.localPosition = new Vector3(Frame1PRTf.localPosition.x + Screen.width / 4, Frame1PRTf.localPosition.y);
        Frame2PRTf.localPosition = new Vector3(Frame2PRTf.localPosition.x + Screen.width / 4, Frame2PRTf.localPosition.y);

        isMultiDisplays = Main.Instance.isMultiDisplays;
        multiDisplays.SetIsOnWithoutNotify(isMultiDisplays);
        singleDisplay.SetIsOnWithoutNotify(!isMultiDisplays);
        isDynamicCamera = Main.Instance.isDynamicCamera;
        dynamicCamera.SetIsOnWithoutNotify(isDynamicCamera);
        normalCamera.SetIsOnWithoutNotify(!isDynamicCamera);
        isVisibleBox = Main.Instance.isVisibleBox;
        visibleBox.SetIsOnWithoutNotify(isVisibleBox);
        invisibleBox.SetIsOnWithoutNotify(!isVisibleBox);
        isVisibleUI = Main.Instance.isVisibleUI;
        visibleUI.SetIsOnWithoutNotify(isVisibleUI);
        invisibleUI.SetIsOnWithoutNotify(!isVisibleUI);
        cameraSize.value = Main.Instance.cameraSize;
        volume.value = Main.Instance.mainBgm.volume * volume.maxValue;
        subVolume.value = Main.Instance.subBgm.volume * subVolume.maxValue;
        gameSpeed.value = Main.Instance.gameSpeed * 10;
        cameraSizeValue.text = "" + cameraSize.value;
        volumeValue.text = "" + volume.value;
        subVolumeValue.text = "" + subVolume.value;
        gameSpeedValue.text = "×" + (gameSpeed.value * 0.1f);
        Selectable[] sel = settingPanel.GetComponentsInChildren<Selectable>();
        foreach(Selectable s in sel) {
            s.interactable = false;
        }
        settingPanel.GetComponent<Canvas>().sortingOrder = -1;
        foreach(GameObject g in manualPanel1) {
            g.SetActive(false);
        }
        foreach(GameObject g in manualPanel2) {
            g.SetActive(false);
        }
        readyPanelInitSize1 = readyPanel1.GetComponent<RectTransform>().localScale;
        readyPanelInitSize2 = readyPanel2.GetComponent<RectTransform>().localScale;
        readyPanel1.SetActive(false);
        readyPanel2.SetActive(false);
    }

    void Update() {

        //Input
        for(int i = 0; i <= 1; i++) {
            if (Main.controller[i] == Main.Controller.None) break;
            int n = 0;
            if (Main.Instance.isSwapController) n =1;
            switch (Main.controller[i]) {
                case Main.Controller.GamePad:
                    xAxis[i] = Input.GetAxis("DPad_XAxis_" + (i + 1 + n));
                    a[i] = Input.GetButtonDown("ButtonA_" + (i + 1 + n));
                    b[i] = Input.GetButtonDown("ButtonB_" + (i + 1 + n));
                    x[i] = Input.GetButtonDown("ButtonX_" + (i + 1 + n));
                    break;
                case Main.Controller.Joycon:
                    xAxis[i] = (i + n == 0 ? -1 : 1) * Main.joycon[i + n].GetStick()[1];
                    a[i] = Main.joycon[i + n].GetButtonDown(i + n == 0 ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP);
                    b[i] = Main.joycon[i + n].GetButtonDown(i + n == 0 ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT);
                    x[i] = Main.joycon[i + n].GetButtonDown(i + n == 0 ? Joycon.Button.DPAD_RIGHT : Joycon.Button.DPAD_LEFT);
                    break;
                default:
                    xAxis[i] = 0.0f;
                    a[i] = false;
                    b[i] = false;
                    x[i] = false;
                    break;
            }
        }

        //2P
        switch (selectState2) {
            case SelectState.Loading:
                if (!FadeManager.Instance.isFading) {
                    selectState2 = SelectState.Select;
                }
                break;
            case SelectState.Select:
                if (x[1] ||(Main.controller[1] == Main.Controller.None && selectState1 == SelectState.Ready && x[0])) ChangePlayerType(1);
                if ((xAxis[1] > 0 || (Main.controller[1] == Main.Controller.None && selectState1 == SelectState.Ready && xAxis[0] > 0)) && isReleseAxis2 && count2 < 2) { //右
                    isReleseAxis2 = false;
                    count2++;
                    iTween.MoveBy(Frame2PRTf.gameObject, iTween.Hash("x", Screen.width / 4, "time", 0.2f, "oncomplete", "MoveEnd2", "oncompletetarget", gameObject));
                }
                else if ((xAxis[1] < 0 || (Main.controller[1] == Main.Controller.None && selectState1 == SelectState.Ready && xAxis[0] < 0)) && isReleseAxis2 && (Main.Instance.isMultiDisplays? -1: 0) < count2) { //左
                    isReleseAxis2 = false;
                    count2--;
                    iTween.MoveBy(Frame2PRTf.gameObject, iTween.Hash("x", -Screen.width / 4, "time", 0.2f, "oncomplete", "MoveEnd2", "oncompletetarget", gameObject));
                }
                if (count2 == -1 && isReleseAxis2 && a[1]) { //操作開く
                    selectState2 = SelectState.Manual;
                    manualCount2 = 0;
                    manualPanel2[0].SetActive(true);
                }
                if (count2 == 1 && isReleseAxis2 && (a[1]|| (Main.controller[1] == Main.Controller.None && selectState1 == SelectState.Ready && a[0]))) { //剣
                    Main.Instance.chara2P = Main.Chara.Sword;
                    selectState2 = SelectState.Ready;
                    readyPanel2.SetActive(true);
                    readyPanel2.GetComponent<RectTransform>().localScale = readyPanelInitSize2;
                    iTween.ScaleFrom(readyPanel2, iTween.Hash("y", 0, "islocal", true, "time", 0.5f));
                }
                break;
            case SelectState.Manual:
                if (a[1]) {
                    if (manualCount2 < manualPanel2.Length-1) {
                        manualPanel2[manualCount2].SetActive(false);
                        manualCount2++;
                        manualPanel2[manualCount2].SetActive(true);
                    }
                    else {
                        manualPanel2[manualCount2].SetActive(false);
                        selectState2 = SelectState.Select;
                    }
                }
                if (b[1]) {
                    if (0 < manualCount2) {
                        manualPanel2[manualCount2].SetActive(false);
                        manualCount2--;
                        manualPanel2[manualCount2].SetActive(true);
                    }
                    else {
                        manualPanel2[0].SetActive(false);
                        selectState2 = SelectState.Select;
                    }
                }
                break;
            case SelectState.Setting: break;
            case SelectState.Ready:
                if (b[1]) { //戻る
                    selectState2 = SelectState.Select;
                    readyPanel2.SetActive(false);
                }
                break;
        }

        //1P
        switch (selectState1) {
            case SelectState.Loading:
                if (!FadeManager.Instance.isFading) {
                    selectState1 = SelectState.Select;
                }
                break;
            case SelectState.Select:
                if (x[0])
                    ChangePlayerType(0);
                if (xAxis[0] > 0 && isReleseAxis1 && count1 < 3) { //右移動
                    isReleseAxis1 = false;
                    count1++;
                    iTween.MoveBy(Frame1PRTf.gameObject, iTween.Hash("x", Screen.width / 4, "time", 0.2f, "oncomplete", "MoveEnd1", "oncompletetarget", gameObject));
                }
                else if (xAxis[0] < 0 && isReleseAxis1 && -1 < count1) { //左移動
                    isReleseAxis1 = false;
                    count1--;
                    iTween.MoveBy(Frame1PRTf.gameObject, iTween.Hash("x", -Screen.width / 4, "time", 0.2f, "oncomplete", "MoveEnd1", "oncompletetarget", gameObject));
                }
                if (count1 == -1 && isReleseAxis1 && a[0]) { //操作開く
                    selectState1 = SelectState.Manual;
                    manualCount1 = 0;
                    manualPanel1[0].SetActive(true);
                }
                if (count1 == 1 && isReleseAxis1 && a[0]) { //剣
                    Main.Instance.chara1P = Main.Chara.Sword;
                    selectState1 = SelectState.Ready;
                    readyPanel1.SetActive(true);
                    readyPanel1.GetComponent<RectTransform>().localScale = readyPanelInitSize1;
                    iTween.ScaleFrom(readyPanel1, iTween.Hash("y", 0, "islocal", true, "time", 0.5f));
                }
                if (count1 == 3 && isReleseAxis1 && a[0]) { //設定開く
                    selectState1 = SelectState.Setting;
                    settingPanel.GetComponent<Canvas>().sortingOrder = 1;
                    Selectable[] sel = settingPanel.GetComponentsInChildren<Selectable>();
                    foreach (Selectable s in sel) {
                        s.interactable = true;
                    }
                    settingPanel.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Toggle>().interactable = false;
                    settingPanel.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.GetComponent<Toggle>().interactable = false;
                    settingPanel.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<Toggle>().Select();//初期
                }
                break;
            case SelectState.Manual:
                if (a[0]) {
                    if (manualCount1 < manualPanel1.Length - 1) {
                        manualPanel1[manualCount1].SetActive(false);
                        manualCount1++;
                        manualPanel1[manualCount1].SetActive(true);
                    }
                    else {
                        manualPanel1[manualCount1].SetActive(false);
                        selectState1 = SelectState.Select;
                    }
                }
                if (b[0]) {
                    if (0 < manualCount1) {
                        manualPanel1[manualCount1].SetActive(false);
                        manualCount1--;
                        manualPanel1[manualCount1].SetActive(true);
                    }
                    else {
                        manualPanel1[0].SetActive(false);
                        selectState1 = SelectState.Select;
                    }
                }
                break;
            case SelectState.Setting:
                if (b[0]) { //設定閉じる
                    Selectable[] sel = settingPanel.GetComponentsInChildren<Selectable>();
                    foreach (Selectable s in sel) {
                        s.interactable = false;
                    }
                    settingPanel.GetComponent<Canvas>().sortingOrder = -1;
                    selectState1 = SelectState.Select;
                }
                break;
            case SelectState.Ready:
                if (b[0]) { //戻る
                    selectState1 = SelectState.Select;
                    readyPanel1.SetActive(false);
                }
                break;
        }

        if (selectState1 == SelectState.Ready && selectState2 == SelectState.Ready) {
            readyCount++;
        }
        else if(selectState1 == SelectState.Select || selectState2 == SelectState.Select){
            readyCount = 0;
        }

        sumiCircle1.fillAmount = (float)readyCount / 60;
        sumiCircle2.fillAmount = (float)readyCount / 60;

        if(60 <= readyCount && Main.state == Main.State.Select) { //シーン遷移
            selectState1 = SelectState.Loading;
            selectState2 = SelectState.Loading;
            Main.Instance.isMultiDisplays = isMultiDisplays;
            Main.Instance.isDynamicCamera = isDynamicCamera;
            Main.Instance.isVisibleBox = isVisibleBox;
            Main.Instance.isVisibleUI = isVisibleUI;
            Main.state = Main.State.Battle;
            FadeManager.Instance.LoadScene("Battle", 0.5f);
        }
    }

    public void ChangeMultiDisplays(Toggle toggle) {
        isMultiDisplays = toggle.isOn;
    }
    public void ChangeDynamicCamera(Toggle toggle) {
        isDynamicCamera = toggle.isOn;
    }
    public void ChangeVisibleBox(Toggle toggle) {
        isVisibleBox = toggle.isOn;
    }
    public void ChangeVisibleUI(Toggle toggle) {
        isVisibleUI = toggle.isOn;
    }
    public void ChangeCameraSize(Slider slider) {
        Main.Instance.cameraSize = slider.value;
        cameraSizeValue.text = "" + slider.value;
    }
    public void ChangeVolume(Slider slider) {
        Main.Instance.mainBgm.volume = slider.value / slider.maxValue;
        volumeValue.text = "" + slider.value;
    }
    public void ChangeSubVolume(Slider slider) {
        Main.Instance.subBgm.volume = slider.value / slider.maxValue;
        subVolumeValue.text = "" + slider.value;
    }
    public void ChangeGameSpeed(Slider slider) {
        Main.Instance.gameSpeed = slider.value * 0.1f;
        gameSpeedValue.text = "×" + (slider.value * 0.1f);
    }
    public void SetDefault() {
        multiDisplays.SetIsOnWithoutNotify(true);
        dynamicCamera.SetIsOnWithoutNotify(true);
        invisibleBox.SetIsOnWithoutNotify(true);
        visibleUI.SetIsOnWithoutNotify(true);
        isMultiDisplays = true;
        isDynamicCamera = true;
        isVisibleBox = false;
        isVisibleUI = true;
        cameraSize.value = 6.0f;
        volume.value = 10.0f;
        subVolume.value = 5.0f;
        gameSpeed.value = 10.0f;
        cameraSizeValue.text = "" + cameraSize.value;
        volumeValue.text = "" + volume.value;
        subVolumeValue.text = "" + subVolume.value;
        gameSpeedValue.text = "×" + (gameSpeed.value * 0.1f);
}

    void MoveEnd1() {
        isReleseAxis1 = true;
    }
    void MoveEnd2() {
        isReleseAxis2 = true;
    }
    void ChangePlayerType(int num) {
        switch (Main.Instance.playerType[num]) {
            case Main.PlayerType.None: break;
            case Main.PlayerType.Player: 
                Main.Instance.playerType[num] = Main.PlayerType.AI;
                if (num == 0) { //1P
                    Frame1PRTf.gameObject.GetComponent<Image>().material = gray;
                    readyPanel1.gameObject.GetComponent<Image>().material = gray;
                }
                else {
                    Frame2PRTf.gameObject.GetComponent<Image>().material = gray;
                    readyPanel2.gameObject.GetComponent<Image>().material = gray;
                }
                break;
            case Main.PlayerType.AI:
                Main.Instance.playerType[num] = Main.PlayerType.Player;
                if (num == 0) { //1P
                    Frame1PRTf.gameObject.GetComponent<Image>().material = red;
                    readyPanel1.gameObject.GetComponent<Image>().material = red;
                }
                else {
                    Frame2PRTf.gameObject.GetComponent<Image>().material = blue;
                    readyPanel2.gameObject.GetComponent<Image>().material = blue;
                }
                //Main.Instance.playerType[num] = Main.PlayerType.Online;
                break;
            case Main.PlayerType.Online:
                Main.Instance.playerType[num] = Main.PlayerType.Player;break;
            default: break;
        }
    }
}
