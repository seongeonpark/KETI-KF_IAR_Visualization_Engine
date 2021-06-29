using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SidebarManager : MonoBehaviour
{
    #region Inspector_Window_Variables

    [SerializeField] private EDroneChartType _ChartType;

    [Header("Indicators:")]
    [SerializeField] private TextMeshProUGUI[] _Indicators;

    [Header("Data:")]
    [SerializeField] private GameObject _Http_Parser;
    [SerializeField] private float _RefreshTime;
    
    [Header("Test:")]
    [SerializeField] private bool _IsTest;
    [SerializeField] private float _TestValue;

    #endregion  // Inspector_Window_Variables

    #region PRIVATE_VARIABLES

    private float m_ServerData = 0f;

    private HTTP_Parser_v01 m_Parser;
    private ParserManager m_ParserManager;

    private bool m_IsReady = false;

    #endregion  // PRIVATE_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Awake()
    {
        m_Parser = _Http_Parser.GetComponent<HTTP_Parser_v01>();
        m_ParserManager = new ParserManager(m_Parser);
    }

    private void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    private void Update()
    {
        if (!m_IsReady)
        {
            m_IsReady = m_ParserManager.CheckConnection(_ChartType);
        }
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS


    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            if (_IsTest)    // make test value
            {
                //_TestValue = Random.Range(0, 360);
                m_ServerData = _TestValue;
            }

            if (!_IsTest && m_IsReady)
            {
                // # 1. Get data from SERVER
                _Indicators[0].text = string.Format("{0}", GetGPS(EDroneChartType.Latitude));
                _Indicators[1].text = string.Format("{0}", GetGPS(EDroneChartType.Longitude));
            }

            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    private string GetGPS(EDroneChartType chart)
    {
        var data = m_ParserManager.GetParsingDataOf(chart);
        string text = data.ToString("F0");  // no decimal

        string degree = "";
        string min = "";
        string sec = "";
        string result = "";

        if (chart == EDroneChartType.Latitude)
        {
            degree = text.Substring(0, 2);
            min = text.Substring(3, 2);
            sec = text.Substring(4, 2);
            result = string.Format("{0:N3}˚{1}'{2}\"E", degree, min, sec);
        }
        else if (chart == EDroneChartType.Longitude)
        {
            degree = text.Substring(0, 3);
            min = text.Substring(4, 2);
            sec = text.Substring(5, 2);
            result = string.Format("{0:N3}˚{1}'{2}\"N", degree, min, sec);

        }

        return result;
    }
}
