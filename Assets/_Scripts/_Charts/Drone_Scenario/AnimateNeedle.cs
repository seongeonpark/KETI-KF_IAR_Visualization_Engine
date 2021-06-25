using System;
using System.Collections;
using TMPro;
using UnityEngine;

public enum EChartType
{
    Battery_remain,
    Airspeed,
    HeadingIndicator,
    TurnCoordinator,
    Attitude_roll,
    Attitude_pitch,
    Battery_voltage,
    Altitude
}

public class AnimateNeedle : MonoBehaviour
{
    #region Inspector_Window_Variables
    [SerializeField] private EChartType _ChartType;

    [Header("UI:")]
    [SerializeField] private RectTransform _Needle;
    [SerializeField] private float _NeedleSpeed;
    [Space()]
    [SerializeField] private TextMeshProUGUI _Text;
    [SerializeField] private string _TextUnit;

    [Space()]
    [SerializeField] private float _AngleOfStart;
    [SerializeField] private float _AngleOfEnd;

    [Space()]
    [SerializeField] private float _MinValue;
    [SerializeField] private float _MaxValue;

    

    [Header("Data:")]
    [SerializeField] private GameObject _Http_Parser;
    [SerializeField] private float _RefreshTime;

    [Header("Test:")]
    [SerializeField] private bool _IsTest;
    [SerializeField] private float _TestValue;

    #endregion  // Inspector_Window_Variables


    #region PRIVATE_VARIABLES

    private HTTP_Parser_v01 m_Parser;
    private float m_ServerData = 0f;
    private float m_NeedleValue = 0f;
    private float m_PreNeedleValue = 0f;
    private float m_CurrentNeedleValue = 0f;

    private bool m_IsReady = false;

    #endregion  // PRIVATE_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Awake()
    {
        m_Parser = _Http_Parser.GetComponent<HTTP_Parser_v01>();
    }

    private void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    private void Update()
    {
        if (!m_IsReady)
        {
            m_IsReady = IsConnected(_ChartType);
        }

        // # 3. Animation
        if (m_PreNeedleValue <= m_NeedleValue && m_NeedleValue < m_CurrentNeedleValue)
        {
            m_NeedleValue += _NeedleSpeed * Time.deltaTime;
            _Needle.eulerAngles = new Vector3(0, 0, GetRotation(m_NeedleValue));
        }
        else if (m_CurrentNeedleValue < m_NeedleValue && m_NeedleValue <= m_PreNeedleValue)
        {
            m_NeedleValue -= _NeedleSpeed * Time.deltaTime;
            _Needle.eulerAngles = new Vector3(0, 0, GetRotation(m_NeedleValue));
        }
        else
        {
            m_NeedleValue = m_CurrentNeedleValue;
            _Needle.eulerAngles = new Vector3(0, 0, GetRotation(m_NeedleValue));
        }

    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS

    private float GetRotation(float value)
    {
        float totalAngleSize = Mathf.Abs(_AngleOfStart - _AngleOfEnd);
        float valueNormalized = (value - _MinValue) / (_MaxValue-_MinValue);

        return _AngleOfStart - (valueNormalized * totalAngleSize);
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
            default:
                data = 0;
                break;
        }

        return Convert.ToSingle(data);
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
            if (_IsTest)    // make test value
            {
                //_TestValue = Random.Range(0, 360);
                m_ServerData = _TestValue;
            }
            if (!_IsTest && m_IsReady)
            {
                // # 1. Get data from SERVER
                m_ServerData = GetParserData(_ChartType);
            }

            // # 2. Pre-processing
            // .... config
            m_PreNeedleValue = m_CurrentNeedleValue;
            m_NeedleValue = m_PreNeedleValue;
            // .... normalization
            m_CurrentNeedleValue = Mathf.Clamp(m_ServerData, _MinValue, _MaxValue);
            
            // Text indicator
            if (_Text != null)
            {
                _Text.text = string.Format("{0} {1}", m_CurrentNeedleValue, _TextUnit);
            }

            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    #endregion  // PRIVATE_METHODS
}
