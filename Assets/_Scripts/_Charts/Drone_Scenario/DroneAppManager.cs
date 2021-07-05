using UnityEngine;

public enum DroneStatus
{
    DISARM,
    ARM
}

public class DroneAppManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MQTT_Parser_v011 _MQTT;
    [SerializeField] private GameObject m_ARDashboard;
    [SerializeField] private GameObject m_2DDashboard;
    [Header("Test")]
    [SerializeField] private DroneStatus _TestFlag;



    private DroneStatus m_Status;
    
    private ParserManager m_ParserManager;
    private bool m_IsReady = false;

    private bool m_ChangedStatus = false;

    private void Awake()
    {
        m_ParserManager = new ParserManager(_MQTT);
    }

    void Update()
    {

        // #1. Check configuration
        if (!m_IsReady)
        {
            m_IsReady = m_ParserManager.CheckConnection(EDroneChartType.Battery_remain);
        }
        else if (m_IsReady)
        {
            // 실증..
            m_Status = CheckDroneStatus();

            // 테스트..
            //m_Status = _TestFlag;

            if (m_Status == DroneStatus.DISARM)
            {
                m_ARDashboard.GetComponent<Canvas>().enabled = true;
                m_2DDashboard.GetComponent<Canvas>().enabled = false;

            }
            else if (m_Status == DroneStatus.ARM)
            {
                m_ARDashboard.GetComponent<Canvas>().enabled = false;
                m_2DDashboard.GetComponent<Canvas>().enabled = true;

            }
        }
    }

    public DroneStatus CheckDroneStatus()
    {
        if (!_MQTT._out_arm_or_disarm)
            return DroneStatus.DISARM;
        else
            return DroneStatus.ARM;
    }
}
