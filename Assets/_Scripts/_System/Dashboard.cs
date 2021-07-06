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

    #region PRIVATE_VARIABLES

    private TextMeshProUGUI _mTxtTime;
    private TextMeshProUGUI _mTxtMachine;
    private Transform _mRootObj;

    private ParserManager m_ParserManager;

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

    }

    private void Update()
    {
        _DateTimeTMP.text = DateTime.Now.ToString("yyyy/MM/dd   HH:mm:ss");
        //    _mTxtTime.text = DateTime.Now.ToString("yyyy/MM/dd   HH:mm:ss");

    }

    private string GetObjName(Transform tf)
    {
        if (tf.name == "IoT_HarsleY28") return "HarsleY28";
        else if (tf.name == "IoT_HX200") return "HX200";
        else if (tf.name == "IoT_HX200temp") return "HX200_TEMP";
        else return "Machine";
    }
}
