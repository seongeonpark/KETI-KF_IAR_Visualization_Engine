using System;
using UnityEngine;

public enum EDroneIoT
{
    // chart
    Battery_remain,
    Airspeed,
    HeadingIndicator,
    TurnCoordinator,
    Attitude_roll,
    Attitude_pitch,
    Battery_voltage,
    // text indicators
    Altitude,
    Latitude,
    Longitude,
    Accelerometer_x,
    Accelerometer_y,
    Accelerometer_z,
    Gyroscope_x,
    Gyroscope_y,
    Gyroscope_z,
    Magnetometer_x,
    Magnetometer_y,
    Magnetometer_z
}

public class Drone_Model : MonoBehaviour
{
    private readonly HTTP_Parser_v01 m_HTTP;
    private readonly MQTT_Parser_v011 m_MQTT;

    public Drone_Model(HTTP_Parser_v01 parser)
    {
        m_HTTP = parser;
    }

    public Drone_Model(MQTT_Parser_v011 parser)
    {
        m_MQTT = parser;
    }


    #region PUBLIC_METHODS

    public bool IsConnected(EDroneIoT chart)
    {
        if (m_HTTP)
        {
            return IsConnectedHTTP(chart);
        }
        if (m_MQTT)
        {
            return IsConnectedMQTT(chart);
        }
        else return false;
    }
    public float GetParsedDataOf(EDroneIoT chart)
    {
        if (m_HTTP)
        {
            return GetHTTPParsedDataOf(chart);
        }
        if (m_MQTT)
        {
            return GetMQTTParsedDataOf(chart);
        }
        else return 0f;
    }

    #endregion  // PUBLIC_METHODS


    #region PRIVATE_METHODS

