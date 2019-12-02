using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAI : InputMethod {

    string filename = "AILv05";
    string firstLine = "";
    string secondLine = "";
    const int dataNum = 12;
    public const int xDataNum = 30;
    public const int yDataNum = 11;
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
        public float Sum(int dx, int dy, int my, int en) {
            return deltaX[dx] + deltaY[dy] + myState[my] + enState[en];
        }
    }
    public InputValue[] inputValues;
    public InputValue[] updateValues;
    public InputValue total;

    void Start() {
        filename = controller.AIFileName;
        inputValues = new InputValue[dataNum];
        updateValues = new InputValue[dataNum];
        total = new InputValue();
        for (int i= 0; i < dataNum; i++) {
            inputValues[i] = new InputValue();
            updateValues[i] = new InputValue();
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
        for(int i = 0; i < dataNum; i++) {
            if (t <= bar && bar < t + inputValues[i].Sum(dx,dy,my,en) / total.Sum(dx,dy,my,en)) {
                desition = i;
                break;
            }
            else {
                t += inputValues[i].Sum(dx,dy,my,en) / total.Sum(dx,dy,my,en);
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
        Debug.Log(filename);
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1) {
            string line = reader.ReadLine(); //1行
            string[] values = line.Split(',');
            if (i == 0) firstLine = line;
            else if (i == 1) secondLine = line;
            else if (2 <= i) {
                inputValues[i-2].name = values[0];
                int t = 1;
                for (int k = 0; k < xDataNum; k++) {
                    inputValues[i-2].deltaX[k] = float.Parse(values[t + k]);
                    updateValues[i - 2].deltaX[k] = 0;
                    total.deltaX[k] += float.Parse(values[t + k]);
                }
                t += xDataNum;
                for (int k = 0; k < yDataNum; k++) {
                    inputValues[i - 2].deltaY[k] = float.Parse(values[t + k]);
                    updateValues[i - 2].deltaY[k] = 0;
                    total.deltaY[k] += float.Parse(values[t + k]);
                }
                t += yDataNum;
                for (int k = 0; k < stateNum; k++) {
                    inputValues[i - 2].myState[k] = float.Parse(values[t + k]);
                    updateValues[i - 2].myState[k] = 0;
                    total.myState[k] += float.Parse(values[t + k]);
                }
                t += stateNum;
                for (int k = 0; k < stateNum; k++) {
                    inputValues[i - 2].enState[k] = float.Parse(values[t + k]);
                    updateValues[i - 2].enState[k] = 0;
                    total.enState[k] += float.Parse(values[t + k]);
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
    public void UpdateCSV(InputValue[] _updateValues) { //CSVの更新
        StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/" + filename + ".csv", false, System.Text.Encoding.GetEncoding("shift_jis"));//
        streamWriter.WriteLine(firstLine);
        streamWriter.WriteLine(secondLine);
        for (int i= 0; i < dataNum; i++) {
            streamWriter.Write(inputValues[i].name + ",");
            for (int j = 0; j < xDataNum; j++) {
                streamWriter.Write(inputValues[i].deltaX[j] + _updateValues[i].deltaX[j] + ",");
            }
            for (int j = 0; j < yDataNum; j++) {
                streamWriter.Write(inputValues[i].deltaY[j] + _updateValues[i].deltaY[j] + ",");
            }
            for (int j = 0; j < stateNum; j++) {
                streamWriter.Write(inputValues[i].myState[j] + _updateValues[i].myState[j] + ",");
            }
            for (int j = 0; j < stateNum; j++) {
                streamWriter.Write(inputValues[i].enState[j] + _updateValues[i].enState[j] + ",");
            }
            streamWriter.Write(inputValues[i].axisX + ",");
            streamWriter.Write(inputValues[i].axisY + ",");
            streamWriter.Write(inputValues[i].a + ",");
            streamWriter.Write(inputValues[i].b + ",");
            streamWriter.Write(inputValues[i].r + ",");
            streamWriter.Write(inputValues[i].l);
            streamWriter.WriteLine("");
        }
        streamWriter.Close();
    }
    void InitCSV() { //学習データを初期化
        StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/" + filename + ".csv", false, System.Text.Encoding.GetEncoding("shift_jis"));
        streamWriter.WriteLine(firstLine);
        streamWriter.WriteLine(secondLine);
        for (int i = 0; i < dataNum; i++) {
            streamWriter.Write(inputValues[i].name + ",");
            for (int j = 0; j < xDataNum; j++) {
                streamWriter.Write("1,");
            }
            for (int j = 0; j < yDataNum; j++) {
                streamWriter.Write("1,");
            }
            for (int j = 0; j < stateNum; j++) {
                streamWriter.Write("1,");
            }
            for (int j = 0; j < stateNum; j++) {
                streamWriter.Write("1,");
            }
            streamWriter.Write(inputValues[i].axisX + ",");
            streamWriter.Write(inputValues[i].axisY + ",");
            streamWriter.Write(inputValues[i].a + ",");
            streamWriter.Write(inputValues[i].b + ",");
            streamWriter.Write(inputValues[i].r + ",");
            streamWriter.Write(inputValues[i].l);
            streamWriter.WriteLine("");
        }
        streamWriter.Close();
    }
}
