using System;

using System.Collections.Generic;

using System.Reflection;

using static MAVLink;

using System.Xml;
using System.Text.RegularExpressions;

public class MavMessage
{
    private MAVLinkMessage _msgMavLink;
    private DateTime _msgGeneratedTime;
    private string _msgToString;

    public DateTime MsgGeneratedTime
    {
        get
        {
            return _msgGeneratedTime;
        }
    }

    public MAVLinkMessage MsgMavLink
    {
        get
        {
            return _msgMavLink;
        }
    }

    public string MsgToString
    {
        get
        {
            return _msgToString;
        }
    }

    public MavMessage(string _requestedMessage)
    {
        string[] doubleColone = _requestedMessage.Split(new string[] { @""":""" }, StringSplitOptions.None); // 1st check

        doubleColone[0] = doubleColone[0].Replace("\"", "");
        doubleColone[1] = doubleColone[1].Replace("\"", "");

        _msgGeneratedTime = DateTime.ParseExact(doubleColone[0], "yyyy-MM-ddThh:mm:ssfff", null); // 2nd check

        byte[] msgBuffer = strToHexByte(doubleColone[1]);

        _msgMavLink = new MAVLinkMessage(msgBuffer, _msgGeneratedTime); // 3rd check
                                                                        //_msgMavLink = new MAVLinkMessage(msgBuffer); // 3rd check

        _msgToString = "DateTime // " + _msgGeneratedTime.ToString("yyyyMMddThhmmssfff") + " // ";
        _msgToString += "msgID: " + _msgMavLink.msgid + " // ";

        if (_msgMavLink.msgtypename != null)
        {
            _msgToString += _msgMavLink.msgtypename.ToString() + " // ";

            var _data = _msgMavLink.data;

            foreach (var _tempField in _data.GetType().GetFields())
            {
                FieldInfo _tempFieldInfo = _tempField;
                string fieldName = _tempFieldInfo.Name;

                var fieldValue = _tempFieldInfo.GetValue(_data);
                var fieldUnit = GetUnit(_tempField.ToString(), msgid: _msgMavLink.msgid);

                _msgToString += fieldName + ": " + fieldValue + " " + fieldUnit + " // ";
            }
        }

        _msgToString += "end.\t<== " + doubleColone[0] + " // " + doubleColone[1] + "\n";
    }

    public byte[] strToHexByte(string hexString)
    {
        byte[] returnBytes = new byte[hexString.Length / 2];

        string hexArray = "";
        string convArray = "";

        for (int i = 0; i < returnBytes.Length; i++)
        {
            returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            string temp_hex = hexString.Substring(i * 2, 2) + " ";
            string temp_conv = Convert.ToByte(hexString.Substring(i * 2, 2), 16).ToString() + " ";

            hexArray += temp_hex;
            convArray += temp_conv;
        }

        return returnBytes;
    }
}

public class MsgGroup
{
    private string _ipAddress;
    private string _hostNtargetNbridgeAddress;
    private string _msgGroupName;
    private string _fullAddress;

    private DateTime _generatedTime;

    private List<MavMessage> _msgs;

    private bool _isAccessed;


    private string _httpResult;
    private string _httpResultBefore;

    public string IPAddress
    {
        get
        {
            return _ipAddress;
        }
    }

    public string HostNTargetNBridgeAddress
    {
        get
        {
            return _hostNtargetNbridgeAddress;
        }
    }

    public string MsgGroupName
    {
        get
        {
            return _msgGroupName;
        }
    }

    public string FullAddress
    {
        get
        {
            return _fullAddress;
        }
    }

    public DateTime GeneratedTime
    {
        get
        {
            return _generatedTime;
        }
    }

    public List<MavMessage> Msgs
    {
        get
        {
            return _msgs;
        }
    }

    public bool IsAccessed
    {
        get
        {
            return _isAccessed;
        }
    }

    public string HTTPResult
    {
        get
        {
            return _httpResult;
        }
    }

