using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Reflection;

using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using M2MqttUnity;

using static MAVLink;

public class MQTT_Parser_v011 : MonoBehaviour
{
    private MqttClient client;
    string msg;

    public string test_bridge = "Dev_Tool_Test";
    //public string test_bridge = "KETI_Air_01";

    public int updateCnt;
    public int chkCnt = 1000;

    public byte basemodeChk;

    public List<ushort> _out_battery;

    public List<int> _out_alt, _out_alt_relative;
    public List<float> _out_airspeed;
    public List<float> _out_roll, _out_pitch, _out_yaw;
    public List<int> _out_lat, _out_lon;

    public List<short> _out_currentbattery;
    public List<sbyte> _out_remainbattery;

    public bool _out_arm_or_disarm;

    public List<short> _out_xacc, _out_yacc, _out_zacc;
    public List<short> _out_xgyro, _out_ygyro, _out_zgyro;
    public List<short> _out_xmag, _out_ymag, _out_zmag;

    string clientId;
    byte[] messageByteArray;

    bool newMsgFlag = false;

    // Use this for initialization
    void Start()
    {
        basemodeChk = 0;
        updateCnt = 0;
        _out_battery = new List<ushort>();

        _out_roll = new List<float>();
        _out_pitch = new List<float>();
        _out_yaw = new List<float>();

        _out_lat = new List<int>();
        _out_lon = new List<int>();
        _out_alt = new List<int>();
        _out_alt_relative = new List<int>();

        _out_airspeed = new List<float>();

        _out_currentbattery = new List<short>();
        _out_remainbattery = new List<sbyte>();

        _out_xacc = new List<short>(); _out_yacc = new List<short>(); _out_zacc = new List<short>();
        _out_xgyro = new List<short>(); _out_ygyro = new List<short>(); _out_zgyro = new List<short>();
        _out_xmag = new List<short>(); _out_ymag = new List<short>(); _out_zmag = new List<short>();

        // create client instance 
        client = new MqttClient("203.253.128.177", 1883, false, null, null, MqttSslProtocols.None);
        //client = new MqttClient("", 1883, false, null, null, MqttSslProtocols.None);
        //client.Settings.TimeoutOnReceiving = 100;
        clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        // register to message received 
        client.Subscribe(new string[] { "/Mobius/KETI_MUV/Drone_Data/" + test_bridge + "/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
    }
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        messageByteArray = e.Message;

        int _timeseries_limit = 3;

        MavMessage _tempMavMsg = new MavMessage(messageByteArray, DateTime.UtcNow.AddMinutes(-2));
        if (MAVLINK_MESSAGE_INFOS.GetMessageInfo(_tempMavMsg.MsgMavLink.msgid).name != null)
        {
            switch (_tempMavMsg.MsgMavLink.msgid)
            {
                case 0: //heartbeat
                    var heartbeatResult = convert_heartbeat(_tempMavMsg.MsgMavLink);

                    basemodeChk = heartbeatResult.base_mode;

                    _out_arm_or_disarm = GetBit(basemodeChk, 8);

                    break;

                case 1: //system_status
                    var systemstatusResult = convert_systemstatus(_tempMavMsg.MsgMavLink);

                    _out_battery.Add((ushort)(systemstatusResult.voltage_battery * 0.001f));
                    if (_out_battery.Count > _timeseries_limit) { _out_battery.RemoveAt(0); }

                    break;

                case 27: //raw_imu
                    var rawimuResult = convert_rawimu(_tempMavMsg.MsgMavLink);

                    _out_xacc.Add(rawimuResult.xacc);
                    _out_yacc.Add(rawimuResult.yacc);
                    _out_zacc.Add(rawimuResult.zacc);
                    if (_out_xacc.Count > _timeseries_limit) { _out_xacc.RemoveAt(0); }
                    if (_out_yacc.Count > _timeseries_limit) { _out_yacc.RemoveAt(0); }
                    if (_out_zacc.Count > _timeseries_limit) { _out_zacc.RemoveAt(0); }

                    _out_xgyro.Add(rawimuResult.xgyro);
                    _out_ygyro.Add(rawimuResult.ygyro);
                    _out_zgyro.Add(rawimuResult.zgyro);
                    if (_out_xgyro.Count > _timeseries_limit) { _out_xgyro.RemoveAt(0); }
                    if (_out_ygyro.Count > _timeseries_limit) { _out_ygyro.RemoveAt(0); }
                    if (_out_zgyro.Count > _timeseries_limit) { _out_zgyro.RemoveAt(0); }

                    _out_xmag.Add(rawimuResult.xmag);
                    _out_ymag.Add(rawimuResult.ymag);
                    _out_zmag.Add(rawimuResult.zmag);
                    if (_out_xmag.Count > _timeseries_limit) { _out_xmag.RemoveAt(0); }
                    if (_out_ymag.Count > _timeseries_limit) { _out_ymag.RemoveAt(0); }
                    if (_out_zmag.Count > _timeseries_limit) { _out_zmag.RemoveAt(0); }

                    break;

                case 30: //attitude
                    var attitudeResult = convert_attitude(_tempMavMsg.MsgMavLink);
                    Debug.Log(_tempMavMsg.MsgToString);
                    _out_roll.Add(attitudeResult.roll * 100.0f);
                    _out_pitch.Add(attitudeResult.pitch * 100.0f);
                    _out_yaw.Add(attitudeResult.yaw * 100.0f);
                    if (_out_roll.Count > _timeseries_limit) { _out_roll.RemoveAt(0); }
                    if (_out_pitch.Count > _timeseries_limit) { _out_pitch.RemoveAt(0); }
                    if (_out_yaw.Count > _timeseries_limit) { _out_yaw.RemoveAt(0); }

                    break;

                case 33:
                    var globalpositionResult = convert_globalposition(_tempMavMsg.MsgMavLink);

                    _out_lat.Add(globalpositionResult.lat);
                    _out_lon.Add(globalpositionResult.lon);
                    _out_alt.Add((int)(globalpositionResult.alt / 3.28f * 0.01f));
                    _out_alt_relative.Add(globalpositionResult.relative_alt);
                    if (_out_lat.Count > _timeseries_limit) { _out_lat.RemoveAt(0); }
                    if (_out_lon.Count > _timeseries_limit) { _out_lon.RemoveAt(0); }
                    if (_out_alt.Count > _timeseries_limit) { _out_alt.RemoveAt(0); }
                    if (_out_alt_relative.Count > _timeseries_limit) { _out_alt_relative.RemoveAt(0); }

                    break;

                case 74:
                    var vfrhudResult = convert_vfrhud(_tempMavMsg.MsgMavLink);

                    _out_airspeed.Add(vfrhudResult.airspeed * 10.0f);
                    if (_out_airspeed.Count > _timeseries_limit) { _out_airspeed.RemoveAt(0); }

                    break;

                case 147:
                    var batterystatusResult = convert_batterystatus(_tempMavMsg.MsgMavLink);

                    _out_remainbattery.Add(batterystatusResult.battery_remaining);
                    _out_currentbattery.Add(batterystatusResult.current_battery);
                    if (_out_remainbattery.Count > _timeseries_limit) { _out_remainbattery.RemoveAt(0); }
                    if (_out_currentbattery.Count > _timeseries_limit) { _out_currentbattery.RemoveAt(0); }

                    break;
            }

        }

        //         if (messageByteArray[5] == 30)
        //         {
        //             Debug.Log(messageByteArray[10] + "" + messageByteArray[11] + "" + messageByteArray[12] + "" + messageByteArray[13] + "");
        //         }

    }

    public bool GetBit(byte b, int bitnumbner)
    {
        return ((b & (1 << bitnumbner - 1)) != 0);
    }

    void OnApplicationQuit()
    {
        client.Unsubscribe(new string[] { "/Mobius/KETI_MUV/Drone_Data/" + test_bridge + "/#" });
        client.Disconnect();

#if !UNITY_EDITOR
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    private void Update()
    {
        int _timeseries_limit = 3;

        if (messageByteArray[5] == 30)
        {

            // Debug.Log(messageByteArray[10] + "" + messageByteArray[11] + "" + messageByteArray[12] + "" + messageByteArray[13] + "");

            //                 MavMessage _tempMavMsg = new MavMessage(messageByteArray, DateTime.UtcNow.AddMinutes(-2));
            //                 if (MAVLINK_MESSAGE_INFOS.GetMessageInfo(_tempMavMsg.MsgMavLink.msgid).name != null)
            //                 {
            //                     switch (_tempMavMsg.MsgMavLink.msgid)
            //                     {
            //                         case 0: //heartbeat
            //                             var heartbeatResult = convert_heartbeat(_tempMavMsg.MsgMavLink);
            // 
            //                             basemodeChk = heartbeatResult.base_mode;
            // 
            //                             _out_arm_or_disarm = GetBit(basemodeChk, 8);
            // 
            //                             break;
            // 
            //                         case 1: //system_status
            //                             var systemstatusResult = convert_systemstatus(_tempMavMsg.MsgMavLink);
            // 
            //                             _out_battery.Add(systemstatusResult.voltage_battery);
            //                             if (_out_battery.Count > _timeseries_limit) { _out_battery.RemoveAt(0); }
            // 
            //                             break;
            // 
            //                         case 27: //raw_imu
            //                             var rawimuResult = convert_rawimu(_tempMavMsg.MsgMavLink);
            // 
            //                             _out_xacc.Add(rawimuResult.xacc);
            //                             _out_yacc.Add(rawimuResult.yacc);
            //                             _out_zacc.Add(rawimuResult.zacc);
            //                             if (_out_xacc.Count > _timeseries_limit) { _out_xacc.RemoveAt(0); }
            //                             if (_out_yacc.Count > _timeseries_limit) { _out_yacc.RemoveAt(0); }
            //                             if (_out_zacc.Count > _timeseries_limit) { _out_zacc.RemoveAt(0); }
            // 
            //                             _out_xgyro.Add(rawimuResult.xgyro);
            //                             _out_ygyro.Add(rawimuResult.ygyro);
            //                             _out_zgyro.Add(rawimuResult.zgyro);
            //                             if (_out_xgyro.Count > _timeseries_limit) { _out_xgyro.RemoveAt(0); }
            //                             if (_out_ygyro.Count > _timeseries_limit) { _out_ygyro.RemoveAt(0); }
            //                             if (_out_zgyro.Count > _timeseries_limit) { _out_zgyro.RemoveAt(0); }
            // 
            //                             _out_xmag.Add(rawimuResult.xmag);
            //                             _out_ymag.Add(rawimuResult.ymag);
            //                             _out_zmag.Add(rawimuResult.zmag);
            //                             if (_out_xmag.Count > _timeseries_limit) { _out_xmag.RemoveAt(0); }
            //                             if (_out_ymag.Count > _timeseries_limit) { _out_ymag.RemoveAt(0); }
            //                             if (_out_zmag.Count > _timeseries_limit) { _out_zmag.RemoveAt(0); }
            // 
            //                             break;
            // 
            //                         case 30: //attitude
            //                             var attitudeResult = convert_attitude(_tempMavMsg.MsgMavLink);
            //                             Debug.Log(_tempMavMsg.MsgToString);
            //                             _out_roll.Add(attitudeResult.roll * 100.0f);
            //                             _out_pitch.Add(attitudeResult.pitch * 100.0f);
            //                             _out_yaw.Add(attitudeResult.yaw * 100.0f);
            //                             if (_out_roll.Count > _timeseries_limit) { _out_roll.RemoveAt(0); }
            //                             if (_out_pitch.Count > _timeseries_limit) { _out_pitch.RemoveAt(0); }
            //                             if (_out_yaw.Count > _timeseries_limit) { _out_yaw.RemoveAt(0); }
            // 
            //                             break;
            // 
            //                         case 33:
            //                             var globalpositionResult = convert_globalposition(_tempMavMsg.MsgMavLink);
            // 
            //                             _out_lat.Add(globalpositionResult.lat);
            //                             _out_lon.Add(globalpositionResult.lon);
            //                             _out_alt.Add(globalpositionResult.alt);
            //                             _out_alt_relative.Add(globalpositionResult.relative_alt);
            //                             if (_out_lat.Count > _timeseries_limit) { _out_lat.RemoveAt(0); }
            //                             if (_out_lon.Count > _timeseries_limit) { _out_lon.RemoveAt(0); }
            //                             if (_out_alt.Count > _timeseries_limit) { _out_alt.RemoveAt(0); }
            //                             if (_out_alt_relative.Count > _timeseries_limit) { _out_alt_relative.RemoveAt(0); }
            // 
            //                             break;
            // 
            //                         case 74:
            //                             var vfrhudResult = convert_vfrhud(_tempMavMsg.MsgMavLink);
            // 
            //                             _out_airspeed.Add(vfrhudResult.airspeed);
            //                             if (_out_airspeed.Count > _timeseries_limit) { _out_airspeed.RemoveAt(0); }
            // 
            //                             break;
            // 
            //                         case 147:
            //                             var batterystatusResult = convert_batterystatus(_tempMavMsg.MsgMavLink);
            // 
            //                             _out_remainbattery.Add(batterystatusResult.battery_remaining);
            //                             _out_currentbattery.Add(batterystatusResult.current_battery);
            //                             if (_out_remainbattery.Count > _timeseries_limit) { _out_remainbattery.RemoveAt(0); }
            //                             if (_out_currentbattery.Count > _timeseries_limit) { _out_currentbattery.RemoveAt(0); }
            // 
            //                             break;
            //                     }
            //                     
            //                 }
            //             MavMessage _tempMavMsg = new MavMessage(messageByteArray, DateTime.UtcNow.AddMinutes(-2));
            //             
            //             if (MAVLINK_MESSAGE_INFOS.GetMessageInfo(_tempMavMsg.MsgMavLink.msgid).name != null)
            //             {
            //                 switch (_tempMavMsg.MsgMavLink.msgid)
            //                 {
            //                     case 0: //heartbeat
            //                         var heartbeatResult = convert_heartbeat(_tempMavMsg.MsgMavLink);
            //                         
            //                         basemodeChk = heartbeatResult.base_mode;
            // 
            //                         _out_arm_or_disarm = GetBit(basemodeChk, 8);
            // 
            //                         break;
            // 
            //                     case 1: //system_status
            //                         var systemstatusResult = convert_systemstatus(_tempMavMsg.MsgMavLink);
            // 
            //                         _out_battery.Add(systemstatusResult.voltage_battery);
            //                         if (_out_battery.Count > _timeseries_limit) { _out_battery.RemoveAt(0); }
            // 
            //                         break;
            // 
            //                     case 27: //raw_imu
            //                         var rawimuResult = convert_rawimu(_tempMavMsg.MsgMavLink);
            // 
            //                         _out_xacc.Add(rawimuResult.xacc);
            //                         _out_yacc.Add(rawimuResult.yacc);
            //                         _out_zacc.Add(rawimuResult.zacc);
            //                         if (_out_xacc.Count > _timeseries_limit) { _out_xacc.RemoveAt(0); }
            //                         if (_out_yacc.Count > _timeseries_limit) { _out_yacc.RemoveAt(0); }
            //                         if (_out_zacc.Count > _timeseries_limit) { _out_zacc.RemoveAt(0); }
            // 
            //                         _out_xgyro.Add(rawimuResult.xgyro);
            //                         _out_ygyro.Add(rawimuResult.ygyro);
            //                         _out_zgyro.Add(rawimuResult.zgyro);
            //                         if (_out_xgyro.Count > _timeseries_limit) { _out_xgyro.RemoveAt(0); }
            //                         if (_out_ygyro.Count > _timeseries_limit) { _out_ygyro.RemoveAt(0); }
            //                         if (_out_zgyro.Count > _timeseries_limit) { _out_zgyro.RemoveAt(0); }
            // 
            //                         _out_xmag.Add(rawimuResult.xmag);
            //                         _out_ymag.Add(rawimuResult.ymag);
            //                         _out_zmag.Add(rawimuResult.zmag);
            //                         if (_out_xmag.Count > _timeseries_limit) { _out_xmag.RemoveAt(0); }
            //                         if (_out_ymag.Count > _timeseries_limit) { _out_ymag.RemoveAt(0); }
            //                         if (_out_zmag.Count > _timeseries_limit) { _out_zmag.RemoveAt(0); }
            // 
            //                         break;
            // 
            //                     case 30: //attitude
            //                         var attitudeResult = convert_attitude(_tempMavMsg.MsgMavLink);
            //                         Debug.Log(_tempMavMsg.MsgToString);
            //                         _out_roll.Add(attitudeResult.roll * 100.0f);
            //                         _out_pitch.Add(attitudeResult.pitch * 100.0f);
            //                         _out_yaw.Add(attitudeResult.yaw * 100.0f);
            //                         if (_out_roll.Count > _timeseries_limit) { _out_roll.RemoveAt(0); }
            //                         if (_out_pitch.Count > _timeseries_limit) { _out_pitch.RemoveAt(0); }
            //                         if (_out_yaw.Count > _timeseries_limit) { _out_yaw.RemoveAt(0); }
            // 
            //                         break;
            // 
            //                     case 33:
            //                         var globalpositionResult = convert_globalposition(_tempMavMsg.MsgMavLink);
            // 
            //                         _out_lat.Add(globalpositionResult.lat);
            //                         _out_lon.Add(globalpositionResult.lon);
            //                         _out_alt.Add(globalpositionResult.alt);
            //                         _out_alt_relative.Add(globalpositionResult.relative_alt);
            //                         if (_out_lat.Count > _timeseries_limit) { _out_lat.RemoveAt(0); }
            //                         if (_out_lon.Count > _timeseries_limit) { _out_lon.RemoveAt(0); }
            //                         if (_out_alt.Count > _timeseries_limit) { _out_alt.RemoveAt(0); }
            //                         if (_out_alt_relative.Count > _timeseries_limit) { _out_alt_relative.RemoveAt(0); }
            // 
            //                         break;
            // 
            //                     case 74:
            //                         var vfrhudResult = convert_vfrhud(_tempMavMsg.MsgMavLink);
            // 
            //                         _out_airspeed.Add(vfrhudResult.airspeed);
            //                         if (_out_airspeed.Count > _timeseries_limit) { _out_airspeed.RemoveAt(0); }
            // 
            //                         break;
            // 
            //                     case 147:
            //                         var batterystatusResult = convert_batterystatus(_tempMavMsg.MsgMavLink);
            // 
            //                         _out_remainbattery.Add(batterystatusResult.battery_remaining);
            //                         _out_currentbattery.Add(batterystatusResult.current_battery);
            //                         if (_out_remainbattery.Count > _timeseries_limit) { _out_remainbattery.RemoveAt(0); }
            //                         if (_out_currentbattery.Count > _timeseries_limit) { _out_currentbattery.RemoveAt(0); }
            // 
            //                         break;
            //                 }
        }
        newMsgFlag = false;

        updateCnt++;
        if (updateCnt != 0 && updateCnt % chkCnt == 0)
        {
            Debug.Log("----------------------------------------------->> change");

            client.Unsubscribe(new string[] { "/Mobius/KETI_MUV/Drone_Data/" + test_bridge + "/#" });
            client.Disconnect();

            client = null;

            client = new MqttClient("203.253.128.177", 1883, false, null, null, MqttSslProtocols.None);
            client.Settings.TimeoutOnReceiving = 100;
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.Connect(clientId);

            client.Subscribe(new string[] { "/Mobius/KETI_MUV/Drone_Data/" + test_bridge + "/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            updateCnt = 0;
        }

        switch (Input.inputString)
        {
            case "z":
                client.Unsubscribe(new string[] { "/Mobius/KETI_MUV/Drone_Data/" + test_bridge + "/#" });

                Debug.Log("unsubscribe");

                break;

            case "x":
                client.Subscribe(new string[] { "/Mobius/KETI_MUV/Drone_Data/" + test_bridge + "/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                Debug.Log("subscribe");
                break;

            case "q":
                client.Disconnect();
                Debug.Log("end");
                break;

            case "r":
                client = new MqttClient("203.253.128.177", 1883, false, null, null, MqttSslProtocols.None);

                client.Settings.TimeoutOnReceiving = 100;
                client.Connect(clientId);
                Debug.Log("Connect");
                break;
        }
    }

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
    public mavlink_heartbeat_t convert_heartbeat(MAVLinkMessage _msg)
    {
        uint custom_mode = 0;
        byte type = 0;
        byte autopilot = 0;
        byte base_mode = 0;
        byte system_status = 0;
        byte mavlink_version = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "custom_mode":
                    custom_mode = (uint)fieldValue;
                    break;
                case "type":
                    type = (byte)fieldValue;
                    break;
                case "autopilot":
                    autopilot = (byte)fieldValue;
                    break;
                case "base_mode":
                    base_mode = (byte)fieldValue;
                    break;
                case "system_status":
                    system_status = (byte)fieldValue;
                    break;
                case "mavlink_version":
                    mavlink_version = (byte)fieldValue;
                    break;
            }
        }

        return new mavlink_heartbeat_t(custom_mode, type, autopilot, base_mode, system_status, mavlink_version);

    }
    public mavlink_sys_status_t convert_systemstatus(MAVLinkMessage _msg)
    {
        uint onboard_control_sensors_present = 0;
        uint onboard_control_sensors_enabled = 0;
        uint onboard_control_sensors_health = 0;
        ushort load = 0;
        ushort voltage_battery = 0;
        short current_battery = 0;
        ushort drop_rate_comm = 0;
        ushort errors_comm = 0;
        ushort errors_count1 = 0;
        ushort errors_count2 = 0;
        ushort errors_count3 = 0;
        ushort errors_count4 = 0;
        sbyte battery_remaining = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "onboard_control_sensors_present":
                    onboard_control_sensors_present = (uint)fieldValue;
                    break;
                case "onboard_control_sensors_enabled":
                    onboard_control_sensors_enabled = (uint)fieldValue;
                    break;
                case "onboard_control_sensors_health":
                    onboard_control_sensors_health = (uint)fieldValue;
                    break;
                case "load":
                    load = (ushort)fieldValue;
                    break;
                case "voltage_battery":
                    voltage_battery = (ushort)fieldValue;
                    break;
                case "current_battery":
                    current_battery = (short)fieldValue;
                    break;
                case "drop_rate_comm":
                    drop_rate_comm = (ushort)fieldValue;
                    break;
                case "errors_comm":
                    errors_comm = (ushort)fieldValue;
                    break;
                case "errors_count1":
                    errors_count1 = (ushort)fieldValue;
                    break;
                case "errors_count2":
                    errors_count2 = (ushort)fieldValue;
                    break;
                case "errors_count3":
                    errors_count3 = (ushort)fieldValue;
                    break;
                case "errors_count4":
                    errors_count4 = (ushort)fieldValue;
                    break;
                case "battery_remaining":
                    battery_remaining = (sbyte)fieldValue;
                    break;
            }
        }

        return new mavlink_sys_status_t(
            onboard_control_sensors_present, onboard_control_sensors_enabled, onboard_control_sensors_health, load,
            voltage_battery, current_battery, drop_rate_comm, errors_comm, errors_count1, errors_count2, errors_count3, errors_count4,
            battery_remaining);
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
    public mavlink_raw_imu_t convert_rawimu(MAVLinkMessage _msg)
    {
        ulong time_usec = 0;
        short xacc = 0;
        short yacc = 0;
        short zacc = 0;
        short xgyro = 0;
        short ygyro = 0;
        short zgyro = 0;
        short xmag = 0;
        short ymag = 0;
        short zmag = 0;

        var _data = _msg.data;

        foreach (var _tempField in _data.GetType().GetFields())
        {
            FieldInfo _tempFildInfo = _tempField;
            string fieldName = _tempFildInfo.Name;
            var fieldValue = _tempFildInfo.GetValue(_data);

            switch (fieldName)
            {
                case "time_usec":
                    time_usec = (ulong)fieldValue;
                    break;
                case "xacc":
                    xacc = (short)fieldValue;
                    break;
                case "yacc":
                    yacc = (short)fieldValue;
                    break;
                case "zacc":
                    zacc = (short)fieldValue;
                    break;
                case "xgyro":
                    xgyro = (short)fieldValue;
                    break;
                case "ygyro":
                    ygyro = (short)fieldValue;
                    break;
                case "zgyro":
                    zgyro = (short)fieldValue;
                    break;
                case "xmag":
                    xmag = (short)fieldValue;
                    break;
                case "ymag":
                    ymag = (short)fieldValue;
                    break;
                case "zmag":
                    zmag = (short)fieldValue;
                    break;
            }
        }

        return new mavlink_raw_imu_t(time_usec, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag);
    }

}
