using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//剣の固有処理
public class Sword : Character {

    static public float maxhp = 100.0f;

    public override IEnumerator UpB_Fall() {
        yield return 0;
    }
}