    public MsgGroup(string _param_ipAddress, string _param_hostNtargetNbridgeAddress, string _param_msgGroupName)
    {
        // _param_ipAddress : http://203.253.128.177:7579
        // _param_hostNtargetNbridgeAddress : Mobius /KETI_MUV/Drone_Data/UMACAir11/2020_11_18_T_03_09
        // _msgGroupName : ... , ... , 4-20201118031212802

        _ipAddress = _param_ipAddress;
        _hostNtargetNbridgeAddress = _param_hostNtargetNbridgeAddress;
        _msgGroupName = _param_msgGroupName;

        _fullAddress = _param_ipAddress + '/' + _hostNtargetNbridgeAddress + '/' + _msgGroupName;

        _generatedTime = DateTime.ParseExact(_msgGroupName.Substring(2), "yyyyMMddHHmmssfff", null);

        _isAccessed = false;

        _msgs = new List<MavMessage>();
    }

    public void SerializeResult_MsgGroup(string _accessResult) // 위의 Parants, targetname 합친 string을 주소로 접근하여, 받아온 결과를 통째로 serialise
    {
        if (_httpResultBefore != _accessResult)
        {
            _httpResult = _accessResult;

            string _uril_txt = _accessResult.Substring(_accessResult.IndexOf("con") + 6);
            string _untailed_txt = _uril_txt.Substring(0, _uril_txt.Length - 3);

            string[] comma_split = _untailed_txt.Split(',');
            foreach (string _tempString in comma_split)
            {
                MavMessage _tempMavMessage = new MavMessage(_tempString);

                switch (_msgs.Count)
                {
                    case 0:
                        _msgs.Add(_tempMavMessage);
                        break;
                    case 1:
                        int _compareResult = DateTime.Compare(_tempMavMessage.MsgGeneratedTime, _msgs[0].MsgGeneratedTime); //check 2

                        if (_compareResult > 0)
                        {
                            _msgs.Add(_tempMavMessage);
                        }
                        else if (_compareResult < 0)
                        {
                            _msgs.Insert(0, _tempMavMessage);
                        }
                        break;
                    default:
                        for (int i = 0; i < _msgs.Count - 1; i++)
                        {
                            DateTime _DateTime_n0 = _msgs[i].MsgGeneratedTime;
                            DateTime _DateTime_n1 = _msgs[i + 1].MsgGeneratedTime;

                            int _compare_n0 = DateTime.Compare(_DateTime_n0, _tempMavMessage.MsgGeneratedTime);
                            int _compare_n1 = DateTime.Compare(_DateTime_n1, _tempMavMessage.MsgGeneratedTime);

                            if (_compare_n0 < 0 && _compare_n1 < 0)
                            {
                                if (i == _msgs.Count - 1)
                                {
                                    _msgs.Add(_tempMavMessage);
                                    break;
                                }
                            }
                            else if (_compare_n0 < 0 && _compare_n1 < 0)
                            {
                                _msgs.Insert(i + 1, _tempMavMessage); //어느 곳에 삽입되는지 확인해야 함
                                break;
                            }
                            else if (_compare_n0 > 0 && _compare_n1 > 0)
                            {
                                _msgs.Insert(i, _tempMavMessage);
                                break;
                            }
                        }
                        break;
                }
            }
            _isAccessed = true;
        }
        _httpResultBefore = _httpResult;
    }
}

public class Bridge
{
    private string _ipAddress;
    private string _hostNtargetAddress;
    private string _bridgeName;
    private string _fullAddress;
    private DateTime _generatedTime;

    private List<MsgGroup> _msgGroups;

    private bool _existNewMsgGroup;

    private string _httpResult;
    private string _httpResultBefore;

    public string IPAddress
    {
        get
        {
            return _ipAddress;
        }
    }

    public string HostNTargetAddress
    {
        get
        {
            return _hostNtargetAddress;
        }
    }

