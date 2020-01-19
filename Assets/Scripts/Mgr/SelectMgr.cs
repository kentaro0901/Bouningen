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
    enum FocusPanel {
        Manual,
        Fighting,
        Sword,
        Hammer,
        Setting
    }
    SelectState[] selectState = { SelectState.Loading, SelectState.Loading };
    FocusPanel[] focusPanel = { FocusPanel.Sword, FocusPanel.Sword};
    bool[] isReleseAxis = { true, true };

    //入力
    float[] xAxis = {0.0f, 0.0f};
    bool[] a = {false, false};
    bool[] b = {false, false};
    bool[] x = { false, false };

    [SerializeField] RectTransform[] FramePRTf;
    [SerializeField] RectTransform Frame1PS;
    [SerializeField] RectTransform Frame2PS;
   
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

    [SerializeField] GameObject[] manualPanels;
    int[] manualPage = { 0, 0 };

    [SerializeField] GameObject[] readyPanel;
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
        Main.gameState = Main.GameState.Select;
        Main.Instance.TitleCameraSetting();
        if (!Main.Instance.isMultiDisplays) { //シングル用
            FramePRTf[0].transform.parent.gameObject.SetActive(false);
            FramePRTf[0] = Frame1PS;
            FramePRTf[1] = Frame2PS;
            readyPanel[0].SetActive(false);
            readyPanel[1].SetActive(false);
            readyPanel[0] = ready1S;
            readyPanel[1] = ready2S;
            sumiCircle1 = sumiCircleS;
        }
        else {
            Frame1PS.transform.parent.gameObject.SetActive(false);
            ready1S.SetActive(false);
            ready2S.SetActive(false);
        }

        if (Main.Instance.playerType[0] == Main.PlayerType.AI) {//1P
            FramePRTf[0].gameObject.GetComponent<Image>().material = gray;
            readyPanel[0].gameObject.GetComponent<Image>().material = gray;
        }
        if (Main.Instance.playerType[1] == Main.PlayerType.AI) {//2P
            FramePRTf[1].gameObject.GetComponent<Image>().material = gray;
            readyPanel[1].gameObject.GetComponent<Image>().material = gray;
        }

        for(int i=0; i < 2; i++) {
            FramePRTf[i].sizeDelta = new Vector2(Screen.width / 2, FramePRTf[i].sizeDelta.y);
            FramePRTf[i].GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2, FramePRTf[i].sizeDelta.y);
            FramePRTf[i].localPosition = new Vector3(FramePRTf[i].localPosition.x + Screen.width / 4, FramePRTf[i].localPosition.y);
        }

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
        for(int i = 0; i<2; i++) {
            for(int j=0; j< manualPanels[i].transform.childCount; j++) {
                manualPanels[i].transform.GetChild(j).gameObject.SetActive(false);
            }
        }
        readyPanelInitSize1 = readyPanel[0].GetComponent<RectTransform>().localScale;
        readyPanelInitSize2 = readyPanel[1].GetComponent<RectTransform>().localScale;
        readyPanel[0].SetActive(false);
        readyPanel[1].SetActive(false);
    }

    void Update() {

        //Input
        for(int i = 0; i <= 1; i++) {
            if (Main.controller[i] == Main.Controller.None) break;
            switch (Main.controller[i]) {
                case Main.Controller.GamePad:
                    xAxis[i] = Input.GetAxis("DPad_XAxis_" + (i + 1));
                    a[i] = Input.GetButtonDown("ButtonA_" + (i + 1));
                    b[i] = Input.GetButtonDown("ButtonB_" + (i + 1));
                    x[i] = Input.GetButtonDown("ButtonX_" + (i + 1));
                    break;
                case Main.Controller.Joycon:
                    xAxis[i] = (i == 0 ? -1 : 1) * Main.joycon[i].GetStick()[1];
                    a[i] = Main.joycon[i].GetButtonDown(i == 0 ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP);
                    b[i] = Main.joycon[i].GetButtonDown(i == 0 ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT);
                    x[i] = Main.joycon[i].GetButtonDown(i == 0 ? Joycon.Button.DPAD_RIGHT : Joycon.Button.DPAD_LEFT);
                    break;
                default:
                    xAxis[i] = 0.0f;
                    a[i] = false;
                    b[i] = false;
                    x[i] = false;
                    break;
            }
        }

        //Select
        for(int i = 0; i < 2; i++) {
            switch (selectState[i]) {
                case SelectState.Loading:
                    if (!FadeManager.Instance.isFading) {
                        selectState[i] = SelectState.Select;
                    } break;
                case SelectState.Select:
                    if (x[i]) { //X
                        ChangePlayerType(i);
                    }
                    if (isReleseAxis[i]) {
                        if (0 < xAxis[i] && focusPanel[i] < (i == 0 ? FocusPanel.Setting:FocusPanel.Hammer)) { //右
                            isReleseAxis[i] = false;
                            focusPanel[i]++;
                            string s = i == 0 ? "MoveEnd1" : "MoveEnd2";
                            iTween.MoveBy(FramePRTf[i].gameObject, iTween.Hash("x", Screen.width / 4, "time", 0.2f, "oncomplete", s, "oncompletetarget", gameObject));
                        }
                        else if (xAxis[i] < 0 && (i==0?FocusPanel.Manual:(Main.Instance.isMultiDisplays? FocusPanel.Manual: FocusPanel.Fighting)) < focusPanel[i]) { //左
                            isReleseAxis[i] = false;
                            focusPanel[i]--;
                            string s = i == 0 ? "MoveEnd1" : "MoveEnd2";
                            iTween.MoveBy(FramePRTf[i].gameObject, iTween.Hash("x", -Screen.width / 4, "time", 0.2f, "oncomplete", s, "oncompletetarget", gameObject));
                        }
                        else if (a[i]) { //A
                            switch (focusPanel[i]) {
                                case FocusPanel.Manual:
                                    selectState[i] = SelectState.Manual;
                                    manualPage[i] = 0;
                                    manualPanels[i].transform.GetChild(0).gameObject.SetActive(true);
                                    break;
                                case FocusPanel.Fighting:
                                    Main.Instance.chara[i] = Main.Chara.Fighter;
                                    selectState[i] = SelectState.Ready;
                                    readyPanel[i].SetActive(true);
                                    readyPanel[i].GetComponent<RectTransform>().localScale = readyPanelInitSize1;
                                    iTween.ScaleFrom(readyPanel[i], iTween.Hash("y", 0, "islocal", true, "time", 0.5f));
                                    break;
                                case FocusPanel.Sword:
                                    Main.Instance.chara[i] = Main.Chara.Sword;
                                    selectState[i] = SelectState.Ready;
                                    readyPanel[i].SetActive(true);
                                    readyPanel[i].GetComponent<RectTransform>().localScale = readyPanelInitSize1;
                                    iTween.ScaleFrom(readyPanel[i], iTween.Hash("y", 0, "islocal", true, "time", 0.5f));
                                    break;
                                case FocusPanel.Hammer:
                                    Main.Instance.chara[i] = Main.Chara.Hammer;
                                    selectState[i] = SelectState.Ready;
                                    readyPanel[i].SetActive(true);
                                    readyPanel[i].GetComponent<RectTransform>().localScale = readyPanelInitSize1;
                                    iTween.ScaleFrom(readyPanel[i], iTween.Hash("y", 0, "islocal", true, "time", 0.5f));
                                    break;
                                case FocusPanel.Setting:
                                    if (i == 1) break;
                                    selectState[i] = SelectState.Setting;
                                    settingPanel.GetComponent<Canvas>().sortingOrder = 1;
                                    Selectable[] sel = settingPanel.GetComponentsInChildren<Selectable>();
                                    foreach (Selectable s in sel) {
                                        s.interactable = true;
                                    }
                                    settingPanel.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Toggle>().interactable = false;
                                    settingPanel.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.GetComponent<Toggle>().interactable = false;
                                    settingPanel.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<Toggle>().Select();//初期
                                    break;
                                default: break;
                            }
                        }                      
                    }                  
                    break;
                case SelectState.Manual:
                    if (a[i]) {
                        if (manualPage[i] < manualPanels[i].transform.childCount-1) {
                            manualPanels[i].transform.GetChild(manualPage[i]).gameObject.SetActive(false);
                            manualPage[i]++;
                            manualPanels[i].transform.GetChild(manualPage[i]).gameObject.SetActive(true);
                        }
                        else {
                            manualPanels[i].transform.GetChild(manualPage[i]).gameObject.SetActive(false);
                            selectState[i] = SelectState.Select;
                        }
                    }
                    else if (b[i]) {
                        if (0 < manualPage[i]) {
                            manualPanels[i].transform.GetChild(manualPage[i]).gameObject.SetActive(false);
                            manualPage[i]--;
                            manualPanels[i].transform.GetChild(manualPage[i]).gameObject.SetActive(true);
                        }
                        else {
                            manualPanels[i].transform.GetChild(manualPage[i]).gameObject.SetActive(false);
                            selectState[i] = SelectState.Select;
                        }
                    }
                    break;
                case SelectState.Setting:
                    if (b[i]) { 
                        Selectable[] sel = settingPanel.GetComponentsInChildren<Selectable>();
                        foreach (Selectable s in sel) {
                            s.interactable = false;
                        }
                        settingPanel.GetComponent<Canvas>().sortingOrder = -1;
                        selectState[i] = SelectState.Select;
                    }
                    break;
                case SelectState.Ready:
                    if (b[i]) {
                        selectState[i] = SelectState.Select;
                        readyPanel[i].SetActive(false);
                    }
                    break;
            }
        }

        if (selectState[0] == SelectState.Ready && selectState[1] == SelectState.Ready) {
            readyCount++;
        }
        else if(selectState[0] == SelectState.Select || selectState[1] == SelectState.Select){
            readyCount = 0;
        }

        sumiCircle1.fillAmount = (float)readyCount / 60;
        sumiCircle2.fillAmount = (float)readyCount / 60;

        if(60 <= readyCount && Main.gameState == Main.GameState.Select) { //シーン遷移
            selectState[0] = SelectState.Loading;
            selectState[1] = SelectState.Loading;
            Main.Instance.isMultiDisplays = isMultiDisplays;
            Main.Instance.isDynamicCamera = isDynamicCamera;
            Main.Instance.isVisibleBox = isVisibleBox;
            Main.Instance.isVisibleUI = isVisibleUI;
            Main.gameState = Main.GameState.Battle;
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
        isReleseAxis[0] = true;
    }
    void MoveEnd2() {
        isReleseAxis[1] = true;
    }
    void ChangePlayerType(int num) {
        switch (Main.Instance.playerType[num]) {
            case Main.PlayerType.None: break;
            case Main.PlayerType.Player: 
                Main.Instance.playerType[num] = Main.PlayerType.AI;
                FramePRTf[num].gameObject.GetComponent<Image>().material = gray;
                readyPanel[num].gameObject.GetComponent<Image>().material = gray;
                break;
            case Main.PlayerType.AI:
                Main.Instance.playerType[num] = Main.PlayerType.Player;
                FramePRTf[num].gameObject.GetComponent<Image>().material = num==0?red:blue;
                readyPanel[num].gameObject.GetComponent<Image>().material = num==0?red:blue;
                break;
            default: break;
        }
    }
}
