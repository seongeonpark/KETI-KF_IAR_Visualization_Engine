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
    private float m_NeedleValue = 0f;
    private float m_PreNeedleValue = 0f;
    private float m_CurrentNeedleValue = 0f;

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
                _Indicators[0].text = string.Format("{0}", m_ParserManager.GetParsingDataOf(EDroneChartType.Latitude));
                Debug.Log("latitude: " + m_ParserManager.GetParsingDataOf(EDroneChartType.Latitude));
                _Indicators[1].text = string.Format("{0}", m_ParserManager.GetParsingDataOf(EDroneChartType.Longitude));
            }

            yield return new WaitForSeconds(_RefreshTime);
        }
    }
}
