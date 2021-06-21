using System.Collections;
using TMPro;
using UnityEngine;

public class AnimateNeedle : MonoBehaviour
{
    #region Inspector_Window_Variables

    [Header("UI:")]
    [SerializeField] private RectTransform _Needle;
    [SerializeField] private float _NeedleSpeed;
    [SerializeField] private float _AngleOfStart;
    [SerializeField] private float _AngleOfEnd;
    [SerializeField] private float _MinValue;
    [SerializeField] private float _MaxValue;
    [Space()]
    [SerializeField] private TextMeshProUGUI _Text;
    [Header("Data:")]
    [SerializeField] private GameObject _Http_Parser;
    [SerializeField] private float _RefreshTime;
    [Header("Test:")]
    [SerializeField] private bool _IsTest;
    [SerializeField] private float _TestValue;

    #endregion  // Inspector_Window_Variables


    #region PRIVATE_VARIABLES

    private HTTP_Parser_v01 mParser;

    private float mServerData = 0f;
    private float mNeedleValue = 0f;
    private float mPreNeedleValue = 0f;
    private float mCurrentNeedleValue = 0f;
    
    #endregion  // PRIVATE_VARIABLES
        

    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Start()
    {
        mParser = _Http_Parser.GetComponent<HTTP_Parser_v01>();
        mCurrentNeedleValue = mServerData;

        StartCoroutine("UpdateGraph");
        
    }

    private void Update()
    {
        //mServerData = mParser._out_pitch[0] * 10000.0f;
        //Debug.Log("server data: " + mServerData);

        if (_IsTest)    // make test value
        {
            _TestValue = Random.Range(0, 360);
            mServerData = _TestValue;
        }

        mNeedleValue = Mathf.Clamp(mServerData, _MinValue, _MaxValue);
        Debug.Log("needle value: " + mNeedleValue);


        // Animation
        if (mPreNeedleValue <= mNeedleValue && mNeedleValue < mCurrentNeedleValue)
        {
            mNeedleValue += (_NeedleSpeed * Time.deltaTime);

        }
        else if (mPreNeedleValue >= mNeedleValue && mNeedleValue > mCurrentNeedleValue)
        {
            mNeedleValue -= (_NeedleSpeed * Time.deltaTime);
            Debug.Log("animation needle value: " + mNeedleValue);

        }
        else
        {
            mNeedleValue = mCurrentNeedleValue;
        }

        _Needle.eulerAngles = new Vector3(0, 0, GetRotationOfValue(mNeedleValue));

        
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS

    private float GetRotationOfValue(float value)
    {
        float totalAngleSize = Mathf.Abs(_AngleOfStart - _AngleOfEnd);
        float valueNormalized = value / _MaxValue;

        return _AngleOfStart - (valueNormalized * totalAngleSize);
    }

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            mPreNeedleValue = mCurrentNeedleValue;
            mCurrentNeedleValue = mServerData;


            // Text indicator
            if (_Text != null)
            {
                _Text.text = string.Format("{0} %", mServerData);
            }

            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    #endregion  // PRIVATE_METHODS
}
