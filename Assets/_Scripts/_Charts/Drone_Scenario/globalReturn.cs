using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalReturn : MonoBehaviour
{
    public GameObject _Http_Parser;
    private HTTP_Parser_v01 mTempComponent;

    // Start is called before the first frame update
    void Start()
    {
        mTempComponent = _Http_Parser.GetComponent<HTTP_Parser_v01>();
        //_tempComponent = GameObject.Find("HTTP_Parser").GetComponent<HTTP_Parser_v01>();

        StartCoroutine("Getter");

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("_out_lat:" + _tempComponent._out_lat[0]);
        //Debug.Log("_out_lon:" + _tempComponent._out_lon[1]);
    }

    private IEnumerator Getter()
    {
        while (true)
        {
            Debug.Log("_out_lat:" + mTempComponent._out_lat[0]);
            Debug.Log("_out_lon:" + mTempComponent._out_lon[1]);

            yield return new WaitForSeconds(1f);
        }
    }
}
