using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SidebarManager : MonoBehaviour
{
    [SerializeField] private EChartType _ChartType;


    [Header("Indicators:")]
    [SerializeField] private TextMeshProUGUI[] _Indicators;

    [Header("Data:")]
    [SerializeField] private GameObject _Http_Parser;
    [SerializeField] private float _RefreshTime;
    
    [Header("Test:")]
    [SerializeField] private bool _IsTest;
    [SerializeField] private float _TestValue;


    private float m_ServerData = 0f;
    private float m_NeedleValue = 0f;
    private float m_PreNeedleValue = 0f;
    private float m_CurrentNeedleValue = 0f;


    

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
            case EChartType.Latitude:
                if (0 < m_Parser._out_lat.Count)
                {
                    return true;
                }
                else return false;
            default:
                break;
        }
        return false;
    }
    private float GetParserData(EChartType chart)
    {
        float data;

        switch (chart)
        {
            case EChartType.Battery_remain:
                data = m_Parser._out_remainbattery[0];
                break;
            case EChartType.Airspeed:
                data = m_Parser._out_airspeed[0];
                break;
            case EChartType.HeadingIndicator:
                data = m_Parser._out_yaw[0];
                break;
            case EChartType.TurnCoordinator:
                data = m_Parser._out_roll[0];
                break;
            case EChartType.Attitude_roll:
                data = m_Parser._out_roll[0];
                break;
            case EChartType.Attitude_pitch:
                data = m_Parser._out_pitch[0];
                break;
            case EChartType.Battery_voltage:
                data = m_Parser._out_currentbattery[0];
                break;
            case EChartType.Latitude:
                data = m_Parser._out_lat[0];
                break;
            case EChartType.Longitude:
                data = m_Parser._out_lon[0];
                break;
            default:
                data = 0;
                break;
        }

        return Convert.ToSingle(data);
    }

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
                _Indicators[0].text = string.Format("{0}", GetParserData(EChartType.Latitude));
                Debug.Log("latitude: " + GetParserData(EChartType.Latitude));
                _Indicators[1].text = string.Format("{0}", GetParserData(EChartType.Longitude));
            }

            yield return new WaitForSeconds(_RefreshTime);
        }
    }
}
