using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;

using static MAVLink;

public class HTTP_Parser_v01 : MonoBehaviour
{
    //_ipAddress : http://203.253.128.177:7579
    //_hostAddress : Mobius/KETI_MUV/Drone_Data
    //_targetAddress : UMACAir11
    public string requestURL_IP = "http://203.253.128.177:7579";
    public string requestURL_HOST = "Mobius/KETI_MUV/Drone_Data";
    public string requestURL_TARGETNAME = "UMACAir11";

    public float repeatTime = 60.0f;

    private bool firstRequest = true;

    private bool isBaseLoaded = false;
    private bool isBridgeLoaded = false;
    private bool isMsgGroupLoaded = false;

    private Base baseRequest;

    private string _filePath = "..\\http_test";

    //30, 33, 65, 74, 136, 147 ==> private List<>
    public List<mavlink_attitude_t> _list_attitude; // 30 //roll, pitch, yaw, etc.
    private List<string> _str_attitude;

    public List<mavlink_global_position_int_t> _list_globalPosition; // 33 //lat, lon, alt, hdg, etc.
    private List<string> _str_globalPosition;

    public List<mavlink_rc_channels_t> _list_rcChannels; // 65 //chan1_raw ~ chan16_raw, etc.
    private List<string> _str_rcChannels;

    public List<mavlink_vfr_hud_t> _list_vfrHud; // 74 // airspeed, groundspeed, heading, alt, etc.
    private List<string> _str_vfrHud;

    public List<mavlink_terrain_report_t> _list_terrain; //136 // lat, lon, etc //필요할지 모르겠음
    private List<string> _str_terrain;

    public List<mavlink_battery_status_t> _list_battery; //147 // temperature, voltage, currents, etc.
    private List<string> _str_battery;

    private List<string> _str_allMsg;
    private List<string> _str_knownMsg;
    private List<string> _str_unknownMsg;

    private int _writeCntForFiles;

    public List<int> _out_alt, _out_alt_relative;
    public List<float> _out_airspeed;
    public List<float> _out_roll, _out_pitch, _out_yaw;
    public List<int> _out_lat, _out_lon;

    public List<short> _out_currentbattery;
    public List<sbyte> _out_remainbattery;

    public mavlink_attitude_t convert_attitude(MAVLinkMessage _msg)
    {
        uint time_boot_ms = 0;
        float roll = 0;
        float pitch = 0;
        float yaw = 0;
        float rollspeed = 0;
        float pitchspeed = 0;
        float yawspeed = 0;

        var _data = _msg.data;

        foreach(var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);
            
            switch (fieldName)
            {
                case "time_boot_ms":
                    time_boot_ms = (uint)fieldValue;
                    break;
                case "roll":
                    roll = (float)fieldValue;
                    break;
                case "pitch":
                    pitch = (float)fieldValue;
                    break;
                case "yaw":
                    yaw = (float)fieldValue;
                    break;
                case "rollspeed":
                    rollspeed = (float)fieldValue;
                    break;
                case "pitchspeed":
                    pitchspeed = (float)fieldValue;
                    break;
                case "yawspeed":
                    yawspeed = (float)fieldValue;
                    break;
            }
        }

