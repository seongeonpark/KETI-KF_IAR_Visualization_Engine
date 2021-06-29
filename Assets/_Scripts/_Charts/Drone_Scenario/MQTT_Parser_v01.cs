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

public class MQTT_Parser_v01 : MonoBehaviour
{
    private MqttClient client;
    string msg;

    private List<mavlink_attitude_t> _list_attitude;
    private List<mavlink_global_position_int_t> _list_globalPosition;
    private List<mavlink_heartbeat_t> _list_heartbeat;
    private List<mavlink_sys_status_t> _list_systemStatus;

    public byte basemodeChk;

    public List<ushort> _out_battery;

    public List<float> _out_roll;
    public List<float> _out_pitch;
    public List<float> _out_yaw;

    public List<int> _out_lat;
    public List<int> _out_lon;
    public List<int> _out_alt;
    public List<int> _out_alt_relative;

    public bool _out_arm_or_disarm;

    public string test_bridge = "Dev_Tool_Test";

    string clientId;
    byte[] messageByteArray;
    byte[] msgBA_before;

    bool newMsgFlag = false;

    

    public int updateCnt;
    public int chkCnt = 1000;

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

        _list_attitude = new List<mavlink_attitude_t>();
        _list_heartbeat = new List<mavlink_heartbeat_t>();
        _list_systemStatus = new List<mavlink_sys_status_t>();
        _list_globalPosition = new List<mavlink_global_position_int_t>();

        // create client instance 
        client = new MqttClient("203.253.128.177", 1883, false, null, null, MqttSslProtocols.None);
        //client = new MqttClient("", 1883, false, null, null, MqttSslProtocols.None);
        client.Settings.TimeoutOnReceiving = 100;
        clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        // register to message received 
        client.Subscribe(new string[] { "/Mobius/KETI_MUV/Drone_Data/" + test_bridge + "/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
    }
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        messageByteArray = e.Message;
        newMsgFlag = true;
    }

    public bool GetBit(byte b, int bitnumbner)
    {
        return ((b & (1 << bitnumbner - 1)) != 0);
    }

    private void Update()
    {
        if(newMsgFlag)
        {
            MavMessage _tempMavMsg = new MavMessage(messageByteArray, DateTime.UtcNow.AddMinutes(-2));
            //Debug.Log(_tempMavMsg.MsgToString);
            if (MAVLINK_MESSAGE_INFOS.GetMessageInfo(_tempMavMsg.MsgMavLink.msgid).name != null)
            {
                switch (_tempMavMsg.MsgMavLink.msgid)
                {
                    case 0: //heartbeat
                        var heartbeatResult = convert_heartbeat(_tempMavMsg.MsgMavLink);
                        _list_heartbeat.Add(heartbeatResult);

                        if (_list_heartbeat.Count > 10) { _list_heartbeat.RemoveAt(0); }
                        
                        basemodeChk = heartbeatResult.base_mode;

                        _out_arm_or_disarm = GetBit(basemodeChk, 8);

                        break;

                    case 1: //system_status
                        var systemstatusResult = convert_systemstatus(_tempMavMsg.MsgMavLink);
                        _list_systemStatus.Add(systemstatusResult);
                        if (_list_heartbeat.Count > 10) { _list_systemStatus.RemoveAt(0); }

                        _out_battery.Add(systemstatusResult.voltage_battery);
                        if (_out_battery.Count > 10) { _out_battery.RemoveAt(0); }

                        break;

                    case 30: //attitude
                        var attitudeResult = convert_attitude(_tempMavMsg.MsgMavLink);
                        _list_attitude.Add(attitudeResult);
                        if (_list_attitude.Count > 10) { _list_attitude.RemoveAt(0); }

                        _out_roll.Add(attitudeResult.roll);
                        _out_pitch.Add(attitudeResult.pitch);
                        _out_yaw.Add(attitudeResult.yaw);
                        if (_out_roll.Count > 10) { _out_roll.RemoveAt(0); }
                        if (_out_pitch.Count > 10) { _out_pitch.RemoveAt(0); }
                        if (_out_yaw.Count > 10) { _out_yaw.RemoveAt(0); }

                        break;

                    case 33:
                        var globalpositionResult = convert_globalposition(_tempMavMsg.MsgMavLink);
                        _list_globalPosition.Add(globalpositionResult);
                        if (_list_globalPosition.Count > 10) { _list_globalPosition.RemoveAt(0); }

                        _out_lat.Add(globalpositionResult.lat);
                        _out_lon.Add(globalpositionResult.lon);
                        _out_alt.Add(globalpositionResult.alt);
                        _out_alt_relative.Add(globalpositionResult.relative_alt);
                        if (_out_lat.Count > 10) { _out_lat.RemoveAt(0); }
                        if (_out_lon.Count > 10) { _out_lon.RemoveAt(0); }
                        if (_out_alt.Count > 10) { _out_alt.RemoveAt(0); }
                        if (_out_alt_relative.Count > 10) { _out_alt_relative.RemoveAt(0); }
                        break;
                }
            }
            newMsgFlag = false;
        }
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

    /*public mavlink_battery_status_t convert_batterystatus(MAVLinkMessage _msg)
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
    }*/


}
