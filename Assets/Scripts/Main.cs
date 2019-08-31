using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    enum State{
        Init,
        Title,
        Select,
        Battle,
        Result
    }
    State state;
    AudioSource ad;

    void Start() {
        state = State.Init;

        //マルチディスプレイ始動
        if (Display.displays.Length > 1) {
            Display.displays[1].Activate();
        }
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);

        ad = this.GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);

        Init();
    }
    void Init() {
        SceneManager.LoadScene("Title");
        state = State.Title;
        if (!ad.isPlaying) {
            ad.Play();
        }
    }

    void Update() {
        switch (state) {
            case State.Init: break;
            case State.Title:
                if (Input.GetButtonDown("ButtonA_0")) {
                    SceneManager.LoadScene("Select");
                    state = State.Select;
                }
                break;
            case State.Select:
                if(Input.GetButtonDown("ButtonA_0")) {
                    SceneManager.LoadScene("Battle");
                    state = State.Battle;
                }
                break;
            case State.Battle:
                if(Input.GetButtonDown("ButtonY_0")) {
                    SceneManager.LoadScene("Result");
                    state = State.Result;
                }
                break;
            case State.Result:
                if(Input.GetButtonDown("ButtonA_0")) {
                    Init();
                }
                break;
            default: break;
        }

        //escで強制終了
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