    public string BridgeName
    {
        get
        {
            return _bridgeName;
        }
    }

    public string FullAddress
    {
        get
        {
            return _fullAddress;
        }
    }

    public DateTime GeneratedTime
    {
        get
        {
            return _generatedTime;
        }
    }

    public List<MsgGroup> MsgGroups
    {
        get
        {
            return _msgGroups;
        }
    }

    public bool ExistNewMsgGroup
    {
        get
        {
            return _existNewMsgGroup;
        }
    }

    public string HTTPResult
    {
        get
        {
            return _httpResult;
        }
    }

    private void AddMsgGroup(string _newMsgGroupName)
    {
        MsgGroup _tempMsgGroup = new MsgGroup(_ipAddress, _hostNtargetAddress + '/' + _bridgeName, _newMsgGroupName);

        switch (_msgGroups.Count)
        {
            case 0:
                _msgGroups.Add(_tempMsgGroup);
                break;
            case 1:
                int _compareResult = DateTime.Compare(_tempMsgGroup.GeneratedTime, _msgGroups[0].GeneratedTime);

                if (_compareResult > 0)
                {
                    _msgGroups.Add(_tempMsgGroup);
                }
                else if (_compareResult < 0)
                {
                    _msgGroups.Insert(0, _tempMsgGroup);
                }
                break;
            default:
                for (int i = 0; i < _msgGroups.Count - 1; i++)
                {
                    DateTime _DateTime_n0 = _msgGroups[i].GeneratedTime;
                    DateTime _DateTime_n1 = _msgGroups[i + 1].GeneratedTime;

                    int _compare_n0 = DateTime.Compare(_DateTime_n0, _tempMsgGroup.GeneratedTime);
                    int _compare_n1 = DateTime.Compare(_DateTime_n1, _tempMsgGroup.GeneratedTime);

                    if (_compare_n0 < 0 && _compare_n1 < 0)
                    {
                        if (i == _msgGroups.Count - 1)
                        {
                            _msgGroups.Add(_tempMsgGroup);
                            break;
                        }
                        //pass;
                    }
                    else if (_compare_n0 < 0 && _compare_n1 > 0)
                    {
                        _msgGroups.Insert(i + 1, _tempMsgGroup); //어느 곳에 삽입되는지 확인해야 함
                        break;
                    }
                    else if (_compare_n0 > 0 && _compare_n1 > 0)
                    {
                        _msgGroups.Insert(i, _tempMsgGroup);
                        break;
                    }
                }
                break;
        }
        _existNewMsgGroup = true;
    }

    public List<MsgGroup> NotAccessedMsgGroups()
    {
        List<MsgGroup> _notAccessedMsgGroups = new List<MsgGroup>();

        foreach (MsgGroup _tempSingleMsgGroup in _msgGroups)
        {
            if (!_tempSingleMsgGroup.IsAccessed)
            {
                _notAccessedMsgGroups.Add(_tempSingleMsgGroup);
            }
        }
        _existNewMsgGroup = false;
        return _notAccessedMsgGroups;
    }

    public Bridge(string _param_ipAddress, string _param_hostNtargetAddress, string _param_bridgeName)
    {
        // _param_ipAddress : http://203.253.128.177:7579
        // _param_hostNTargetAddress : Mobius /KETI_MUV/Drone_Data/UMACAir11
        // _targetName : ... , ... , 2020_11_18_T_03_09

        _ipAddress = _param_ipAddress;
        _hostNtargetAddress = _param_hostNtargetAddress;
        _bridgeName = _param_bridgeName;

        _fullAddress = _ipAddress + "/" + _hostNtargetAddress + '/' + _bridgeName;

        _generatedTime = DateTime.ParseExact(_bridgeName, "yyyy_MM_dd_T_HH_mm", null);
        _existNewMsgGroup = true;

        _msgGroups = new List<MsgGroup>();
    }

