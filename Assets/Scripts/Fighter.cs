using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//武の固有処理
public class Fighter : Character {

    static public float maxhp = 120.0f;

    public override IEnumerator UpB_Fall() {
        Debug.Log("UpB_F");
        yield return 0;
    }
}
