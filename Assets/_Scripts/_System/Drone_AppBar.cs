using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drone_AppBar : MonoBehaviour
{
    [Header("UI:")]
    [SerializeField] private TextMeshProUGUI _ActivationTimeTMP;
    [SerializeField] private TextMeshProUGUI _DeviceID;
    [SerializeField] private TextMeshProUGUI _DateTimeTMP;

    [Header("Data:")]
    [SerializeField] private GameObject _Parser;
    
    private MQTT_Parser_v011 m_MQTT;
    private DateTime mStartTime;

    private void Awake()
    {
        m_MQTT = _Parser.GetComponent<MQTT_Parser_v011>();

    }
    void Start()
    {
        mStartTime = DateTime.Now;
        _DeviceID.text = string.Format("{0}", m_MQTT.test_bridge);
    }

    void Update()
    {
        
        TimeSpan timeDiff = DateTime.Now - mStartTime;
        _ActivationTimeTMP.text = string.Format("Elapsed Time {0:mm\\:ss}", timeDiff);
        _DateTimeTMP.text = DateTime.Now.ToString("yyyy/MM/dd   HH:mm:ss");
    }
}