    private bool IsConnectedHTTP(EDroneIoT chart)
    {
        switch (chart)
        {
            case EDroneIoT.Battery_remain:
                if (0 < m_HTTP._out_remainbattery.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Airspeed:
                if (0 < m_HTTP._out_airspeed.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.HeadingIndicator:
                if (0 < m_HTTP._out_yaw.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.TurnCoordinator:
                if (0 < m_HTTP._out_yaw.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Attitude_roll:
                if (0 < m_HTTP._out_roll.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Attitude_pitch:
                if (0 < m_HTTP._out_pitch.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Battery_voltage:
                if (0 < m_HTTP._out_currentbattery.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Altitude:
                if (0 < m_HTTP._out_alt.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Latitude:
                if (0 < m_HTTP._out_lat.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Longitude:
                if (0 < m_HTTP._out_lon.Count)
                {
                    return true;
                }
                else return false;
            default:
                break;
        }

        return false;
    }
    private bool IsConnectedMQTT(EDroneIoT chart)
    {
        switch (chart)
        {
            case EDroneIoT.Battery_remain:
                if (0 < m_MQTT._out_remainbattery.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Airspeed:
                if (0 < m_MQTT._out_airspeed.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.HeadingIndicator:
                if (0 < m_MQTT._out_yaw.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.TurnCoordinator:
                if (0 < m_MQTT._out_yaw.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Attitude_roll:
                if (0 < m_MQTT._out_roll.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Attitude_pitch:
                if (0 < m_MQTT._out_pitch.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Battery_voltage:
                if (0 < m_MQTT._out_currentbattery.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Altitude:
                if (0 < m_MQTT._out_alt.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Latitude:
                if (0 < m_MQTT._out_lat.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Longitude:
                if (0 < m_MQTT._out_lon.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Accelerometer_x:
                if (0 < m_MQTT._out_xacc.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Accelerometer_y:
                if (0 < m_MQTT._out_yacc.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Accelerometer_z:
                if (0 < m_MQTT._out_zacc.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Gyroscope_x:
                if (0 < m_MQTT._out_xgyro.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Gyroscope_y:
                if (0 < m_MQTT._out_ygyro.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Gyroscope_z:
                if (0 < m_MQTT._out_zgyro.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Magnetometer_x:
                if (0 < m_MQTT._out_xmag.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Magnetometer_y:
                if (0 < m_MQTT._out_ymag.Count)
                {
                    return true;
                }
                else return false;
            case EDroneIoT.Magnetometer_z:
                if (0 < m_MQTT._out_zmag.Count)
                {
                    return true;
                }
                else return false;
            default:
                break;
        }

        return false;
    }

    private float GetHTTPParsedDataOf(EDroneIoT chart)
    {
        float data;

        switch (chart)
        {
            case EDroneIoT.Battery_remain:
                data = Convert.ToSingle(m_HTTP._out_remainbattery[0]);
                break;
            case EDroneIoT.Airspeed:
                data = Convert.ToSingle(m_HTTP._out_airspeed[0]);
                break;
            case EDroneIoT.HeadingIndicator:
                data = Convert.ToSingle(m_HTTP._out_yaw[0]);
                break;
            case EDroneIoT.TurnCoordinator:
                data = Convert.ToSingle(m_HTTP._out_yaw[0]);
                break;
            case EDroneIoT.Attitude_roll:
                data = Convert.ToSingle(m_HTTP._out_roll[0]);
                break;
            case EDroneIoT.Attitude_pitch:
                data = Convert.ToSingle(m_HTTP._out_pitch[0]);
                break;
            case EDroneIoT.Battery_voltage:
                data = Convert.ToSingle(m_HTTP._out_currentbattery[0]);
                break;
            case EDroneIoT.Altitude:
                data = Convert.ToSingle(m_HTTP._out_alt[0]);
                break;
            case EDroneIoT.Latitude:
                data = Convert.ToSingle(m_HTTP._out_lat[0]);
                break;
            case EDroneIoT.Longitude:
                data = Convert.ToSingle(m_HTTP._out_lon[0]);
                break;
            default:
                data = 0;
                break;
        }

        return data;
    }
    private float GetMQTTParsedDataOf(EDroneIoT chart)
    {
        float data;

        switch (chart)
        {
            case EDroneIoT.Battery_remain:
                data = Convert.ToSingle(m_MQTT._out_remainbattery[m_MQTT._out_remainbattery.Count - 1]);
                break;
            case EDroneIoT.Airspeed:
                data = Convert.ToSingle(m_MQTT._out_airspeed[m_MQTT._out_airspeed.Count - 1]);
                break;
            case EDroneIoT.HeadingIndicator:
                data = Convert.ToSingle(m_MQTT._out_yaw[m_MQTT._out_yaw.Count - 1]);
                break;
            case EDroneIoT.TurnCoordinator:
                data = Convert.ToSingle(m_MQTT._out_yaw[m_MQTT._out_yaw.Count - 1]);
                break;
            case EDroneIoT.Attitude_roll:
                data = Convert.ToSingle(m_MQTT._out_roll[m_MQTT._out_roll.Count - 1]);
                break;
            case EDroneIoT.Attitude_pitch:
                data = Convert.ToSingle(m_MQTT._out_pitch[m_MQTT._out_pitch.Count - 1]);
                break;
            case EDroneIoT.Battery_voltage:
                data = Convert.ToSingle(m_MQTT._out_currentbattery[m_MQTT._out_currentbattery.Count - 1]);
                break;
            case EDroneIoT.Altitude:
                data = Convert.ToSingle(m_MQTT._out_alt[m_MQTT._out_alt.Count - 1]);
                break;
            case EDroneIoT.Latitude:
                data = Convert.ToSingle(m_MQTT._out_lat[m_MQTT._out_lat.Count - 1]);
                break;
            case EDroneIoT.Longitude:
                data = Convert.ToSingle(m_MQTT._out_lon[m_MQTT._out_lon.Count - 1]);
                break;
            case EDroneIoT.Accelerometer_x:
                data = Convert.ToSingle(m_MQTT._out_xacc[m_MQTT._out_xacc.Count - 1]);
                break;
            case EDroneIoT.Accelerometer_y:
                data = Convert.ToSingle(m_MQTT._out_yacc[m_MQTT._out_yacc.Count - 1]);
                break;
            case EDroneIoT.Accelerometer_z:
                data = Convert.ToSingle(m_MQTT._out_zacc[m_MQTT._out_zacc.Count - 1]);
                break;
            case EDroneIoT.Gyroscope_x:
                data = Convert.ToSingle(m_MQTT._out_xgyro[m_MQTT._out_xgyro.Count - 1]);
                break;
            case EDroneIoT.Gyroscope_y:
                data = Convert.ToSingle(m_MQTT._out_ygyro[m_MQTT._out_ygyro.Count - 1]);
                break;
            case EDroneIoT.Gyroscope_z:
                data = Convert.ToSingle(m_MQTT._out_zgyro[m_MQTT._out_zgyro.Count - 1]);
                break;
            case EDroneIoT.Magnetometer_x:
                data = Convert.ToSingle(m_MQTT._out_xmag[m_MQTT._out_xmag.Count - 1]);
                break;
            case EDroneIoT.Magnetometer_y:
                data = Convert.ToSingle(m_MQTT._out_ymag[m_MQTT._out_ymag.Count - 1]);
                break;
            case EDroneIoT.Magnetometer_z:
                data = Convert.ToSingle(m_MQTT._out_zmag[m_MQTT._out_zmag.Count - 1]);
                break;
            default:
                data = 0;
                break;
        }

        return data;
    }

    #endregion  // PRIVATE_METHODS
}
