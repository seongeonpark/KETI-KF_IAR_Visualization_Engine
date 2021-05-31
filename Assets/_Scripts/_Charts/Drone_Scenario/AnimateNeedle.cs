using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float _RefreshTime;
    [Space]
    [SerializeField] private float _TestValue;


    #endregion  // Inspector_Window_Variables


    #region PRIVATE_VARIABLES

    private float mServerData = 0f;
    private float mNeedleValue = 0f;

    #endregion  // PRIVATE_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Awake()
    {
        mServerData = _TestValue;
    }

    private void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    private void Update()
    {
        mNeedleValue = Mathf.Clamp(mNeedleValue, _MinValue, _MaxValue);

        if (_MinValue <= mNeedleValue && mNeedleValue <= mServerData)
        {
            mNeedleValue += _NeedleSpeed * Time.deltaTime;
            Debug.Log(mNeedleValue);

            _Needle.eulerAngles = new Vector3(0, 0, GetRotationOfValue());
        }
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
            mNeedleValue = 0f;

            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    #endregion  // PRIVATE_METHODS
}
