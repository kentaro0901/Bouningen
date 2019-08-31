using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour {

    float timeScaleSeconds = 0.0f;

    private void Update() {
        if(timeScaleSeconds > 0) {
            timeScaleSeconds -= Time.unscaledDeltaTime;
        }
        else {
            timeScaleSeconds = 0.0f;
            Time.timeScale = 1.0f;
        }
    }

    public void TimeScale(float speed, float seconds) {
        Time.timeScale = speed;
        timeScaleSeconds = seconds;
    }
}