        return new mavlink_attitude_t(time_boot_ms, roll, pitch, yaw, rollspeed, pitchspeed, yawspeed);
    }
    public mavlink_global_position_int_t convert_globalposition(MAVLinkMessage _msg)
    {
        uint time_boot_ms = 0;
        int lat = 0;
        int lon = 0;
        int alt = 0;
        int relative_alt = 0;
        short vx = 0;
        short vy = 0;
        short vz = 0;
        ushort hdg = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "time_boot_ms":
                    time_boot_ms = (uint)fieldValue;
                    break;
                case "lat":
                    lat = (int)fieldValue;
                    break;
                case "lon":
                    lon = (int)fieldValue;
                    break;
                case "alt":
                    alt = (int)fieldValue;
                    break;
                case "relative_alt":
                    relative_alt = (int)fieldValue;
                    break;
                case "vx":
                    vx = (short)fieldValue;
                    break;
                case "vy":
                    vy = (short)fieldValue;
                    break;
                case "vz":
                    vz = (short)fieldValue;
                    break;
                case "hdg":
                    hdg = (ushort)fieldValue;
                    break;
            }
        }

        return new mavlink_global_position_int_t(time_boot_ms, lat, lon, alt, relative_alt, vx, vy, vz, hdg);
    }
    public mavlink_rc_channels_t convert_rcchannels(MAVLinkMessage _msg)
    {
        uint time_boot_ms = 0;
        ushort chan1_raw = 0;
        ushort chan2_raw = 0;
        ushort chan3_raw = 0;
        ushort chan4_raw = 0;
        ushort chan5_raw = 0;
        ushort chan6_raw = 0;
        ushort chan7_raw = 0;
        ushort chan8_raw = 0;
        ushort chan9_raw = 0;
        ushort chan10_raw = 0;
        ushort chan11_raw = 0;
        ushort chan12_raw = 0;
        ushort chan13_raw = 0;
        ushort chan14_raw = 0;
        ushort chan15_raw = 0;
        ushort chan16_raw = 0;
        ushort chan17_raw = 0;
        ushort chan18_raw = 0;
        byte chancount = 0;
        byte rssi = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "time_boot_ms":
                    time_boot_ms = (uint)fieldValue;
                    break;
                case "chan1_raw":
                    chan1_raw = (ushort)fieldValue;
                    break;
                case "chan2_raw":
                    chan2_raw = (ushort)fieldValue;
                    break;
                case "chan3_raw":
                    chan3_raw = (ushort)fieldValue;
                    break;
                case "chan4_raw":
                    chan4_raw = (ushort)fieldValue;
                    break;
                case "chan5_raw":
                    chan5_raw = (ushort)fieldValue;
                    break;
                case "chan6_raw":
                    chan6_raw = (ushort)fieldValue;
                    break;
                case "chan7_raw":
                    chan7_raw = (ushort)fieldValue;
                    break;
                case "chan8_raw":
                    chan8_raw = (ushort)fieldValue;
                    break;
                case "chan9_raw":
                    chan9_raw = (ushort)fieldValue;
                    break;
                case "chan10_raw":
                    chan10_raw = (ushort)fieldValue;
                    break;
                case "chan11_raw":
                    chan11_raw = (ushort)fieldValue;
                    break;
                case "chan12_raw":
                    chan12_raw = (ushort)fieldValue;
                    break;
                case "chan13_raw":
                    chan13_raw = (ushort)fieldValue;
                    break;
                case "chan14_raw":
                    chan14_raw = (ushort)fieldValue;
                    break;
                case "chan15_raw":
                    chan15_raw = (ushort)fieldValue;
                    break;
                case "chan16_raw":
                    chan16_raw = (ushort)fieldValue;
                    break;
                case "chan17_raw":
                    chan17_raw = (ushort)fieldValue;
                    break;
                case "chan18_raw":
                    chan18_raw = (ushort)fieldValue;
                    break;
                case "chancount":
                    chancount = (byte)fieldValue;
                    break;
                case "rssi":
                    rssi = (byte)fieldValue;
                    break;
            }
        }

        return new mavlink_rc_channels_t(time_boot_ms, chan1_raw, chan2_raw, chan3_raw, chan4_raw, chan5_raw, chan6_raw, chan7_raw, chan8_raw, chan9_raw, chan10_raw, 
            chan11_raw, chan12_raw, chan13_raw, chan14_raw, chan15_raw, chan16_raw, chan17_raw, chan18_raw, chancount, rssi);
    }
    public mavlink_vfr_hud_t convert_vfrhud(MAVLinkMessage _msg)
    {
        float airspeed = 0;
        float groundspeed = 0;
        float alt = 0;
        float climb = 0;
        short heading = 0;
        ushort throttle = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "airspeed":
                    airspeed = (float)fieldValue;
                    break;
                case "groundspeed":
                    groundspeed = (float)fieldValue;
                    break;
                case "alt":
                    alt = (float)fieldValue;
                    break;
                case "climb":
                    climb = (float)fieldValue;
                    break;
                case "heading":
                    heading = (short)fieldValue;
                    break;
                case "throttle":
                    throttle = (ushort)fieldValue;
                    break;
            }
        }

        return new mavlink_vfr_hud_t(airspeed, groundspeed, alt, climb, heading, throttle);
    }
    public mavlink_terrain_report_t convert_terrainreport(MAVLinkMessage _msg)
    {
        int lat = 0;
        int lon = 0;
        float terrain_height = 0;
        float current_height = 0;
        ushort spacing = 0;
        ushort pending = 0;
        ushort loaded = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "lat":
                    lat = (int)fieldValue;
                    break;
                case "lon":
                    lon = (int)fieldValue;
                    break;
                case "terrain_height":
                    terrain_height = (float)fieldValue;
                    break;
                case "current_height":
                    current_height = (float)fieldValue;
                    break;
                case "spacing":
                    spacing = (ushort)fieldValue;
                    break;
                case "pending":
                    pending = (ushort)fieldValue;
                    break;
                case "loaded":
                    loaded = (ushort)fieldValue;
                    break;
            }
        }

        return new mavlink_terrain_report_t(lat, lon, terrain_height, current_height, spacing, pending, loaded);
    }
    public mavlink_battery_status_t convert_batterystatus(MAVLinkMessage _msg)
    {
        int current_consumed = 0;
        int energy_consumed = 0;
        short temperature = 0;
        ushort[] voltages = null;
        short current_battery = 0;
        byte id = 0;
        byte battery_function = 0;
        byte type = 0;
        sbyte battery_remaining = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "current_consumed":
                    current_consumed = (int)fieldValue;
                    break;
                case "energy_consumed":
                    energy_consumed = (int)fieldValue;
                    break;
                case "temperature":
                    temperature = (short)fieldValue;
                    break;
                case "voltages":
                    voltages = (ushort[])fieldValue;
                    break;
                case "current_battery":
                    current_battery = (short)fieldValue;
                    break;
                case "id":
                    id = (byte)fieldValue;
                    break;
                case "battery_function":
                    battery_function = (byte)fieldValue;
                    break;
                case "type":
                    type = (byte)fieldValue;
                    break;
                case "battery_remaining":
                    battery_remaining = (sbyte)fieldValue;
                    break;
            }
        }

        return new mavlink_battery_status_t(current_consumed, energy_consumed, temperature, voltages, current_battery, id, battery_function, type, battery_remaining);
    }
    // Start is called before the first frame update
    void Start()
    {
        baseRequest = new Base(requestURL_IP, requestURL_HOST, requestURL_TARGETNAME);

        //20개 제한 생성 ( 화면에서 보여줄 때, 20개면 충분 할 듯 )
        _list_attitude = new List<mavlink_attitude_t>();
        _list_globalPosition = new List<mavlink_global_position_int_t>();
        _list_rcChannels = new List<mavlink_rc_channels_t>();
        _list_vfrHud = new List<mavlink_vfr_hud_t>();
        _list_terrain = new List<mavlink_terrain_report_t>();
        _list_battery = new List<mavlink_battery_status_t>();

        _writeCntForFiles = 0;

        _str_allMsg = new List<string>();
        _str_knownMsg = new List<string>();
        _str_unknownMsg = new List<string>();

        _str_attitude = new List<string>();
        _str_globalPosition = new List<string>();
        _str_rcChannels = new List<string>();
        _str_vfrHud = new List<string>();
        _str_terrain = new List<string>();
        _str_battery = new List<string>();

        _out_alt = new List<int>();
        _out_alt_relative = new List<int>();
        _out_airspeed = new List<float>();
        
        _out_roll = new List<float>();
        _out_pitch = new List<float>();
        _out_yaw = new List<float>();

        _out_lat = new List<int>();
        _out_lon = new List<int>();

        _out_currentbattery = new List<short>();
        _out_remainbattery = new List<sbyte>();

        StartCoroutine(GetHTTP_0Tier_Base_Request());
    }

    public IEnumerator GetHTTP_0Tier_Base_Request()
    {
        while (true)
        {
            isBaseLoaded = false;

            UnityWebRequest _info_inDrone_Get =
                UnityWebRequest.Get(baseRequest.FullAddress + "?fu=1&lvl=1");
            _info_inDrone_Get.SetRequestHeader("X-M2M-Origin", "FU1_LVL1_Get");
            _info_inDrone_Get.SetRequestHeader("X-M2M-RI", "REQ00001");
            _info_inDrone_Get.SetRequestHeader("Accept", "application/xml");

            if (firstRequest)
            {
                yield return _info_inDrone_Get.SendWebRequest();
            }
            else
            {
                _info_inDrone_Get.SendWebRequest();

                yield return new WaitForSeconds(repeatTime);
            }

            if (_info_inDrone_Get.isNetworkError || _info_inDrone_Get.isHttpError)
            {
                Debug.Log(_info_inDrone_Get.error);
            }
            else
            {
                File.WriteAllText(_filePath + "\\base.xml", _info_inDrone_Get.downloadHandler.text);
                baseRequest.SerialiseResult_Base(_info_inDrone_Get.downloadHandler.text);
                
                yield return GetHTTP_1Tier_Bridge_Request();
            }
            firstRequest = false;
            Debug.Log("0tier end.");
        }
    }

    public IEnumerator GetHTTP_1Tier_Bridge_Request()
    {
        Bridge notAccessedLastBridge = baseRequest.LastBridge;
                
        UnityWebRequest _lastBridge_inBase_Get =
            UnityWebRequest.Get(notAccessedLastBridge.FullAddress + "?fu=1&lvl=1");

        _lastBridge_inBase_Get.SetRequestHeader("X-M2M-Origin", "FU1_LVL1_Get");
        _lastBridge_inBase_Get.SetRequestHeader("X-M2M-RI", "REQ00002");
        _lastBridge_inBase_Get.SetRequestHeader("Accept", "application/json");

        yield return _lastBridge_inBase_Get.SendWebRequest();

        if (_lastBridge_inBase_Get.isNetworkError || _lastBridge_inBase_Get.isHttpError)
        {
            Debug.Log(_lastBridge_inBase_Get.error);
        }
        else
        {
            int _tempIndex = baseRequest.Bridges.IndexOf(notAccessedLastBridge);

            string _tempResponse = _lastBridge_inBase_Get.downloadHandler.text;
            File.WriteAllText(_filePath + "\\bridge" + _tempIndex + ".txt", _tempResponse);
            baseRequest.Bridges[_tempIndex].SerialiseResult_Bridge(_tempResponse);
        }
        
        yield return GetHTTP_2Tier_MsgGroup_Request();

        Debug.Log("--> 1tier end.");
        
        /*
        //모든 bridge의 경우 ==> 시간 오래 걸림.
        List<Bridge> notAccessedBridges = baseRequest.NotAccessedBridges();
        
        foreach (Bridge _singleNotAccessBridge in notAccessedBridges)
        {
            UnityWebRequest _bridge_inBase_Get =
                UnityWebRequest.Get(_singleNotAccessBridge.FullAddress + "?fu=1&lvl=1");
            _bridge_inBase_Get.SetRequestHeader("X-M2M-Origin", "FU1_LVL1_Get");
            _bridge_inBase_Get.SetRequestHeader("X-M2M-RI", "REQ00002");
            _bridge_inBase_Get.SetRequestHeader("Accept", "application/json");

            yield return _bridge_inBase_Get.SendWebRequest();

            if (_bridge_inBase_Get.isNetworkError || _bridge_inBase_Get.isHttpError)
            {
                Debug.Log(_bridge_inBase_Get.error);
            }
            else
            {
                int _tempIndex = baseRequest.Bridges.IndexOf(_singleNotAccessBridge);
                
                string _tempResponse = _bridge_inBase_Get.downloadHandler.text;
                File.WriteAllText(_filePath + "\\bridge" + _tempIndex + ".json", _tempResponse);
                baseRequest.Bridges[_tempIndex].SerialiseResult_Bridge(_tempResponse);
            }
        }

        yield return GetHTTP_2Tier_MsgGroup_Request();

        Debug.Log("--> 1tier end.");
        */
    }
    
    public IEnumerator GetHTTP_2Tier_MsgGroup_Request()
    {
        /*
        //모든 bridge의 경우 (시간 오래 걸림)
        foreach (Bridge _singleBridge in baseRequest.Bridges)
        {
            List<MsgGroup> notAccessedMsgGroups = _singleBridge.NotAccessedMsgGroups();
            
            foreach (MsgGroup _singleMsgGroup in notAccessedMsgGroups)
            {
                UnityWebRequest _msgGroup_inSingleBridge_get =
                    UnityWebRequest.Get(_singleMsgGroup.FullAddress);
                _msgGroup_inSingleBridge_get.SetRequestHeader("X-M2M-Origin", "Get");
                _msgGroup_inSingleBridge_get.SetRequestHeader("X-M2M-RI", "REQ00003");
                _msgGroup_inSingleBridge_get.SetRequestHeader("Accept", "application/json");

                yield return _msgGroup_inSingleBridge_get.SendWebRequest();

                if (_msgGroup_inSingleBridge_get.isNetworkError || _msgGroup_inSingleBridge_get.isHttpError)
                {
                    Debug.Log(_msgGroup_inSingleBridge_get.error);
                }
                else
                {
                    int _tempBridgeIndex = baseRequest.Bridges.IndexOf(_singleBridge);
                    int _tempMsgGroupIndex = baseRequest.Bridges[_tempBridgeIndex].MsgGroups.IndexOf(_singleMsgGroup);
                    string _tempResponse = _msgGroup_inSingleBridge_get.downloadHandler.text;

                    //File.WriteAllText(_filePath + "\\msgGroup" + _tempBridgeIndex + "_" + _tempMsgGroupIndex + ".json", _tempResponse);
                    baseRequest.Bridges[_tempBridgeIndex].MsgGroups[_tempMsgGroupIndex].SerializeResult_MsgGroup(_tempResponse);

                }
            }
        }
        */
        
        List<MsgGroup> notAccessedMsgGroups = baseRequest.LastBridge.NotAccessedMsgGroups();

        //만약, 하나의 MsgGroup으로도 6종의 MavLinkMessage 가 충분히 채워진다면, 맨마지막 MsgGroup만 사용해보는 것도 나쁘지 않을듯.
        foreach (MsgGroup _singleMsgGroup in notAccessedMsgGroups)
        {
            UnityWebRequest _msgGroup_inSingleBridge_get =
                UnityWebRequest.Get(_singleMsgGroup.FullAddress);
            _msgGroup_inSingleBridge_get.SetRequestHeader("X-M2M-Origin", "Get");
            _msgGroup_inSingleBridge_get.SetRequestHeader("X-M2M-RI", "REQ00003");
            _msgGroup_inSingleBridge_get.SetRequestHeader("Accept", "application/json");

            yield return _msgGroup_inSingleBridge_get.SendWebRequest();

            if (_msgGroup_inSingleBridge_get.isNetworkError || _msgGroup_inSingleBridge_get.isHttpError)
            {
                Debug.Log(_msgGroup_inSingleBridge_get.error);
            }
            else
            {
                int _tempBridgeIndex = baseRequest.Bridges.IndexOf(baseRequest.LastBridge);
                int _tempMsgGroupIndex = baseRequest.Bridges[_tempBridgeIndex].MsgGroups.IndexOf(_singleMsgGroup);
                string _tempResponse = _msgGroup_inSingleBridge_get.downloadHandler.text;

                File.WriteAllText(_filePath + "\\msgGroup" + _tempBridgeIndex + "_" + _tempMsgGroupIndex + ".txt", _tempResponse);
                baseRequest.Bridges[_tempBridgeIndex].MsgGroups[_tempMsgGroupIndex].SerializeResult_MsgGroup(_tempResponse);

                var msgs = baseRequest.Bridges[_tempBridgeIndex].MsgGroups[_tempMsgGroupIndex].Msgs;
                
                foreach (MavMessage _singleMavMsg in msgs)
                {
                    MAVLinkMessage _tempMsg = _singleMavMsg.MsgMavLink;

                    _str_allMsg.Add(_singleMavMsg.MsgToString);

                    if (MAVLINK_MESSAGE_INFOS.GetMessageInfo(_tempMsg.msgid).name != null)
                    {
                        _str_knownMsg.Add(_singleMavMsg.MsgToString);
                        
                        switch (_tempMsg.msgid)
                        {
                            case 30:
                                var attitudeResult = convert_attitude(_tempMsg);
                                _list_attitude.Add(attitudeResult);
                                _str_attitude.Add(_singleMavMsg.MsgToString);
                                if (_list_attitude.Count>20) { _list_attitude.RemoveAt(0); }

                                _out_roll.Add(attitudeResult.roll);
                                _out_pitch.Add(attitudeResult.pitch);
                                _out_yaw.Add(attitudeResult.yaw);
                                if (_out_roll.Count > 20) { _out_roll.RemoveAt(0); }
                                if (_out_pitch.Count > 20) { _out_pitch.RemoveAt(0); }
                                if (_out_yaw.Count > 20) { _out_yaw.RemoveAt(0); }

                                break;

                            case 33:
                                var globalpositionResult = convert_globalposition(_tempMsg);
                                _list_globalPosition.Add(globalpositionResult);
                                _str_globalPosition.Add(_singleMavMsg.MsgToString);
                                if (_list_globalPosition.Count > 20) { _list_globalPosition.RemoveAt(0); }

                                _out_lat.Add(globalpositionResult.lat);
                                _out_lon.Add(globalpositionResult.lon);
                                _out_alt.Add(globalpositionResult.alt);
                                _out_alt_relative.Add(globalpositionResult.relative_alt);
                                if (_out_lat.Count > 20) { _out_lat.RemoveAt(0); }
                                if (_out_lon.Count > 20) { _out_lon.RemoveAt(0); }
                                if (_out_alt.Count > 20) { _out_alt.RemoveAt(0); }
                                if(_out_alt_relative.Count > 20) { _out_alt_relative.RemoveAt(0); }
                                break;

                            case 65:
                                var rcChannelsResult = convert_rcchannels(_tempMsg);
                                _list_rcChannels.Add(rcChannelsResult);
                                _str_rcChannels.Add(_singleMavMsg.MsgToString);
                                if (_list_rcChannels.Count > 20) { _list_rcChannels.RemoveAt(0); }

                                //blank

                                break;

                            case 74:
                                var vfrhudResult = convert_vfrhud(_tempMsg);
                                _list_vfrHud.Add(vfrhudResult);
                                _str_vfrHud.Add(_singleMavMsg.MsgToString);
                                if (_list_vfrHud.Count > 20) { _list_vfrHud.RemoveAt(0); }

                                _out_airspeed.Add(vfrhudResult.airspeed);
                                if(_out_airspeed.Count > 20) { _out_airspeed.RemoveAt(0); }
                                break;

                            case 136:
                                var terrainreportResult = convert_terrainreport(_tempMsg);
                                _list_terrain.Add(terrainreportResult);
                                _str_terrain.Add(_singleMavMsg.MsgToString);
                                if (_list_terrain.Count > 20) { _list_terrain.RemoveAt(0); }

                                _out_lat.Add(terrainreportResult.lat);
                                _out_lon.Add(terrainreportResult.lon);
                                if(_out_lat.Count > 20) { _out_lat.RemoveAt(0); }
                                if (_out_lon.Count > 20) { _out_lon.RemoveAt(0); }

                                break;

                            case 147:
                                var batterystatusResult = convert_batterystatus(_tempMsg);
                                _list_battery.Add(batterystatusResult);
                                _str_battery.Add(_singleMavMsg.MsgToString);
                                if (_list_battery.Count > 20) { _list_battery.RemoveAt(0); }

                                _out_remainbattery.Add(batterystatusResult.battery_remaining);
                                _out_currentbattery.Add(batterystatusResult.current_battery);
                                if(_out_remainbattery.Count>20) { _out_remainbattery.RemoveAt(0); }
                                if (_out_currentbattery.Count > 20) { _out_currentbattery.RemoveAt(0); }

                                break;
                        }
                    }
                    else
                    {
                        _str_unknownMsg.Add(_singleMavMsg.MsgToString);
                    }
                    _writeCntForFiles++;
                    if(_writeCntForFiles != 0 && (_writeCntForFiles % 100) == 0)
                    {
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_allMsg.txt", _str_allMsg.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_knownMsg.txt", _str_knownMsg.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_unknownMsg.txt", _str_unknownMsg.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_30_attitude.txt", _str_attitude.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_33_globalPosition.txt", _str_globalPosition.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_65_rcChannels.txt", _str_rcChannels.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_74_vfrHud.txt", _str_vfrHud.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_136_terrain.txt", _str_terrain.ToArray());
                        File.WriteAllLines(_filePath + "\\msgs\\" + (int)(_writeCntForFiles / 100) + "_147_battery.txt", _str_battery.ToArray());

                        _str_allMsg.Clear();
                        _str_knownMsg.Clear();
                        _str_unknownMsg.Clear();
                        _str_attitude.Clear();
                        _str_globalPosition.Clear();
                        _str_rcChannels.Clear();
                        _str_vfrHud.Clear();
                        _str_terrain.Clear();
                        _str_battery.Clear();
                    }
                }
            }
        }
        


        Debug.Log("--> --> 2tier end.");
    }
}
