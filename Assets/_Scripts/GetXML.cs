using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public enum eURL_IP
{
    InternalIP, ExternalIP
}

public class GetXML : MonoBehaviour
{
    #region Inspector Options
    [SerializeField] private float _receivedData = 0f;
    [Header("URL Options: ")]
    [SerializeField] private eURL_IP _requestURL_IP;
    [SerializeField] private string _requestURL_Body;
    [SerializeField] private float _repeatTime = 3f;
    [Header("Data Range")]
    [SerializeField] private float Min = 0f;
    [SerializeField] private float Max = 100f;
    #endregion

    private XmlDocument _received_xml;
    private string _requestURL;

    #region Properties
    public float RepeatTime { get; private set; }
    public float Data { get => _receivedData; private set => _receivedData = value; }
    #endregion

    void Start()
    {
        _requestURL = GetURL_IP(_requestURL_IP) + _requestURL_Body;
        RepeatTime = _repeatTime;

        StartCoroutine("GetText");
    }

    private IEnumerator GetText()
    {
        while (true)
        {
            UnityWebRequest _www = UnityWebRequest.Get(_requestURL);
            /*
             in origin ==> AE or CSE
             AE = T_Demo_IoT
             CSE = T_Demo_HarsleY28
             */
            _www.SetRequestHeader("X-M2M-Origin", "S.0.HARSLEY28");

            /*
             in RI ==> RequestID
             in case of, this header will be set to specific require id (or number)
             */
            _www.SetRequestHeader("X-M2M-RI", "REQ00001");

            _www.SetRequestHeader("Accept", "application/xml");

            _www.SendWebRequest();

            yield return new WaitForSeconds(_repeatTime);

            if (_www.isNetworkError || _www.isHttpError)
            {
                Debug.Log(_www.error);
            }
            else
            {
                _received_xml = new XmlDocument();

                _received_xml.LoadXml(_www.downloadHandler.text);
                
                XmlNodeList _nodeList = _received_xml.GetElementsByTagName("con");
                foreach (XmlNode _tempNode in _nodeList)
                {
                    var temp = Convert.ToSingle(_tempNode.InnerText);
                    var clampedData = Mathf.Clamp(temp, Min, Max);
                    Data = Normalize(clampedData, 0f, 100f);
                }
            }
        }
    }

    private float Normalize(float value, float min, float max)
    {
        var x = (max - min) * ((value - min) / (max - min)) + min;
        return x;
    }

    private string GetURL_IP(eURL_IP IP)
    {
        switch (IP)
        {
            case eURL_IP.InternalIP:
                //return "http://192.168.0.71:7579/";
                return "http://192.168.0.2:7579/";
            case eURL_IP.ExternalIP:
                return "http://175.207.29.140:7579/";
            default:
                return "http://175.207.29.140:7579/";
        }
    }
}