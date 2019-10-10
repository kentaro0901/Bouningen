using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour {

    //シングルトン
    private static TitleMgr instance;
    public static TitleMgr Instance {
        get {
            if (instance == null) {
                instance = (TitleMgr)FindObjectOfType(typeof(TitleMgr));
                if (instance == null)
                    Debug.LogError(typeof(TitleMgr) + "is nothing");
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

    [SerializeField] Animator animator;

    void Start() {
        Main.state = Main.State.Title;
        //Main.CameraSetting();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(0 < animator.speed) {
                animator.speed = 0;
            }
            else {
                animator.speed = 0.8f;
            }
            
        }
        if (Input.GetButtonDown("ButtonA_0") && Main.state == Main.State.Title) {
            Main.state = Main.State.Select;
            FadeManager.Instance.LoadScene("Select", 0.5f);
        }
    }
}
