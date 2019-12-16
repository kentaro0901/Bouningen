using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//剣の固有処理
public class Fighter : Character {

    static public float maxhp = 100.0f;

    public override void NutralB() {
        if (counter == 0) {
            Teach(4);
        }
    }
    public override void NutralB_Air() {
        if (counter == 0) {
            Teach(4);
        }
    }
    public override void SideB() {
        if (counter == 0) {
            Teach(6);
        }
        if (counter == 13)
            BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
    }
    public override void SideB_Air() {
        if (counter == 0) {
            Teach(6);
        }
        playerTf.position += Vector3.down * 0.03f * animator.speed;
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            //BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            //BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        }
    }
    public override void DownB() {
        if (counter == 0) {
            Teach(8);
        }
    }
    public override void DownB_Fall() {
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) { //着地寸前
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        }
    }
    public override void DownB_Air() {
        if (counter == 0) {
            Teach(8);
        }
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) { //着地寸前
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        }
    }
    public override void UpB() {
        if (counter == 0) {
            Teach(10);
        }
    }
    public override void UpB_Fall() {
        if (counter == 0) {
            if (playerController.enemyController.stateInfo.fullPathHash == AnimState.CriticalUp ||
                playerController.enemyController.stateInfo.fullPathHash ==  AnimState.CriticalFall) {
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
}
