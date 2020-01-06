using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Status : ScriptableObject {
    //public GameObject charaPref;
    public float maxhp = 100;
    //public int mp;
    public float dashspeed = 0.9f;
    public float airspeed = 0.25f;
    public float vectorspeed = 3.0f;
    public string AIFileName;
}
