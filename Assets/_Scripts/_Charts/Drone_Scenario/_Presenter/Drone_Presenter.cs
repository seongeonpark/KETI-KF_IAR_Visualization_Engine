using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IARDrone
{
    // Presenter for scene(canvas) root
    public class Drone_Presenter : MonoBehaviour
    {
        #region Inspector_Window_Variables

        [Header("Data Model:")]
        [SerializeField] private GameObject _Parser;
        [SerializeField] private float _RefreshTime = 0.3f;
        [Header("Drone Chart:")]
        [SerializeField] private Drone_Chart _Chart;
        [Header("Chart Test:")]
        public bool _IsTest = false;
        public float _TestValue;

        #endregion  // Inspector_Window_Variables

        #region PRIVATE_VARIABLES

        private HTTP_Parser_v01 m_HTTP;
        private MQTT_Parser_v011 m_MQTT;
        private Drone_Model m_DataModel;

        private bool m_IsReady = false;

        private float m_ServerData = 0f;

        #endregion  // PRIVATE_VARIABLES


        private void Awake()
        {
            m_HTTP = _Parser.GetComponent<HTTP_Parser_v01>();
            m_MQTT = _Parser.GetComponent<MQTT_Parser_v011>();
            
            if (m_HTTP)
            {
                m_DataModel = new Drone_Model(m_HTTP);
            }
            else if (m_MQTT)
            {
                m_DataModel = new Drone_Model(m_MQTT);
            }
        }

        private void Start()
        {
            StartCoroutine("UpdateData");
        }

        private void Update()
        {
            // #1. Check configuration
            if (!m_IsReady)
            {
                m_IsReady = m_DataModel.IsConnected(_Chart._IoTType);
            }
        }

        private IEnumerator UpdateData()
        {
            while (true)
            {
                if (_IsTest)
                {
                    // send test data to chart
                    _Chart.IoTData = _TestValue;
                }

                // from SERVER
                if (m_IsReady && !_IsTest)  
                {
                    // get data
                    m_ServerData = m_DataModel.GetParsedDataOf(_Chart._IoTType);
                    // set data
                    _Chart.IoTData = m_ServerData;
                }

                yield return new WaitForSeconds(_RefreshTime);
            }
        }
    }
}
