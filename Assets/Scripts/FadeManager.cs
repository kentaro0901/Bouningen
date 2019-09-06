using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FadeManager : MonoBehaviour {

    //シングルトン（インスタンスが一つしかないことを保証する）
	private static FadeManager instance;
	public static FadeManager Instance {
		get {
			if (instance == null) {
				instance = (FadeManager)FindObjectOfType (typeof(FadeManager));
				if (instance == null) Debug.LogError (typeof(FadeManager) + "is nothing");
			}
			return instance;
		}
	}

	//private float fadeAlpha = 0; //不透明度（0で透明、１で不透明）
	private bool isFading = false;
	//public Color fadeColor = Color.black;

    private float fadeValue = 0;
    RectTransform transLeftD1;
    RectTransform transRightD1;
    RectTransform transLeftD2;
    RectTransform transRightD2;

	public void Awake () {
		if (this != Instance) { //２つ目以降のインスタンスは破棄
			Destroy (this.gameObject);
			return;
		}
		DontDestroyOnLoad (this.gameObject);

        transLeftD1 = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        transRightD1 = this.gameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
        transLeftD1.offsetMax = Vector2.right * -Screen.width;
        transRightD1.offsetMin = Vector2.right * Screen.width;
        transLeftD2 = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();
        transRightD2 = this.gameObject.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
        transLeftD2.offsetMax = Vector2.right * -Screen.width;
        transRightD2.offsetMin = Vector2.right * Screen.width;
    }

	public void LoadScene (string scene, float interval) {
		StartCoroutine (TransScene (scene, interval));
	}

	private IEnumerator TransScene (string scene, float interval) {
		this.isFading = true;
		float time = 0;
		while (time <= interval) {
			this.fadeValue = Mathf.Lerp (0f, 1f, time / interval);
            transLeftD1.offsetMax = Vector2.right * (-Screen.width + Screen.width / 2 * fadeValue);
            transRightD1.offsetMin = Vector2.right *(Screen.width - Screen.width / 2 * fadeValue);
            transLeftD2.offsetMax = Vector2.right * (-Screen.width + Screen.width / 2 * fadeValue);
            transRightD2.offsetMin = Vector2.right * (Screen.width - Screen.width / 2 * fadeValue);
            time += Time.deltaTime;
			yield return 0;
		}

		SceneManager.LoadScene (scene);

		time = 0;
		while (time <= interval) {
			this.fadeValue = Mathf.Lerp (1f, 0f, time / interval);
            transLeftD1.offsetMax = Vector2.right * (-Screen.width + Screen.width / 2 * fadeValue);
            transRightD1.offsetMin = Vector2.right * (Screen.width - Screen.width / 2 * fadeValue);
            transLeftD2.offsetMax = Vector2.right * (-Screen.width + Screen.width / 2 * fadeValue);
            transRightD2.offsetMin = Vector2.right * (Screen.width - Screen.width / 2 * fadeValue);
            time += Time.deltaTime;
			yield return 0;
		}
        transLeftD1.offsetMax = Vector2.right * -Screen.width;
        transRightD1.offsetMin = Vector2.right * Screen.width;
        transLeftD2.offsetMax = Vector2.right * -Screen.width;
        transRightD2.offsetMin = Vector2.right * Screen.width;
        this.isFading = false;
	}
}
