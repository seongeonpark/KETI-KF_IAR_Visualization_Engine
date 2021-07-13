using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    #region Inspector_Window_Variables

    [Header("Graph Options: ")]
    [SerializeField] private float _NeedleSpeed = 100f;


    [Space()]
    [SerializeField] private float _AngleOfStart;
    [SerializeField] private float _AngleOfEnd;

    [Space()]
    [SerializeField] private float _MinValue;
    [SerializeField] private float _MaxValue;
    #endregion  // Inspector_Window_Variables

    //private GetXML XML;

    #region PRIVATE_VARIABLES

    private Transform mNeedle;
    private TextMeshProUGUI mTextIndicator;
    private Text mTextStatus;
    private Text mTextLatency;

    private const float MIN_GAUGE_ANGLE = 200f;
    private const float MAX_GAUGE_ANGLE = 0f;
    private const float GAUGE_MAX = 200f;

    private float _mAmount = 0f;

    #endregion  // PRIVATE_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Awake()
    {
        //XML = GameObject.Find("Gauge").GetComponent<GetXML>();
        
        mNeedle = gameObject.transform.Find("Needle").transform;
        //Needle.eulerAngles = new Vector3(0, 0, MIN_GAUGE_ANGLE);
        
        var tf = gameObject.transform.Find("Indicator");
        mTextIndicator = tf.Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
        mTextStatus = tf.Find("Status").Find("Txt_Status").GetComponent<Text>();
        mTextLatency = tf.Find("Latency").Find("Txt_Latency").GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    private void Update()
    {
        if (mNeedle)
        {
            if (0 <= _mAmount && _mAmount < (28f * 2))
            {
                _mAmount += _NeedleSpeed * Time.deltaTime;

                mNeedle.eulerAngles = new Vector3(0, 0, GetRotation(_mAmount));

                mTextIndicator.text = ((int)_mAmount / 2).ToString() + "%";
            }
        }
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    //private float GetRotation()
    //{
    //    float totalAngleSize = MIN_GAUGE_ANGLE - MAX_GAUGE_ANGLE;
    //    float speedNormalized = _mAmount / GAUGE_MAX;

    //    return MIN_GAUGE_ANGLE - speedNormalized * totalAngleSize;
    //}

    private float GetRotation(float value)
    {
        float totalAngleSize = Mathf.Abs(_AngleOfStart - _AngleOfEnd);
        float valueNormalized = (value - _MinValue) / (_MaxValue - _MinValue);

        return _AngleOfStart - (valueNormalized * totalAngleSize);
    }

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            _mAmount = 0f;

            //yield return new WaitForSeconds(XML.RepeatTime);

            //TextLatency.text = XML.RepeatTime.ToString() + "s";
            //TextStatus.text = ShowCurrentState(XML.Data);
            //ChangeColor(XML.Data, TextStatus);

            yield return new WaitForSeconds(3);
            float repeatTime = 3f;
            mTextLatency.text = repeatTime.ToString() + "s";
            mTextStatus.text = ShowCurrentState(28f);
            ChangeColor(28f, mTextStatus);
        }
    }

    private void ChangeColor(float value, Text target)
    {
        if (0 <= value && value <= 25)
        {
            target.color = Color.green;
        }
        else if (25 < value && value <= 50)
        {
            target.color = new Color32(119, 171, 59, 255);
        }
        else if (50 < value && value <= 75)
        {
            target.color = Color.yellow;
        }
        else
        {
            target.color = Color.red;
        }
    }

    private string ShowCurrentState(float value)
    {
        if (0 <= value && value <= 25) return "안전";
        else if (25 < value && value <= 50) return "관심";
        else if (50 < value && value <= 75) return "주의";
        else return "심각";
    }

    #endregion  // PRIVATE_METHODS

}