    public void SerialiseResult_Bridge(string _accessResult)
    {
        if (_httpResultBefore != _accessResult)
        {

            _httpResult = _accessResult;
            string _uril_txt = _accessResult.Substring(_accessResult.IndexOf("m2m:uril") + 11);
            string _untailed_txt = _uril_txt.Substring(0, _uril_txt.Length - 2);

            string[] comma_split = _untailed_txt.Split(',');
            foreach (string _tempString in comma_split)
            {
                string _replace_DoubleQuote = _tempString.Replace("\"", "");
                string _newMsgGroupName = _replace_DoubleQuote.Replace(_hostNtargetAddress + "/" + _bridgeName + "/", "");

                Regex regex = new Regex(@"^[0-9]{1}-[0-9]{17}");

                if (regex.IsMatch(_newMsgGroupName))
                {
                    bool _isExist = false;

                    foreach (MsgGroup _tempMsgGroup in _msgGroups)
                    {
                        if (_tempMsgGroup.MsgGroupName == _newMsgGroupName)
                        {
                            _isExist = true; break;
                        }
                    }

                    if (!_isExist)
                    {
                        AddMsgGroup(_newMsgGroupName);
                    }
                }
            }
        }
        _httpResultBefore = _httpResult;
    }
}

public class Base
{
    //_ipAddress : http://203.253.128.177:7579
    //_hostAddress : Mobius/KETI_MUV/Drone_Data
    //_targetAddress : UMACAir11
    private string _targetName;
    private string _hostAddress;
    private string _ipAddress;
    private string _fullAddress;

    private DateTime _generatedTime;

    private List<Bridge> _bridges;
    private Bridge _lastBridge;

    private bool _existNewBridge;

    private string _httpResult;
    private string _httpResultBefore = "";

    public string IPAddress
    {
        get
        {
            return _ipAddress;
        }
    }
    public string HostAddress
    {
        get
        {
            return _hostAddress;
        }
    }
    public string TargetName
    {
        get
        {
            return _targetName;
        }
    }

    public string FullAddress
    {
        get
        {
            return _fullAddress;
        }
    }

    public DateTime GeneratedTime
    {
        get
        {
            return _generatedTime;
        }
        set // base의 generatedTime의 경우, 단어가 아이템 체계가 아니므로, 초기에 한번 다 읽은 후 ct를 추출하고, 그 다음 fu=1~ 을 이용하여 레벨 1 아래의 데이터들을 수집. (혹은 안씀)
        {
            _generatedTime = value;
        }

    }

    public List<Bridge> Bridges
    {
        get
        {
            return _bridges;
        }
    }

    public Bridge LastBridge
    {
        get
        {
            return _lastBridge;
        }
    }

    public bool ExistNewBridge
    {
        get
        {
            return _existNewBridge;
        }
    }

    public string HTTPResult
    {
        get
        {
            return _httpResult;
        }
    }

