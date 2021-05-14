using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class _ID_xmlCon
{
    public string _requestID { get; set; }
    public float _con { get; set; }
    public string _time { get; set; }

    public override string ToString()
    {
        return "ID: " + _requestID + " // CON: " + _con + " // TIME: " + _time;
    }
}


