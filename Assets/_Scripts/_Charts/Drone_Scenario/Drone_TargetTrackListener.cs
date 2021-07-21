using UnityEngine;

namespace Vuforia
{
    public class Drone_TargetTrackListener : MonoBehaviour
    {
        public bool visiToggle;

        [Header("IoT Object")]
        [SerializeField] private Drone_TargetTracker TargetObject;

        [Header("Configuration")]
        [SerializeField] private MQTT_Parser_v011 _MQTT;
        [SerializeField] private Canvas m_ARDashboard;
        [SerializeField] private Canvas m_2DDashboard;

        private Drone_Model m_DataModel;
        private bool m_IsReady = false;


        private void Awake()
        {
            m_DataModel = new Drone_Model(_MQTT);
        }

        void Start()
        {
            TargetObject.OnTrackingFound += trackingFound;
            TargetObject.OnTrackingLost += trackingLost;
        }

        private void Update()
        {
            // #1. Check configuration
            if (!m_IsReady)
            {
                m_IsReady = m_DataModel.IsConnected(EDroneIoT.Battery_remain);
            }
        }

        void onDestroy()
        {
            TargetObject.OnTrackingFound -= trackingFound;
            TargetObject.OnTrackingLost -= trackingLost;
        }


        public void trackingFound()
        {
            if (visiToggle)
            {
                //gameObject.SetActive(false);
                //visiToggle = false;
            }
            else
            {
                if (m_ARDashboard != null)
                {
                    m_ARDashboard.enabled = true;
                }
                if (m_2DDashboard != null)
                {
                    m_2DDashboard.enabled = false;
                }

                //gameObject.SetActive(true);
                visiToggle = true;
            }
        }

        public void trackingLost()
        {
            if (visiToggle)
            {
                //gameObject.SetActive(false);
                if (m_ARDashboard != null)
                {
                    m_ARDashboard.enabled = false;
                }
                if (m_2DDashboard != null)
                {
                    m_2DDashboard.enabled = true;
                }
                visiToggle = false;
            }
        }

    }
}
