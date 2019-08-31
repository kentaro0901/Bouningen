using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour {

    Material material;
    public Material NormalTone;
    public Material redBlack;
    public Material reverseTone;
    public enum ToneName {
        redBlack,
        reverseTone
    }
    ToneName toneName = ToneName.redBlack;
    float postEffectSeconds = 0.0f;
    float maxVibrateSeconds = 0.0f;
    float vibrateSeconds = 0.0f;
    float vibrateRange = 0.0f;
    float zoomInSeconds = 0.0f;
    float zoomOutSeconds = 0.0f;
    Transform tf;
    Vector3 iniPos;

    private void Start() {
        tf = this.transform;
        iniPos = tf.localPosition;
        material = NormalTone;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, material);
    }
    private void Update() {
        if(vibrateSeconds > 0) {
            tf.localPosition = iniPos + vibrateRange * Vector3.up * Mathf.Sin(vibrateSeconds * 50) * Mathf.Pow(vibrateSeconds/maxVibrateSeconds, 4);
            vibrateSeconds -= Time.unscaledDeltaTime;
        }
        else {
            tf.localPosition = iniPos;
        }

        if(zoomInSeconds > 0) {
            tf.localPosition = iniPos + new Vector3(0, -2.0f, 8.0f);
            zoomInSeconds -= Time.unscaledDeltaTime;
        }
        else if(zoomOutSeconds > 0){
            zoomInSeconds = 0.0f;
            tf.localPosition = iniPos + new Vector3(0, -2.0f, -10.0f);
            zoomOutSeconds -= Time.unscaledDeltaTime;
        }
        else {
            zoomOutSeconds = 0.0f;
        }

        if(postEffectSeconds > 0) {
            switch (toneName) {
                case ToneName.redBlack: material = redBlack; break;
                case ToneName.reverseTone: material = reverseTone; break;
            }
            postEffectSeconds -= Time.unscaledDeltaTime;
        }
        else {
            material = NormalTone;
            postEffectSeconds = 0.0f;
        }
    }

    public void Vibrate(float seconds, float range) {
        maxVibrateSeconds = seconds;
        vibrateSeconds = seconds;
        vibrateRange = range;
    }

    public void ZoomInOut(float seconds) {
        zoomInSeconds = seconds;
        zoomOutSeconds = seconds;
    }

    public void ChangeTone(float seconds, ToneName tname) {
        postEffectSeconds = seconds;
        toneName = tname;
    }

}
