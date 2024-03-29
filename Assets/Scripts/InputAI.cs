﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAI : InputMethod {

    string filename = "AISword";
    const int dataNum = 12;
    const int xDataNum = 30;
    const int yDataNum = 11;
    const int stateNum = 14;

    public class InputValue {
        public string name;
        public float[] deltaX;
        public float[] deltaY;
        public float[] myState;
        public float[] enState;
        public float axisX = 0.0f;
        public float axisY = 0.0f;
        public int a = 0;
        public int b = 0;
        public int r = 0;
        public int l = 0;
        public InputValue() {
            name = "";
            deltaX = new float[xDataNum];
            deltaY = new float[yDataNum];
            myState = new float[stateNum];
            enState = new float[stateNum];
        }
        public InputValue(string s, float[] x, float[] y, float[] m, float[] e) {
            name = s;
            deltaX = x;
            deltaY = y;
            myState = m;
            enState = e;
        }
        public float CalcValue(int dx, int dy, int my, int en) {
            return deltaX[dx] * deltaY[dy] * myState[my] * enState[en];
        }
    }
    public InputValue[] inputValues;

    void Start() {
        filename = controller.AIFileName;
        inputValues = new InputValue[dataNum];
        for (int i= 0; i < dataNum; i++) {
            inputValues[i] = new InputValue();
        }
        LoadCSV();
    }
    void Update() {
        AIDesition();
    }

    void AIDesition() {
        int desition = 0;
        Vector2 dv = controller.enemyTf.position - controller.playerTf.position;
        int dx = Mathf.Min(Mathf.Abs((int)dv.x), xDataNum-1);
        int dy = ((int)Mathf.Abs(dv.y) < (int)Mathf.Floor(yDataNum / 2) ? (int)dv.y: (dv.y<0 ? -1: 1) * (int)Mathf.Floor(yDataNum / 2)) + (int)Mathf.Floor(yDataNum/2);
        int my = controller.character.prestatenum;
        int en = controller.enemyController.character.prestatenum;

        float bar = Random.Range(0.0f, 1.0f);
        float t = 0.0f;
        float totalValue = 0.0f;
        for (int i = 0; i < dataNum; i++) {
            totalValue += inputValues[i].CalcValue(dx, dy, my, en);
        }
        for(int i = 0; i < dataNum; i++) {
            if (t <= bar && bar < t + inputValues[i].CalcValue(dx,dy,my,en) / totalValue) {
                desition = i;
                break;
            }
            else {
                t += inputValues[i].CalcValue(dx,dy,my,en) / totalValue;
            }
        }

        xAxis = inputValues[desition].axisX * (controller.playerTf.position.x < controller.enemyTf.position.x ? 1: -1);
        yAxis = inputValues[desition].axisY;
        A =  0 < inputValues[desition].a;
        B = 0 < inputValues[desition].b;
        R = 0 < inputValues[desition].r;
        L = 0 < inputValues[desition].l;
    }
    void LoadCSV() { //CSVの読み込み
        int i = 0;
        TextAsset csv = Resources.Load(filename) as TextAsset;
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1) {
            string line = reader.ReadLine(); //1行
            string[] values = line.Split(',');
            if (2 <= i) {
                inputValues[i-2].name = values[0];
                int t = 1;
                for (int k = 0; k < xDataNum; k++) {
                    inputValues[i-2].deltaX[k] = float.Parse(values[t + k]);
                }
                t += xDataNum;
                for (int k = 0; k < yDataNum; k++) {
                    inputValues[i - 2].deltaY[k] = float.Parse(values[t + k]);
                }
                t += yDataNum;
                for (int k = 0; k < stateNum; k++) {
                    inputValues[i - 2].myState[k] = float.Parse(values[t + k]);
                }
                t += stateNum;
                for (int k = 0; k < stateNum; k++) {
                    inputValues[i - 2].enState[k] = float.Parse(values[t + k]);
                }
                t += stateNum;
                inputValues[i-2].axisX = float.Parse(values[t]);
                inputValues[i-2].axisY = float.Parse(values[t+1]);
                inputValues[i-2].a = int.Parse(values[t+2]);
                inputValues[i-2].b = int.Parse(values[t+3]);
                inputValues[i-2].r = int.Parse(values[t+4]);
                inputValues[i-2].l = int.Parse(values[t+5]);
            }
            i++;
        }
    }
}
