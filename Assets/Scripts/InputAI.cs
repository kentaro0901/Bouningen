using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAI : InputMethod {

    string filename = "TestAI02";
    string firstLine = "";
    string secondLine = "";
    const int dataNum = 12;
    public const int xDataNum = 30;
    public const int yDataNum = 11;

    public class InputValue {
        public string name;
        public float[] deltaX;
        public float[] deltaY;
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
        }
        public InputValue(string s, float[] x, float[] y) {
            name = s;
            deltaX = x;
            deltaY = y;
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

        float bar = Random.Range(0.0f, 1.0f);
        float t = 0.0f;
        for(int i = 0; i < dataNum; i++) {
            if (t <= bar && bar < t + (inputValues[i].deltaX[dx] + inputValues[i].deltaY[dy]) / (total.deltaX[dx] + total.deltaY[dy])) {
                desition = i;
                break;
            }
            else {
                t += (inputValues[i].deltaX[dx] + inputValues[i].deltaY[dy]) / (total.deltaX[dx] + total.deltaY[dy]);
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
            if (i == 0) firstLine = line;
            else if (i == 1) secondLine = line;
            else if (2 <= i) {
                inputValues[i-2].name = values[0];
                for (int k = 0; k < xDataNum; k++) {
                    inputValues[i-2].deltaX[k] = float.Parse(values[1 + k]);
                    updateValues[i - 2].deltaX[k] = 0;
                    total.deltaX[k] += float.Parse(values[1 + k]);
                }
                for (int k = 0; k < yDataNum; k++) {
                    inputValues[i - 2].deltaY[k] = float.Parse(values[1 + xDataNum + k]);
                    updateValues[i - 2].deltaY[k] = 0;
                    total.deltaY[k] += float.Parse(values[1 + xDataNum + k]);
                }
                inputValues[i-2].axisX = float.Parse(values[xDataNum+yDataNum+1]);
                inputValues[i-2].axisY = float.Parse(values[xDataNum + yDataNum+2]);
                inputValues[i-2].a = int.Parse(values[xDataNum + yDataNum+3]);
                inputValues[i-2].b = int.Parse(values[xDataNum + yDataNum+4]);
                inputValues[i-2].r = int.Parse(values[xDataNum + yDataNum+5]);
                inputValues[i-2].l = int.Parse(values[xDataNum + yDataNum+6]);
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
