using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//格闘の固有処理
public class Fighter : Character {

    public override void NutralB() {
        base.NutralB();
        if (counter == 0) {
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
        yield return new WaitForSeconds(12.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        yield return 0;
    }
    public override void SideB() {
        base.SideB();
        if (counter == 0) {
            StartCoroutine("SideBCoroutine");
        }
    }
    IEnumerator SideBCoroutine() {
        float speed = controller.animator.speed;
        BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
        yield return new WaitForSeconds(32.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.5f, 0.5f);
        BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, Quaternion.identity, 1.0f);
        yield return new WaitForSeconds(8.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), playerTf.position.y + 1, 0), Quaternion.Euler(0,0,(playerTf.localScale.x>0? 1 : -1)*90), 1.0f);
        BattleMgr.Instance.CreateVFX("CriticalWave", playerTf.position, Quaternion.identity, 1.0f);
        yield return new WaitForSeconds(4.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), playerTf.position.y + 1, 0), Quaternion.Euler(0, 0, (playerTf.localScale.x > 0 ? 1 : -1) * 90), 1.0f);
        yield return new WaitForSeconds(4.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("LandingCrash", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 2 : -2), playerTf.position.y + 1, 0), Quaternion.Euler(0, 0, (playerTf.localScale.x > 0 ? 1 : -1) * 90), 1.0f);
        yield return 0;
    }
    public override void SideB_Air() {
        base.SideB_Air();
        if (counter == 0) {
            StartCoroutine("SideBCoroutine");
        }
        playerTf.position += Vector3.down * 0.03f * animator.speed;
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) {
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
        }
    }
    public override void DownB() {
        base.DownB();
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
        }
    }
    public override void DownB_Fall() {
        base.DownB_Fall();
        if (counter == 0) {
            if (Mathf.Abs(enemyTf.position.x - playerTf.position.x) <= 2.5f && 
                (playerTf.localScale.x>0? enemyTf.position.x - playerTf.position.x > 0: enemyTf.position.x - playerTf.position.x < 0)) {
                BattleMgr.Instance.CreateVFX("XLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.CreateVFX("OLight", playerTf.position + Vector3.up, Quaternion.identity, 1.0f);
                BattleMgr.Instance.ChangeToneDouble(0.35f, CameraEffect.ToneName.reverseTone);
                BattleMgr.Instance.ChangeAnimeSpeedDouble(0.05f, 0.35f);
            }
        }
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.3f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) { //着地寸前 
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x>0?1:-1), 0, 0));
            BattleMgr.Instance.CreateVFX("WhiteStone", playerTf.position + (playerTf.localScale.x > 0 ? 1 : -1) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position + (playerTf.localScale.x > 0 ? 1 : -1) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 1 : -1),0,0) , Quaternion.identity, 1.0f);
        }
    }
    public override void DownB_Air() {
        base.DownB_Air();
        playerTf.position = new Vector3(playerTf.position.x, playerTf.position.y - (counter * 0.3f) * animator.speed, 0);
        if (playerTf.position.y < 0.05f && !animator.GetBool(AnimState.isLand)) { //着地寸前
            playerTf.position = new Vector3(playerTf.position.x, 0, 0);
            BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
            BattleMgr.Instance.CreateHibi(new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 1 : -1), 0, 0));
            BattleMgr.Instance.CreateVFX("WhiteStone", playerTf.position + (playerTf.localScale.x > 0 ? 1 : -1) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position + (playerTf.localScale.x > 0 ? 1 : -1) * Vector3.one, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", new Vector3(playerTf.position.x + (playerTf.localScale.x > 0 ? 1 : -1), 0, 0), Quaternion.identity, 1.0f);
        }
    }
    public override void UpB() {
        base.UpB();
        if (counter == 0) {
            BattleMgr.Instance.VibrateDouble(0.3f, 0.3f);
            StartCoroutine("UpBCoroutine");
        }
    }
    IEnumerator UpBCoroutine() {
        float speed = controller.animator.speed;
        yield return new WaitForSeconds(5.0f / 60 / speed);
        BattleMgr.Instance.CreateVFX("CriticalUpWave", playerTf.position, Quaternion.identity, 1.0f);
        yield return 0;
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
            BattleMgr.Instance.CreateVFX("WhiteStone", playerTf.position, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandingCrash", playerTf.position, Quaternion.identity, 1.0f);
            BattleMgr.Instance.CreateVFX("LandWave", new Vector3(playerTf.position.x, 0, 0), Quaternion.identity, 1.0f);
        }
    }
    protected override void EndCoroutine() {
        base.EndCoroutine();
        StopCoroutine("NutralBCoroutine");
        StopCoroutine("SideBCoroutine");
        StopCoroutine("UpBCoroutine");
    }
}
