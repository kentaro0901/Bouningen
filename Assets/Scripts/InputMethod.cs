using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputMethod : MonoBehaviour {

    protected PlayerController controller;
    public float xAxis = 0.0f;
    public float yAxis = 0.0f;
    public bool A = false;
    public bool B = false;
    public bool X = false;
    public bool Y = false;
    public bool R = false;
    public bool L = false;

    protected void Awake() {
        controller = gameObject.GetComponent<PlayerController>();
    }
}
