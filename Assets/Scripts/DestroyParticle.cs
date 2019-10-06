using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour {

    public float lifeTime = 1.0f;

    void Update() {
        lifeTime -= Time.unscaledDeltaTime;
        if (lifeTime <= 0) {
            Destroy(this.gameObject);
        }
    }
}
