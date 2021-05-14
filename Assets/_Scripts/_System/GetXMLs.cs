using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class GetXMLs : MonoBehaviour
{
    #region Inspector Options
    [SerializeField] private float[] _ReceivedData;
    [Header("URL Options: ")]
    [SerializeField] private float _RepeatTime = 3f;
    [Tooltip("Internal IP: http://192.168.0.2:7579/\n" +
        "External IP: http://175.207.29.140:7579/")]
    [SerializeField] private eURL_IP _RequestURL_IP;
    [SerializeField] private string[] _RequestURL_Bodys;

    #endregion

    #region Properties

    public float RepeatTime { get => _RepeatTime; private set => _RepeatTime = value; }
    public float[] Data { get => _ReceivedData; private set => _ReceivedData = value; }

    #endregion
    
    private string[] m_RequestURLs;

    private void Start()
    {
        m_RequestURLs = new string[_RequestURL_Bodys.Length];

        for (int i = 0; i < _RequestURL_Bodys.Length; i++)
        {
            m_RequestURLs[i] = GetURL_IP(_RequestURL_IP) + _RequestURL_Bodys[i];
        }
        
        RepeatTime = _RepeatTime;
        _ReceivedData = new float[_RequestURL_Bodys.Length];

        StartCoroutine("GetText");
    }

    private IEnumerator GetText()
    {
        UnityWebRequest[] _www = new UnityWebRequest[m_RequestURLs.Length];

        while (true)
        {
            for (int i = 0; i < m_RequestURLs.Length; i++)
            {
                _www[i] = GetWebRequest(m_RequestURLs[i]);
            }

            yield return new WaitForSeconds(_RepeatTime);

            for (int i = 0; i < m_RequestURLs.Length; i++)
            {
                _ReceivedData[i] = GetData(_www[i]);
            }

            Data = _ReceivedData;
            
        }
    }

    private UnityWebRequest GetWebRequest(string URL)
    {
        UnityWebRequest _www = UnityWebRequest.Get(URL);
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

        return _www;
    }
    private float GetData(UnityWebRequest www)
    {
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            return 0f;
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(www.downloadHandler.text);

            XmlNodeList _nodeList = xmlDocument.GetElementsByTagName("con");
            
            float data = 0f;
            foreach (XmlNode _tempNode in _nodeList)
            {
                data = Convert.ToSingle(_tempNode.InnerText);
            }
            return data;
        }
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