using System.Collections;
using TMPro;
using UnityEngine;

public class VerticalScope : MonoBehaviour
{
    #region Inspector_Window_Variables
    [SerializeField] private EDroneChartType _ChartType;

    [Header("UI:")]
    [SerializeField] private RectTransform _Needle;
    [SerializeField] private float _NeedleSpeed;
    [Space()]
    [SerializeField] private TextMeshProUGUI _Text;
    [SerializeField] private string _TextUnit;
    [Space()]
    [SerializeField] private float _PosOfStart;
    [SerializeField] private float _PosOfEnd;

    [Space()]
    [SerializeField] private float _MinValue;
    [SerializeField] private float _MaxValue;

    [Header("Data:")]
    [SerializeField] private GameObject _Parser;
    [SerializeField] private float _RefreshTime;
    [SerializeField] private float _OutputData;


    [Header("Test:")]
    [SerializeField] private bool _IsTest;
    [SerializeField] private float _TestValue;

    #endregion  // Inspector_Window_Variables


    #region PRIVATE_VARIABLES

    private HTTP_Parser_v01 m_HTTP;
    private MQTT_Parser_v011 m_MQTT;
    private ParserManager m_ParserManager;

    private float m_ServerData = 0f;
    private float m_NeedleValue = 0f;
    private float m_PreNeedleValue = 0f;
    private float m_CurrentNeedleValue = 0f;

    private bool m_IsReady = false;


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
        // #1. Check configuration
        if (!m_IsReady)
        {
            m_IsReady = m_ParserManager.CheckConnection(_ChartType);
        }

        // #4. Animation
        if (m_PreNeedleValue <= m_NeedleValue && m_NeedleValue < m_CurrentNeedleValue)
        {
            m_NeedleValue += _NeedleSpeed * Time.deltaTime;
        }
        else if (m_CurrentNeedleValue < m_NeedleValue && m_NeedleValue <= m_PreNeedleValue)
        {
            m_NeedleValue -= _NeedleSpeed * Time.deltaTime;
        }
        else
        {
            m_NeedleValue = m_CurrentNeedleValue;
        }

        // Needle
        if (_Needle)
        {
            _Needle.localPosition = new Vector3(0, GetPostion(m_NeedleValue), 0);
        }

        // Text indicator
        if (_Text)
        {
            _Text.text = string.Format("{0:N0} {1}", m_NeedleValue, _TextUnit);
        }

        _OutputData = m_ServerData;
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    private float GetPostion(float value)
    {
        float totalScopeSize = Mathf.Abs(_PosOfStart - _PosOfEnd);
        float valueNormalized = (value - _MinValue) / (_MaxValue - _MinValue);

        return _PosOfStart - (valueNormalized * totalScopeSize);
    }

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            // #2. Get data
            if (_IsTest)    // from test value
            {
                //_TestValue = Random.Range(0, 360);
                m_ServerData = _TestValue;
            }
            if (!_IsTest && m_IsReady)  // from SERVER
            {
                m_ServerData = m_ParserManager.GetParsingDataOf(_ChartType);
            }

            // #3. Pre-processing
            // .... initialization
            m_PreNeedleValue = m_CurrentNeedleValue;
            m_NeedleValue = m_PreNeedleValue;
            // .... normalization
            m_CurrentNeedleValue = Mathf.Clamp(m_ServerData, _MinValue, _MaxValue);

            

            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    #endregion  // PRIVATE_METHODS
}
