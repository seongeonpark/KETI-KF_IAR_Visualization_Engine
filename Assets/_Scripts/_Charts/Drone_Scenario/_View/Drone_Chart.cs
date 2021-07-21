using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using IARDrone;

public class Drone_Chart : MonoBehaviour
{
    #region Inspector_Window_Variables

    public EDroneIoT _IoTType;

    [Header("UI:")]
    [SerializeField] private RectTransform _Needle;
    [SerializeField] private float _NeedleSpeed = 150f;
    [SerializeField] private float _RefreshTime = 0.3f;

    [Space()]
    [SerializeField] private TextMeshProUGUI _Text;
    [SerializeField] private string _TextUnit;

    [Space()]
    [SerializeField] private float _MinValue;
    [SerializeField] private float _MaxValue;

    [Header("Round type info:")]
    [Space()]
    [SerializeField] private float _AngleOfStart;
    [SerializeField] private float _AngleOfEnd;

    [Header("Vertical type info:")]
    [Space()]
    [SerializeField] private float _PosOfStart;
    [SerializeField] private float _PosOfEnd;
    [SerializeField] private bool _IsVerticalType = false;

    #endregion  // Inspector_Window_Variables


    #region PRIVATE_VARIABLES

    private float m_IoTData = 0f;
    public float IoTData { get => m_IoTData; set => m_IoTData = value; }

    private float m_NeedleValue = 0f;
    private float m_PreviousValue = 0f;
    private float m_CurrentValue = 0f;

    private bool m_IsReady = false;

    #endregion  // PRIVATE_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    void Update()
    {
        // Animation
        if (m_PreviousValue <= m_NeedleValue && m_NeedleValue < m_CurrentValue)
        {
            m_NeedleValue += _NeedleSpeed * Time.deltaTime;
        }
        else if (m_CurrentValue < m_NeedleValue && m_NeedleValue <= m_PreviousValue)
        {
            m_NeedleValue -= _NeedleSpeed * Time.deltaTime;
        }
        else
        {
            m_NeedleValue = m_CurrentValue;
        }


        if (!_IsVerticalType)
        {
            _Needle.eulerAngles = new Vector3(0, 0, GetRotation(m_NeedleValue));
        }
        else if (_IsVerticalType)
        {
            _Needle.localPosition = new Vector3(0, GetPostion(m_NeedleValue), 0);
        }

        // Text indicator
        if (_Text)
        {
            _Text.text = string.Format("{0:N0} {1}", m_NeedleValue, _TextUnit);
        }

    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS



    #region PRIVATE_METHODS

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            // # 3. Pre-processing
            // .... initialization
            m_PreviousValue = m_CurrentValue;
            m_NeedleValue = m_PreviousValue;
            
            // .... normalization

            m_CurrentValue = Mathf.Clamp(m_IoTData, _MinValue, _MaxValue);

            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    private float GetRotation(float value)
    {
        float totalAngleSize = Mathf.Abs(_AngleOfStart - _AngleOfEnd);
        float valueNormalized = (value - _MinValue) / (_MaxValue - _MinValue);

        return _AngleOfStart - (valueNormalized * totalAngleSize);
    }

    private float GetPostion(float value)
    {
        float totalScopeSize = Mathf.Abs(_PosOfStart - _PosOfEnd);
        float valueNormalized = (value - _MinValue) / (_MaxValue - _MinValue);

        return _PosOfStart - (valueNormalized * totalScopeSize);
    }

    #endregion  // PRIVATE_METHODS
}
