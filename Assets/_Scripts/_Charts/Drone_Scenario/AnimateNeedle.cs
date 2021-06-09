using System.Collections;
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
        if (_IsTest)    // make test value
        {
            _TestValue = Random.Range(0, 360);
            mServerData = _TestValue;
        }
        else
        {
            mServerData = mParser._out_pitch[0] * 1000.0f;
        }

        mNeedleValue = Mathf.Clamp(mNeedleValue, _MinValue, _MaxValue);

        // Animation
        if (mPreNeedleValue <= mNeedleValue && mNeedleValue < mCurrentNeedleValue)
        {
            mNeedleValue += (_NeedleSpeed * Time.deltaTime);
        }
        else if (mPreNeedleValue >= mNeedleValue && mNeedleValue > mCurrentNeedleValue)
        {
            mNeedleValue -= (_NeedleSpeed * Time.deltaTime);
        }
        else
        {
            mNeedleValue = mCurrentNeedleValue;
        }

        _Needle.eulerAngles = new Vector3(0, 0, GetRotationOfValue());
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS

    private float GetRotationOfValue()
    {
        float totalAngleSize = Mathf.Abs(_AngleOfStart - _AngleOfEnd);
        float valueNormalized = mNeedleValue / _MaxValue;

        return _AngleOfStart - (valueNormalized * totalAngleSize);
    }

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            mPreNeedleValue = mCurrentNeedleValue;
            mCurrentNeedleValue = mServerData;
            
            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    #endregion  // PRIVATE_METHODS
}
