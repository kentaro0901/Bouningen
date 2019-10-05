using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//剣の固有処理
public class Sword : Character {

    static public float maxhp = 100.0f;

    public override IEnumerator UpB_Fall() {
        float speed = playerController.animator.speed;
        yield return new WaitForSeconds(10.0f / 60 / speed);
        BattleMgr.Instance.VibrateDouble(0.8f, 2.0f);
        Instantiate(playerController.HibiPref[Random.Range(0, playerController.HibiPref.Length)], new Vector3(playerController.playerTf.position.x, 0, 0), Quaternion.identity);
        yield return 0;
    }
}
