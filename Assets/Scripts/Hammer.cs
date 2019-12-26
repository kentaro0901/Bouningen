using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ハンマーの固有処理
public class Hammer : Character {

    static public float maxhp = 100.0f;

    public override void NutralB() {
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        }
    }
    public override void NutralB_Air() {
        playerTf.position += Vector3.down * counter * 0.03f * animator.speed;
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.5f, 1.2f);
        }
    }
    public override void SideB() {
        if (counter == 0) {
            StartCoroutine("SideBCoroutine");
        }
    }
    public override void SideB_Air() {
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
        float speed = playerController.animator.speed;
        yield return new WaitForSeconds(13.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
        BattleMgr.Instance.CreateVFX("MSunder", playerTf.position + Vector3.up * 2, Quaternion.Euler(0,0,Random.Range(-90,90)), 1.0f);
        yield return new WaitForSeconds(5.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.3f, 0.5f);
        BattleMgr.Instance.CreateVFX("MSunder", playerTf.position + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-90, 90)), 1.0f);
        yield return new WaitForSeconds(8.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.3f, 0.8f);
        BattleMgr.Instance.CreateVFX("MSunder", playerTf.position + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-90, 90)), 1.0f);
        yield return new WaitForSeconds(6.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.8f, 1.2f);
        BattleMgr.Instance.CreateVFX("MSunder", playerTf.position + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-90, 90)), 1.0f);
        yield return new WaitForSeconds(3.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("MSunder", playerTf.position + Vector3.up * 2, Quaternion.Euler(0, 0, Random.Range(-90, 90)), 1.0f);
        yield return 0;
    }
    public override void DownB() {
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
            BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, Quaternion.identity, 1.0f);
        }
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x > 0? 1.8f :-1.8f), 0, 0));
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0));
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2.2f : -2.2f), 0, 0));
            BattleMgr.Instance.CreateVFX("LandingCrash", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("WhiteStone", new Vector3( playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2),0,0), Quaternion.Euler(0,0,Random.Range(-30,30)),1.0f);
            BattleMgr.Instance.CreateVFX("WhiteStone", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.Euler(0, 0, Random.Range(-30, 30)), 1.0f);
            BattleMgr.Instance.CreateVFX("WhiteStone", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.Euler(0, 0, Random.Range(-30, 30)), 1.0f);
        }
    }
    IEnumerator DownBCoroutine() {
        float speed = playerController.animator.speed;
        yield return new WaitForSeconds(15.0f / 60 / speed);
        BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
        BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        yield return 0;
    }
    public override void DownB_Air() {
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 1.8f : -1.8f), 0, 0));
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0));
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2.2f : -2.2f), 0, 0));
            BattleMgr.Instance.CreateVFX("LandingCrash", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("WhiteStone", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.Euler(0, 0, Random.Range(-30, 30)), 1.0f);
            BattleMgr.Instance.CreateVFX("WhiteStone", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.Euler(0, 0, Random.Range(-30, 30)), 1.0f);
            BattleMgr.Instance.CreateVFX("WhiteStone", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), 0, 0), Quaternion.Euler(0, 0, Random.Range(-30, 30)), 1.0f);
        }
    }
    public override void DownB_Air_Fall() {
        if (counter == 0) {
            if (Mathf.Abs(playerController.enemyTf.position.x - playerTf.position.x) <= 1.5f) {
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
            BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position + (playerTf.localScale.x > 0 ? 3 : -3) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 3 : -3), 0, 0), Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("WhiteStone", playerTf.position + (playerTf.localScale.x > 0 ? 3 : -3) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.VibrateDouble(1.0f, 1.5f);
        }
    }
    public override void UpB() {
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
            BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, Quaternion.identity, 1.0f);
        }
    }
    public override void UpB_Fall() {
        if (counter == 0) {
            if (playerController.enemyController.stateInfo.fullPathHash == AnimState.CriticalUp ||
                playerController.enemyController.stateInfo.fullPathHash ==  AnimState.CriticalFall ||
                playerController.enemyController.stateInfo.fullPathHash == AnimState.CriticalDown) {
                BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
            }
        }
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.2f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) { //着地寸前
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(1.0f, 3.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x, 0, 0));
            BattleMgr.Instance.CreateVFX("WhiteStone", playerTf.position + (playerTf.localScale.x > 0 ? 1 : -1) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position + (playerTf.localScale.x > 0 ? 1 : -1) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 3 : -3), 0, 0), Quaternion.identity, 1.0f);
        }
    }
    protected override void EndCoroutine() {
        base.EndCoroutine();
        StopCoroutine("SideBCoroutine");
        StopCoroutine("DownBCoroutine");
    }
}
