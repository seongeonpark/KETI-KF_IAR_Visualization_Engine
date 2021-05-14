using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class LogWindow : MonoBehaviour
{
    private TextMeshProUGUI m_TMP_LogText;
    private ScrollRect m_Scroll_Rect = null;

    private float deltaTime = 0.0f;
    private float msec = 0f;
    private float fps = 0f;
    private string text;

    void Start()
    {
        m_TMP_LogText = GameObject.Find("log_Text_TMP").GetComponent<TextMeshProUGUI>();
        m_Scroll_Rect = GameObject.Find("Scroll_View").GetComponent<ScrollRect>();

        text += "Start \'KF-IndustrialAR\' App" + "\n\n";

        StartCoroutine("WriteLog");
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private IEnumerator WriteLog()
    {
        while (true)
        {
            DateTime today = DateTime.Now;
            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            
            text += string.Format("{0:T}   -   {1:0.} fps ({2:0.0} ms)\n", today, fps, msec);
            
            m_TMP_LogText.text = text;

            m_Scroll_Rect.verticalNormalizedPosition = 0f;
            yield return new WaitForSeconds(1f);
        }
    }
}