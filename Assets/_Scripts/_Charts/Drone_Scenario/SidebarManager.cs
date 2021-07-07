using System.Collections;
using TMPro;
using UnityEngine;

public class SidebarManager : MonoBehaviour
{
    #region Inspector_Window_Variables

    [SerializeField] private EDroneChartType _ChartType;

    [Header("GPS:")]
    [SerializeField] private TextMeshProUGUI _Lat;
    [SerializeField] private TextMeshProUGUI _Lon;

    [Header("Accelerometer:")]
    [SerializeField] private TextMeshProUGUI _XAcc;
    [SerializeField] private TextMeshProUGUI _YAcc;
    [SerializeField] private TextMeshProUGUI _ZAcc;

    [Header("Gyroscope:")]
    [SerializeField] private TextMeshProUGUI _XGyr;
    [SerializeField] private TextMeshProUGUI _YGyr;
    [SerializeField] private TextMeshProUGUI _ZGyr;
    
    [Header("Magnetometer:")]
    [SerializeField] private TextMeshProUGUI _XMag;
    [SerializeField] private TextMeshProUGUI _YMag;
    [SerializeField] private TextMeshProUGUI _ZMag;

    [Header("Data:")]
    [SerializeField] private GameObject _Parser;
    [SerializeField] private float _RefreshTime;
    
    [Header("Test:")]
    [SerializeField] private bool _IsTest;
    [SerializeField] private float _TestValue;

    #endregion  // Inspector_Window_Variables

    #region PRIVATE_VARIABLES

    private float m_ServerData = 0f;

    private HTTP_Parser_v01 m_HTTP;
    private MQTT_Parser_v011 m_MQTT;
    private ParserManager m_ParserManager;

    private bool m_IsReady = false;
    private bool m_IsIMUReady = false;
    #endregion  // PRIVATE_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Awake()
    {
        m_HTTP = _Parser.GetComponent<HTTP_Parser_v01>();
        m_MQTT = _Parser.GetComponent<MQTT_Parser_v011>();

        if (m_HTTP)
        {
            m_ParserManager = new ParserManager(m_HTTP);
        }
        else if (m_MQTT)
        {
            m_ParserManager = new ParserManager(m_MQTT);
        }
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
        if (!m_IsIMUReady)
        {
            m_IsIMUReady = m_ParserManager.CheckConnection(EDroneChartType.Accelerometer_x);
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
                _Lat.text = string.Format("{0}", GetGPS(EDroneChartType.Latitude));
                _Lon.text = string.Format("{0}", GetGPS(EDroneChartType.Longitude));
                               
            }

            if (!_IsTest && m_IsIMUReady)
            {
                _XAcc.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Accelerometer_x));
                _YAcc.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Accelerometer_y));
                _ZAcc.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Accelerometer_z));
                                              
                _XGyr.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Gyroscope_x));
                _YGyr.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Gyroscope_y));
                _ZGyr.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Gyroscope_z));
                                              
                _XMag.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Magnetometer_x));
                _YMag.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Magnetometer_y));
                _ZMag.text = string.Format("{0,3}", m_ParserManager.GetParsingDataOf(EDroneChartType.Magnetometer_z));
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
