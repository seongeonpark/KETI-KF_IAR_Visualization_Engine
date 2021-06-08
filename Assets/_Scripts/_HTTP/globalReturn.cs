using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalReturn : MonoBehaviour
{
    public HTTP_Parser_v01 _tempComponent;

    // Start is called before the first frame update
    void Start()
    {
        _tempComponent = GameObject.Find("http_getter_1").GetComponent<HTTP_Parser_v01>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("_out_lat:" + _tempComponent._out_lat[0]);
        Debug.Log("_out_lon:" + _tempComponent._out_lon[1]);
    }
}
