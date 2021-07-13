using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour
{
    #region Inspector_Window_Variables

    [Header("Graph Options: ")]
    [SerializeField] private float AnimationSpeed = 70f;
    [Header("Color")]
    [SerializeField] private Color Progressbar = new Color(0, 161, 255, 255);
    [SerializeField] private Color ProgressbarBackground; /*= new Color(190, 190, 190, 255);*/
    [SerializeField] private Color Center = new Color(255, 255, 255, 255);
    [SerializeField] private Color Font = new Color(0, 161, 255, 255);

    [SerializeField] private float VALUE = 13F;
    [SerializeField] private float _mAmount = 0f;
    //public GetXML XML;

    #endregion  // Inspector_Window_Variables


    #region PRIVATE_VARIABLES

    private Image mProgressBar;
    private Image mBarBackground;
    private Image mCenterBackground;
    private TextMeshProUGUI mTextIndicator;
    private Text mTextStatus;
    private Text mTextLatency;

    #endregion  // PRIVATE_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Awake()
    {
        //XML = GameObject.Find("RadialProgressBar").GetComponent<GetXML>();

        mProgressBar = transform.Find("ProgressBar").GetComponent<Image>();
        mProgressBar.color = Progressbar;
        mBarBackground = transform.Find("Background").GetComponent<Image>();
        mBarBackground.color = ProgressbarBackground;
        mCenterBackground = transform.Find("Center").GetComponent<Image>();
        mCenterBackground.color = Center;

        mTextIndicator = gameObject.transform.Find("Center").Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
        var tf = gameObject.transform.Find("Indicator");
        mTextStatus = tf.Find("Status").Find("Txt_Status").GetComponent<Text>();
        mTextLatency = tf.Find("Latency").Find("Txt_Latency").GetComponent<Text>();
    }
    
    private void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    private void Update()
    {
        if (0 <= _mAmount && _mAmount < VALUE)
        {
            _mAmount += AnimationSpeed * Time.deltaTime;

            mProgressBar.fillAmount = _mAmount * 0.01f;

            mTextIndicator.text = ((int)_mAmount).ToString() + "%";
        }
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            _mAmount = 0f;

            //yield return new WaitForSeconds(XML.RepeatTime);

            //TextLatency.text = XML.RepeatTime.ToString() + "s";
            //ChangeColor(XML.Data, ProgressBar.GetComponent<Image>());
            //ChangeColor(XML.Data, TextIndicator);
            //TextStatus.text = ShowCurrentState(XML.Data);
            //ChangeColor(XML.Data, TextStatus);

            yield return new WaitForSeconds(3f);
            float repeatTime = 3f;

            mTextLatency.text = "3s";

            ChangeColor(VALUE, mProgressBar.GetComponent<Image>());
            ChangeColor(VALUE, mTextIndicator);
            mTextStatus.text = ShowCurrentState(VALUE);
            ChangeColor(VALUE, mTextStatus);
        }
    }

    private void ChangeColor(float value, Image target)
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

    private void ChangeColor(float value, TextMeshProUGUI target)
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
