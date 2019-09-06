using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

    public enum State{
        Root,
        Title,
        Select,
        Battle,
        Result
    }
    public static State state = State.Root;

    static AudioSource bgm; //BGM
    public AudioClip inferno; //とりあえず

    void Awake() {
        DontDestroyOnLoad(this.gameObject);
        bgm = this.GetComponent<AudioSource>();
        bgm.clip = inferno;
    }

    //タイトルへ
    public static void Init() {
        bgm.Stop();
        bgm.Play();
        state = State.Title;
        SceneManager.LoadScene("Title");
    }

    void Update() {

        //F5でタイトルに戻る
        if (Input.GetKeyDown(KeyCode.F5)) {
            Init();
        }

        //escで強制終了
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
