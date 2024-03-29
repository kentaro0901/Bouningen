﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//剣の固有処理
public class Sword : Character {

    public override void NutralB() {
        base.NutralB();
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
            StartCoroutine("NutralBCoroutine");
        }
    }
    public override void NutralB_Air() {
        base.NutralB_Air();
        if (counter == 0) {
            StartCoroutine("NutralBCoroutine");
        }
        playerTf.position += Vector3.down * counter * 0.03f * animator.speed;
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
    }
    IEnumerator NutralBCoroutine() {
        float speed = controller.animator.speed;
        yield return new WaitForSeconds(10.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + (playerTf.localScale.x > 0 ? 2:-2) * Vector3.right, Quaternion.identity, 1.0f);
        yield return new WaitForSeconds(4.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + (playerTf.localScale.x > 0 ? 2 : -2) * Vector3.right, Quaternion.identity, 1.0f);
        yield return new WaitForSeconds(4.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position + (playerTf.localScale.x > 0 ? 2 : -2) * Vector3.right, Quaternion.identity, 1.0f);
        yield return 0;
    }
    public override void SideB() {
        base.SideB();
        if (counter == 0) {
            StartCoroutine("SideBCoroutine");
        }
    }
    public override void SideB_Air() {
        base.SideB_Air();
        if (counter == 0) {
            StartCoroutine("SideBCoroutine");
        }
        playerTf.position += Vector3.down * counter * 0.03f * animator.speed;
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        }
    }
    IEnumerator SideBCoroutine() {
        float speed = controller.animator.speed;
        BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
        yield return new WaitForSeconds(16.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.5f, 0.8f);
        BattleMgr.Instance.CreateVFX("Line", playerTf.position + Vector3.right * (playerTf.localScale.x > 0 ? 5:-5), Quaternion.Euler(0, 0, playerTf.localScale.x > 0? 30 : -30), 1.0f);
        yield return 0;
    }
    public override void DownB() {
        base.DownB();
        if (counter == 0) {
            StartCoroutine("DownBCoroutine");
        }
        if (counter % 4 == 0 && counter <= 8) {
            BattleMgr.Instance.CreateVFX("FallSunder", playerTf.position, Quaternion.identity, 1.0f);
        }
    }
    IEnumerator DownBCoroutine() {
        float speed = controller.animator.speed;
        yield return new WaitForSeconds(15.0f / 60 / speed);
        BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        yield return 0;
    }
    public override void DownB_Air() {
        base.DownB_Air();
    }
    public override void DownB_Air_Fall() {
        base.DownB_Air_Fall();
        if (counter == 0) {
            if (Mathf.Abs(controller.enemyTf.position.x - playerTf.position.x) <= 1.5f) {
                BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
            }
        }
        if (counter % 3 == 0 && counter <= 9) {
            BattleMgr.Instance.CreateVFX("CriticalDownWave", playerTf.position + Vector3.up * 2, Quaternion.identity, 1.0f);
        }
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
            BattleMgr.Instance.CreateVFX("WhiteStone", playerTf.position, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", playerTf.position, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position, Quaternion.identity, 1.0f);
            BattleMgr.Instance.VibrateDouble(1.0f, 1.5f);
        }
    }
    public override void UpB() {
        base.UpB();
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
        }
    }
    public override void UpB_Fall() {
        base.UpB_Fall();
        if (counter == 0) {
            if (controller.enemyController.stateInfo.fullPathHash == AnimState.CriticalUp ||
                controller.enemyController.stateInfo.fullPathHash ==  AnimState.CriticalFall) {
                BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
                BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, Quaternion.identity, 1.0f);
            }
        }
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) { //着地寸前
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        }
    }
    protected override void EndCoroutine() {
        base.EndCoroutine();
        StopCoroutine("NutralBCoroutine");
        StopCoroutine("SideBCoroutine");
        StopCoroutine("DownBCoroutine");
    }
}
