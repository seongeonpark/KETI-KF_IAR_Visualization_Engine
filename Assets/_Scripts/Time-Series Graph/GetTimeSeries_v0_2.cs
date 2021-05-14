using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Networking;

public class GetTimeSeries_v0_2 : MonoBehaviour
{
    public List<_ID_xmlCon> _requestIDs_xmlCons = new List<_ID_xmlCon>();
    public _ID_xmlCon _received_latest;

    private XmlDocument _received_xml_latest, _received_xml_main, _received_xml_sub;

    private string _received_latest_requestID;
    private string _received_latest_con;
    private string _received_latest_time;

    public string requestURL_IP;
    public string requestURL_BODY;
    private string _requestURL_command;

    public uint numOfRequest = 100;
    public float repeatTime = 10;

    private string _resultString = "";

    private bool _firstRequest = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("GetTimeSeries");
    }

    // Update is called once per frame
    IEnumerator GetTimeSeries()
    {
        while(true)
        {
            UnityWebRequest _latestGet = UnityWebRequest.Get("http://" + requestURL_IP + "/" + requestURL_BODY + "/latest");

            _latestGet.SetRequestHeader("X-M2M-Origin", "LatestGet");
            _latestGet.SetRequestHeader("X-M2M-RI", "REQ000001");
            _latestGet.SetRequestHeader("Accept", "application/xml");

            if(_firstRequest)
            {
               yield return _latestGet.SendWebRequest();
                _firstRequest = false;
            }
            else
            {
                _latestGet.SendWebRequest();

                yield return new WaitForSeconds(repeatTime);
            }
            

            if (_latestGet.isNetworkError || _latestGet.isHttpError)
            {
                Debug.Log(_latestGet.error);
            }
            else
            {
                _received_xml_latest = new XmlDocument();
                _received_xml_latest.LoadXml(_latestGet.downloadHandler.text);

                XmlNodeList _nodeList = _received_xml_latest.GetElementsByTagName("con");
                foreach (XmlNode _tempNode in _nodeList)
                {
                    _received_latest_con = _tempNode.InnerText;
                }

                _nodeList = _received_xml_latest.GetElementsByTagName("ri");
                foreach (XmlNode _tempNode in _nodeList)
                {
                    _received_latest_requestID = _tempNode.InnerText;
                }

                _nodeList = _received_xml_latest.GetElementsByTagName("ct");
                foreach (XmlNode _tempNode in _nodeList)
                {
                    _received_latest_time = _tempNode.InnerText;
                }

                DateTime _dt_createBefore = DateTime.ParseExact(_received_latest_time, "yyyyMMddTHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                TimeSpan ts = new TimeSpan(0, 0, (int)(numOfRequest * repeatTime));
                DateTime _dt_createAfter = _dt_createBefore - ts;

                string _maked_createAfter = _dt_createAfter.ToString("yyyyMMddTHHmmss");
                string _maked_createBefore = _received_latest_time;

                _received_latest = new _ID_xmlCon() {
                    _requestID = _received_latest_requestID,
                    _con = float.Parse(_received_latest_con),
                    _time = _received_latest_time }; //--------------------------------------------------- latest 처리 끝 ---------------------------------------------------

                _requestURL_command = "?fu=1&cra=" + _maked_createAfter + "&crb=" + _maked_createBefore + "&lim=" + numOfRequest.ToString();

                UnityWebRequest _reversedRequestsGet =
                    UnityWebRequest.Get("http://" + requestURL_IP + "/" + requestURL_BODY + _requestURL_command);

                Debug.Log("http://" + requestURL_IP + "/" + requestURL_BODY + _requestURL_command);
                //Debug.Log("http://" + requestURL_IP + "/" + requestURL_BODY + _requestURL_command);
                _reversedRequestsGet.SetRequestHeader("X-M2M-Origin", "ReversedRequestsGet");
                _reversedRequestsGet.SetRequestHeader("X-M2M-RI", "REQ000002");
                _reversedRequestsGet.SetRequestHeader("Accept", "application/xml");

                yield return _reversedRequestsGet.SendWebRequest();

                if(_reversedRequestsGet.isNetworkError || _reversedRequestsGet.isHttpError)
                {
                    Debug.Log(_reversedRequestsGet.error);
                }
                else
                {
                    _received_xml_main = new XmlDocument();
                    _received_xml_main.LoadXml(_reversedRequestsGet.downloadHandler.text);

                    char[] _delimiterChars = { ' ' };
                    string[] _reversed_requestIDs = _received_xml_main.InnerText.Split(_delimiterChars, StringSplitOptions.RemoveEmptyEntries);

                    List<string> _requestList = _reversed_requestIDs.ToList();
                    _requestList.Reverse();

                    string[] _requestIDs = _requestList.ToArray();

                    foreach (string _singleRequestID in _requestIDs)
                    {
                        if(!_requestIDs_xmlCons.Exists(x => x._requestID.Contains(_singleRequestID)))
                        {
                            UnityWebRequest _singleRequestGet =
                            UnityWebRequest.Get("http://" + requestURL_IP + "/" + _singleRequestID);
                            _singleRequestGet.SetRequestHeader("X-M2M-Origin", "SingleRequestGet");
                            _singleRequestGet.SetRequestHeader("X-M2M-RI", "REQ000003");
                            _singleRequestGet.SetRequestHeader("Accept", "application/xml");

                            yield return _singleRequestGet.SendWebRequest();

                            float _innerCon = 0.0f;
                            string _innerTime = "00000000T000000";

                            if (_singleRequestGet.isNetworkError || _singleRequestGet.isHttpError)
                            {
                                Debug.Log(_singleRequestGet.error);
                            }
                            else
                            {
                                _received_xml_sub = new XmlDocument();
                                _received_xml_sub.LoadXml(_singleRequestGet.downloadHandler.text);

                                _nodeList = _received_xml_sub.GetElementsByTagName("con");
                                foreach (XmlNode _singleNode in _nodeList)
                                {
                                    _innerCon = float.Parse(_singleNode.InnerText);
                                }

                                _nodeList = _received_xml_sub.GetElementsByTagName("ct");
                                foreach (XmlNode _singleNode in _nodeList)
                                {
                                    _innerTime = _singleNode.InnerText;
                                }

                                _requestIDs_xmlCons.Add(new _ID_xmlCon() { _requestID = _singleRequestID, _con = _innerCon, _time = _innerTime });

                                if (_requestIDs_xmlCons.Count > numOfRequest)
                                {
                                    _requestIDs_xmlCons.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }

            foreach (_ID_xmlCon _single in _requestIDs_xmlCons)
            {
                _resultString += (_single.ToString() + "\n");
            }
            Debug.Log(_resultString);
            Debug.Log("end");
            _resultString = "" ;
        }
    }
}
