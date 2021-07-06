using System;
using TMPro;
using UnityEngine;

public class Dashboard : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MQTT_Parser_v011 _MQTT;

    [Header("UI:")]
    [SerializeField] private TextMeshProUGUI _DeviceID;
    [SerializeField] private TextMeshProUGUI _DateTimeTMP;
    [SerializeField] private TextMeshProUGUI _RemainBattery;
    [SerializeField] private TextMeshProUGUI _CurrentBattery;
    [SerializeField] private TextMeshProUGUI _LeftTime;
    [SerializeField] private TextMeshProUGUI _Activity;

    #region PRIVATE_VARIABLES

    private TextMeshProUGUI _mTxtTime;
    private TextMeshProUGUI _mTxtMachine;
    private Transform _mRootObj;

    private ParserManager m_ParserManager;

    private bool m_IsReady = false;
    private DroneStatus m_Status;


    #endregion  // PRIVATE_VARIABLES

    //private void Awake()
    //{
    //    _mRootObj = gameObject.transform.parent.parent;
    //    _mTxtTime = gameObject.transform.Find("Time").GetComponent<TextMeshProUGUI>();
    //    _mTxtMachine = gameObject.transform.Find("Machine").GetComponent<TextMeshProUGUI>();

    //}

    private void Start()
    {
        _DeviceID.text = string.Format("{0}", _MQTT.test_bridge);
        //    _mTxtMachine.text = GetObjName(_mRootObj);

        m_ParserManager = new ParserManager(_MQTT);

    }

    private void Update()
    {
        if (!m_IsReady)
        {
            m_IsReady = m_ParserManager.CheckConnection(EDroneChartType.Battery_remain);
        }
        else if (m_IsReady)
        {
            var remainBattery = m_ParserManager.GetParsingDataOf(EDroneChartType.Battery_remain);
            var currentBattery = m_ParserManager.GetParsingDataOf(EDroneChartType.Battery_voltage);

            _RemainBattery.text = string.Format("{0} %", remainBattery);
            _CurrentBattery.text = string.Format("{0}", currentBattery);

            m_Status = CheckDroneStatus();
            string droneActivity = "LANDING";

            if (m_Status == DroneStatus.DISARM)
            {
                droneActivity = "LANDING";
            }
            else if (m_Status == DroneStatus.ARM)
            {
                droneActivity = "FLYING";
            }
            _Activity.text = string.Format("{0}", droneActivity);
        }

        _DateTimeTMP.text = DateTime.Now.ToString("yyyy/MM/dd   HH:mm:ss");
        //    _mTxtTime.text = DateTime.Now.ToString("yyyy/MM/dd   HH:mm:ss");

    }

    public DroneStatus CheckDroneStatus()
    {
        if (!_MQTT._out_arm_or_disarm)
            return DroneStatus.DISARM;
        else
            return DroneStatus.ARM;
    }

    private string GetObjName(Transform tf)
    {
        if (tf.name == "IoT_HarsleY28") return "HarsleY28";
        else if (tf.name == "IoT_HX200") return "HX200";
        else if (tf.name == "IoT_HX200temp") return "HX200_TEMP";
        else return "Machine";
    }

}
