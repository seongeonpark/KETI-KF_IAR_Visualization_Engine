using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLogger : MonoBehaviour
{
    [Range(1, 100)]
    public int FontSize;
    [Range(1, 100)]
    public float Red, Green, Blue;

    private float mDeltaTime = 0.0f;

    private float msec = 0f;
    private float fps = 0f;
    private string text;
    private bool IsCoroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        FontSize = FontSize == 0 ? 50 : FontSize;
        Green = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        mDeltaTime += (Time.unscaledDeltaTime - mDeltaTime) * 0.1f;
    }
    private IEnumerator DrawLog()
    {
        while (true)
        {
            msec = mDeltaTime * 1000.0f;
            fps = 1.0f / mDeltaTime;
            text += DateTime.Now.ToString("HH:mm:ss ");
            text += string.Format("{0:0.0} ms ({1:0.}) fps\n", msec, fps);
            yield return new WaitForSeconds(1f);
        }
    }

    string a = "test";
    Vector2 scrollPosition = Vector2.zero;
    private void OnGUI()
    {


        //int w = Screen.width;
        //int h = Screen.height;

        //GUIStyle style = new GUIStyle();
        //scrollPosition = GUI.BeginScrollView(new Rect(10, 300, 100, 100), scrollPosition, new Rect(0, 0, 220, 200));
        //Rect rect = new Rect(0, 0, w, h * 2 / 100);
        //style.alignment = TextAnchor.UpperRight;
        //style.fontSize = h * 2 / FontSize;
        //style.normal.textColor = new Color(Red, Green, Blue, 1.0f);
        //if (!IsCoroutineRunning)
        //{
        //    IsCoroutineRunning = true;
        //    StartCoroutine("DrawLog");
        //}
        //GUI.Label(rect, text, style);
        //GUI.EndScrollView();

        //GUI.Box(new Rect(Screen.width - 100, 10, 100, 60), "전체 무게변화율");
        //a = (GUI.TextField(new Rect(Screen.width - 80, 40, 60, 20), a.ToString(), 25));
        //if (GUI.Button(new Rect(Screen.width - 80, 80, 60, 20), "적용"))
        //{
        //    var totalPersent = a;
        //    a = "0";
        //}
    }
}