    private void AddBridge(string _newBridgeName)
    {
        Bridge _tempBridge = new Bridge(_ipAddress, _hostAddress + "/" + _targetName, _newBridgeName);

        switch (_bridges.Count)
        {
            case 0:
                //Debug.Log(_bridges.Count + " ==> DT_temp: " + _tempBridge.GeneratedTime.ToString());
                _bridges.Add(_tempBridge);

                break;
            case 1:
                int _compareResult = DateTime.Compare(_tempBridge.GeneratedTime, _bridges[0].GeneratedTime);

                //Debug.Log(_bridges.Count + " ==> DT_temp: " + _tempBridge.GeneratedTime.ToString() + " // DT_N0: " + _bridges[0].GeneratedTime.ToString() + " // _compareResult: " + _compareResult);

                if (_compareResult > 0)
                {
                    _bridges.Add(_tempBridge);
                }
                else if (_compareResult < 0)
                {
                    _bridges.Insert(0, _tempBridge);
                }
                break;
            default:
                for (int i = 0; i < _bridges.Count - 1; i++)
                {
                    DateTime _DateTime_n0 = _bridges[i].GeneratedTime;
                    DateTime _DateTime_n1 = _bridges[i + 1].GeneratedTime;

                    int _compare_n0 = DateTime.Compare(_DateTime_n0, _tempBridge.GeneratedTime);
                    int _compare_n1 = DateTime.Compare(_DateTime_n1, _tempBridge.GeneratedTime);

                    //Debug.Log(_bridges.Count);
                    //Debug.Log(i + " ==> DT_n0: " + _DateTime_n0.ToString() + " ==> DT_temp: " + _tempBridge.GeneratedTime.ToString() + "(" + _compare_n0 + ")");
                    //Debug.Log(i + " ==> DT_n1: " + _DateTime_n1.ToString() + " ==> DT_temp: " + _tempBridge.GeneratedTime.ToString() + "(" + _compare_n1 + ")");

                    if (_compare_n0 < 0 && _compare_n1 < 0)
                    {
                        //Debug.Log("\tMatch 0, " + _compare_n0 + ", " + _compare_n1);
                        if (i == (_bridges.Count - 1))
                        {
                            _bridges.Add(_tempBridge);
                            break;
                        }
                    }
                    else if (_compare_n0 < 0 && _compare_n1 > 0)
                    {
                        //Debug.Log("\tMatch 1, " + _compare_n0 + ", " + _compare_n1);
                        _bridges.Insert(i + 1, _tempBridge);
                        break;
                    }
                    else if (_compare_n0 > 0 && _compare_n1 > 0)
                    {
                        //Debug.Log("\tMatch 2, " + _compare_n0 + ", " + _compare_n1);
                        _bridges.Insert(i, _tempBridge);
                        break;
                    }
                }
                break;
        }
        _existNewBridge = true;
    }

    public List<Bridge> NotAccessedBridges()
    {
        List<Bridge> _notAccessedBridges = new List<Bridge>();

        foreach (Bridge _tempSingleBridge in _bridges)
        {

            if (_tempSingleBridge.ExistNewMsgGroup)
            {
                _notAccessedBridges.Add(_tempSingleBridge);
            }
        }
        _existNewBridge = false;

        return _notAccessedBridges;
    }

    public Base(string _param_ipAddress, string _param_hostAddress, string _param_targetName)
    {
        //_ipAddress : http://203.253.128.177:7579
        //_hostAddress : Mobius/KETI_MUV/Drone_Data
        //_targetAddress : UMACAir11
        _ipAddress = _param_ipAddress;
        _hostAddress = _param_hostAddress;
        _targetName = _param_targetName;

        _fullAddress = _ipAddress + '/' + _hostAddress + '/' + _targetName;

        _bridges = new List<Bridge>();
    }

    public void SerialiseResult_Base(string _accessResult)
    {
        if (_httpResultBefore != _accessResult) //==>  bridge 가 하나 더 생겼다는 의미 or 생성된 뒤 처음 실행 된 것
        {
            _httpResult = _accessResult;

            XmlDocument _tempXml = new XmlDocument();
            _tempXml.LoadXml(_httpResult);

            foreach (string _tempString in _tempXml.InnerText.Split(' '))
            {
                string _hostNtarget = _hostAddress + '/' + _targetName;
                string _newBridgeName = _tempString.Replace(_hostNtarget + "/", "");

                Regex regex = new Regex(@"^[0-9]{4}_[0-9]{2}_[0-9]{2}_T_[0-9]{2}_[0-9]{2}");
                if (regex.IsMatch(_newBridgeName))
                {
                    bool _isExist = false;

                    foreach (Bridge _tempBridge in _bridges)
                    {
                        if (_tempBridge.BridgeName == _newBridgeName) { _isExist = true; break; }
                    }

                    if (!_isExist)
                    {
                        AddBridge(_newBridgeName);
                    }
                }
            }
        }
        _httpResultBefore = _httpResult;
        _lastBridge = _bridges[_bridges.Count - 1];
    }
}


