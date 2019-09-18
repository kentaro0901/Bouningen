﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour {

    Material material;
    public Material NormalTone;
    public Material redBlack;
    public Material blueBlack;
    public Material reverseTone;
    public Material whiteWhite;
    public Material blackBlack;
    public enum ToneName {
        redBlack,
        blueBlack,
        reverseTone,
        whiteWhite,
        blackBlack
    }
    public static ToneName toneName = ToneName.redBlack;
    float postEffectSeconds = 0.0f;
    float maxVibrateSeconds = 0.0f;
    float vibrateSeconds = 0.0f;
    float vibrateRange = 0.0f;
    float zoomInSeconds = 0.0f;
    float zoomOutSeconds = 0.0f;
    Transform tf;
    Vector3 iniPos;
    Camera _camera;

    private void Start() {
        tf = this.transform;
        iniPos = tf.localPosition;
        material = NormalTone;
        _camera = this.GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, material);
    }
    private void LateUpdate() {
        if(0 != vibrateSeconds) VibrateCountDown();
        if(0 != zoomOutSeconds) ZoomCountDown();
        if(0 != postEffectSeconds) ToneCountDown();
    }

    public void Vibrate(float seconds, float range) {
        maxVibrateSeconds = seconds;
        vibrateSeconds = seconds;
        vibrateRange = range;
    }
    private void VibrateCountDown() {
        if (vibrateSeconds > 0) {
            tf.localPosition = iniPos + vibrateRange * Vector3.up * Mathf.Sin(vibrateSeconds * 50) * Mathf.Pow(vibrateSeconds / maxVibrateSeconds, 4);
            vibrateSeconds -= Time.unscaledDeltaTime;
        }
        else {
            tf.localPosition = iniPos;
        }
    }

    public void ZoomInOut(float seconds) {
        zoomInSeconds = seconds;
        zoomOutSeconds = seconds;
    }
    private void ZoomCountDown() {
        if (zoomInSeconds > 0) {
            tf.localPosition = iniPos + new Vector3(0, -2.0f, 0f);
            _camera.orthographicSize = Main.Instance.cameraSize - 4;
            zoomInSeconds -= Time.unscaledDeltaTime;
        }
        else if (zoomOutSeconds > 0) {
            zoomInSeconds = 0.0f;
            tf.localPosition = iniPos;
            _camera.orthographicSize = Main.Instance.cameraSize + 4;
            zoomOutSeconds -= Time.unscaledDeltaTime;
        }
        else {
            zoomOutSeconds = 0.0f;
            _camera.orthographicSize = Main.Instance.cameraSize;
        }
    }

    public void ChangeTone(float seconds, ToneName name) {
        postEffectSeconds = seconds;
        toneName = name;
        switch (toneName) {
            case ToneName.redBlack: material = redBlack; break;
            case ToneName.blueBlack: material = blueBlack; break;
            case ToneName.reverseTone: material = reverseTone; break;
            case ToneName.whiteWhite: material = whiteWhite; break;
            case ToneName.blackBlack: material = blackBlack; break;
        }
    }
    private void ToneCountDown() {
        if (postEffectSeconds > 0) {        
            postEffectSeconds -= Time.unscaledDeltaTime;
        }
        else {
            material = NormalTone;
            postEffectSeconds = 0.0f;
        }
    }
}
