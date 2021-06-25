using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SidebarManager : MonoBehaviour
{
    [Header("Indicators:")]
    [SerializeField] private TextMeshProUGUI[] _Indicators;

    [Header("Data:")]
    [SerializeField] private GameObject _Http_Parser;
    [SerializeField] private float _RefreshTime;
    
    [Header("Test:")]
    [SerializeField] private bool _IsTest;
    [SerializeField] private float _TestValue;

    private EChartType _ChartType;

    private HTTP_Parser_v01 m_Parser;
    private bool m_IsReady = false;

  

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsReady)
        {
            m_IsReady = IsConnected(_ChartType);
        }
    }


    private bool IsConnected(EChartType chart)
    {
        switch (chart)
        {
            case EChartType.Battery_remain:
                if (0 < m_Parser._out_remainbattery.Count)
                {
                    return true;
                }
                else return false;
            case EChartType.Airspeed:
                if (0 < m_Parser._out_airspeed.Count)
                {
                    return true;
                }
                else return false;
            case EChartType.HeadingIndicator:
                if (0 < m_Parser._out_yaw.Count)
                {
                    return true;
                }
                else return false;
            case EChartType.TurnCoordinator:
                if (0 < m_Parser._out_roll.Count)
                {
                    return true;
                }
                else return false;
            case EChartType.Attitude_roll:
                if (0 < m_Parser._out_roll.Count)
                {
                    return true;
                }
                else return false;
            case EChartType.Attitude_pitch:
                if (0 < m_Parser._out_pitch.Count)
                {
                    return true;
                }
                else return false;
            case EChartType.Battery_voltage:
                if (0 < m_Parser._out_currentbattery.Count)
                {
                    return true;
                }
                else return false;
            default:
                break;
        }
        return false;
    }


    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            //if (_IsTest)    // make test value
            //{
            //    //_TestValue = Random.Range(0, 360);
            //    m_ServerData = _TestValue;
            //}
            //if (!_IsTest && m_IsReady)
            //{
            //    // # 1. Get data from SERVER
            //    m_ServerData = GetParserData(_ChartType);
            //}

            //// # 2. Pre-processing
            //// .... config
            //m_PreNeedleValue = m_CurrentNeedleValue;
            //m_NeedleValue = m_PreNeedleValue;
            //// .... normalization
            //m_CurrentNeedleValue = Mathf.Clamp(m_ServerData, _MinValue, _MaxValue);

            //// Text indicator
            //if (_Text != null)
            //{
            //    _Text.text = string.Format("{0} {1}", m_CurrentNeedleValue, _TextUnit);
            //}

            yield return new WaitForSeconds(_RefreshTime);
        }
    }
}
