using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAI : MonoBehaviour {

    PlayerController controller;
    public float AxisX = 0.0f;
    public float AxisY = 0.0f;
    public bool A = false;
    public bool B = false;
    public bool X = false;//
    public bool Y = false;//
    public bool R = false;
    public bool L = false;

    string filename = "TestAI";

    class InputValue {
        public string name = "";
        public float[] deltaX = new float[5];
        public float[] deltaY = new float[5];
        public float axisX = 0.0f;
        public float axisY = 0.0f;
        public int a = 0;
        public int b = 0;
        public int r = 0;
        public int l = 0;
        public InputValue(string s, float[] x, float[] y) {
            name = s;
            deltaX = x;
            deltaY = y;
        }
    }
    const int num = 3;
    InputValue[] inputValues = new InputValue[num]; //0はIdleがよさそう
    InputValue total = new InputValue("Total", new float[5] { 0, 0, 0, 0, 0 }, new float[5] { 0, 0, 0, 0, 0 });

    void Awake() {
        controller = gameObject.GetComponent<PlayerController>();
        for (int i= 0; i < num; i++) {
            inputValues[i] = new InputValue("", new float[5] { 0,0,0,0,0}, new float[5] { 0,0,0,0,0});
        }
        LoadCSV();
        //UpdateCSV();
    }

    void Update() {
        AIDesition();
    }

    void AIDesition() {
        int desition = 0;
        Vector2 dv = controller.enemyTf.position - controller.playerTf.position;
        int dx;
        int dy;
        if (Mathf.Abs(dv.x) < 3) dx = 0;
        else if (Mathf.Abs(dv.x) < 5) dx = 1;
        else if (Mathf.Abs(dv.x) < 10) dx = 2;
        else if (Mathf.Abs(dv.x) < 15) dx = 3;
        else dx = 4;
        if (dv.y < -3) dy = 0;
        else if (dv.y < -1) dy = 1;
        else if (dv.y < 1) dy = 2;
        else if (dv.y < 3) dy = 3;
        else dy = 4;

        float bar = Random.Range(0.0f, 1.0f);
        float t = 0.0f;
        for(int i = 0; i < num; i++) {
            if (t <= bar && bar < t + (inputValues[i].deltaX[dx] + inputValues[i].deltaY[dy]) / (total.deltaX[dx] + total.deltaY[dy])) {
                desition = i;
                break;
            }
            else {
                t += (inputValues[i].deltaX[dx] + inputValues[i].deltaY[dy]) / (total.deltaX[dx] + total.deltaY[dy]);
            }
        }

        AxisX = inputValues[desition].axisX * (controller.playerTf.position.x < controller.enemyTf.position.x ? 1: -1);
        AxisY = inputValues[desition].axisY;
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
                for (int k = 0; k <= 4; k++) {
                    inputValues[i-2].deltaX[k] = float.Parse(values[1 + k]);
                    inputValues[i-2].deltaY[k] = float.Parse(values[6 + k]);
                    total.deltaX[k] += float.Parse(values[1 + k]);
                    total.deltaY[k] += float.Parse(values[6 + k]);
                }
                inputValues[i-2].axisX = float.Parse(values[11]);
                inputValues[i-2].axisY = float.Parse(values[12]);
                inputValues[i-2].a = int.Parse(values[13]);
                inputValues[i-2].b = int.Parse(values[14]);
                inputValues[i-2].r = int.Parse(values[15]);
                inputValues[i-2].l = int.Parse(values[16]);
            }
            i++;
        }
    }
    public void UpdateCSV() { //CSVの更新
        StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/" + filename + ".csv", false, System.Text.Encoding.GetEncoding("shift_jis"));
        streamWriter.WriteLine("1行書き込み");
        streamWriter.Close();
    }
}
