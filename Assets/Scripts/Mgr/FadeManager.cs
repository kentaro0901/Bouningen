using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FadeManager : MonoBehaviour {

    //シングルトン
    private static FadeManager instance;
    public static FadeManager Instance {
        get {
            if (instance == null) {
                instance = (FadeManager)FindObjectOfType(typeof(FadeManager));
                if (instance == null) Debug.LogError(typeof(FadeManager) + "is nothing");
            }
            return instance;
        }
    }

    public bool isFading = false;

    [SerializeField] GameObject c1TransCircle1;
    [SerializeField] GameObject c1TransCircle2;
    [SerializeField] GameObject c2TransCircle1;
    [SerializeField] GameObject c2TransCircle2;

	public void Awake () {
		if (this != Instance) {
			Destroy (this.gameObject);
			return;
		}
		DontDestroyOnLoad (this.gameObject);
    }

	public void LoadScene (string scene, float interval) {
		StartCoroutine (TransScene (scene, interval));
	}

	private IEnumerator TransScene (string scene, float interval) {
		this.isFading = true;
        c1TransCircle1.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        c1TransCircle2.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
        c2TransCircle1.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        c2TransCircle2.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
        yield return 0;
        iTween.RotateTo(c1TransCircle1, iTween.Hash("z", -60, "time", 0.3f, "EaseType", iTween.EaseType.easeOutCirc, "islocal", true));
        iTween.RotateTo(c1TransCircle2, iTween.Hash("z", 98, "time", 0.3f, "EaseType", iTween.EaseType.easeOutCirc, "islocal", true));
        iTween.RotateTo(c2TransCircle1, iTween.Hash("z", -60, "time", 0.3f, "EaseType", iTween.EaseType.easeOutCirc, "islocal", true));
        iTween.RotateTo(c2TransCircle2, iTween.Hash("z", 98, "time", 0.3f, "EaseType", iTween.EaseType.easeOutCirc, "islocal", true));
        yield return new WaitForSeconds(0.4f);
        iTween.RotateAdd(c1TransCircle1, iTween.Hash("z", -45, "time", interval /2, "EaseType", iTween.EaseType.easeInBack, "islocal", true));
        iTween.RotateAdd(c1TransCircle2, iTween.Hash("z", -45, "time", interval /2, "EaseType", iTween.EaseType.easeInBack, "islocal", true));
        iTween.RotateAdd(c2TransCircle1, iTween.Hash("z", -45, "time", interval / 2, "EaseType", iTween.EaseType.easeInBack, "islocal", true));
        iTween.RotateAdd(c2TransCircle2, iTween.Hash("z", -45, "time", interval / 2, "EaseType", iTween.EaseType.easeInBack, "islocal", true));
        yield return new WaitForSeconds(interval * 0.55f);
		SceneManager.LoadScene (scene);
        c1TransCircle1.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, -10);
        c1TransCircle2.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 148);
        c2TransCircle1.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, -10);
        c2TransCircle2.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 148);
        yield return 0;
        iTween.RotateAdd(c1TransCircle1, iTween.Hash("z", -50, "time", interval /2, "EaseType", iTween.EaseType.easeOutBack, "islocal", true));
        iTween.RotateAdd(c1TransCircle2, iTween.Hash("z", -50, "time", interval /2, "EaseType", iTween.EaseType.easeOutBack, "islocal", true));
        iTween.RotateAdd(c2TransCircle1, iTween.Hash("z", -50, "time", interval / 2, "EaseType", iTween.EaseType.easeOutBack, "islocal", true));
        iTween.RotateAdd(c2TransCircle2, iTween.Hash("z", -50, "time", interval / 2, "EaseType", iTween.EaseType.easeOutBack, "islocal", true));
        yield return new WaitForSeconds(interval * 0.55f);
        iTween.RotateTo(c1TransCircle1, iTween.Hash("z", 0, "time", 0.3f, "EaseType", iTween.EaseType.easeInCirc, "islocal", true));
        iTween.RotateTo(c1TransCircle2, iTween.Hash("z", 90, "time", 0.3f, "EaseType", iTween.EaseType.easeInCirc, "islocal", true));
        iTween.RotateTo(c2TransCircle1, iTween.Hash("z", 0, "time", 0.3f, "EaseType", iTween.EaseType.easeInCirc, "islocal", true));
        iTween.RotateTo(c2TransCircle2, iTween.Hash("z", 90, "time", 0.3f, "EaseType", iTween.EaseType.easeInCirc, "islocal", true));
        yield return new WaitForSeconds(0.4f);
        c1TransCircle1.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        c1TransCircle2.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
        c2TransCircle1.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        c2TransCircle2.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
        this.isFading = false;
        yield return 0;
	}
}
