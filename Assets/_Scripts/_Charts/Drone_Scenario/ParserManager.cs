using System;
using UnityEngine;

public enum EDroneChartType
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
    Longitude
}

public class ParserManager : MonoBehaviour
{
    private readonly HTTP_Parser_v01 m_Parser;

    public ParserManager(HTTP_Parser_v01 parser)
    {
        m_Parser = parser;
    }

    public bool CheckConnection(EDroneChartType chart)
    {
        switch (chart)
        {
            case EDroneChartType.Battery_remain:
                if (0 < m_Parser._out_remainbattery.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.Airspeed:
                if (0 < m_Parser._out_airspeed.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.HeadingIndicator:
                if (0 < m_Parser._out_yaw.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.TurnCoordinator:
                if (0 < m_Parser._out_yaw.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.Attitude_roll:
                if (0 < m_Parser._out_roll.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.Attitude_pitch:
                if (0 < m_Parser._out_pitch.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.Battery_voltage:
                if (0 < m_Parser._out_currentbattery.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.Altitude:
                if (0 < m_Parser._out_alt.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.Latitude:
                if (0 < m_Parser._out_lat.Count)
                {
                    return true;
                }
                else return false;
            case EDroneChartType.Longitude:
                if (0 < m_Parser._out_lon.Count)
                {
                    return true;
                }
                else return false;
            default:
                break;
        }

        return false;
    }

    public float GetParsingDataOf(EDroneChartType chart)
    {
        float data;

        switch (chart)
        {
            case EDroneChartType.Battery_remain:
                data = Convert.ToSingle(m_Parser._out_remainbattery[0]);
                break;
            case EDroneChartType.Airspeed:
                data = Convert.ToSingle(m_Parser._out_airspeed[0]);
                break;
            case EDroneChartType.HeadingIndicator:
                data = Convert.ToSingle(m_Parser._out_yaw[0]);
                break;
            case EDroneChartType.TurnCoordinator:
                data = Convert.ToSingle(m_Parser._out_yaw[0]);
                break;
            case EDroneChartType.Attitude_roll:
                data = Convert.ToSingle(m_Parser._out_roll[0]);
                break;
            case EDroneChartType.Attitude_pitch:
                data = Convert.ToSingle(m_Parser._out_pitch[0]);
                break;
            case EDroneChartType.Battery_voltage:
                data = Convert.ToSingle(m_Parser._out_currentbattery[0]);
                break;
            case EDroneChartType.Altitude:
                data = Convert.ToSingle(m_Parser._out_alt[0]);
                break;
            case EDroneChartType.Latitude:
                data = Convert.ToSingle(m_Parser._out_lat[0]);
                break;
            case EDroneChartType.Longitude:
                data = Convert.ToSingle(m_Parser._out_lon[0]);
                break;
            default:
                data = 0;
                break;
        }

        return data;
    }
}
