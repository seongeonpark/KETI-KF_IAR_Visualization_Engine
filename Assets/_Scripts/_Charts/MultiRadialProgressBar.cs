using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RadialBarProperties
{
    public float Speed;
    public Color BarColor;
    public Color BarBackgroundColor;
    public Color FontColor;

}

public class MultiRadialProgressBar : MonoBehaviour
{
    public RadialBarProperties[] bars = new RadialBarProperties[4];

    [Header("Input Options: ")]
    [SerializeField] private float ReceivedData_1;
    [SerializeField] private float ReceivedData_2;
    [SerializeField] private float ReceivedData_3;
    [SerializeField] private float ReceivedData_4;
    [SerializeField] private float _speed = 70f;

    [Header("Type: ")]
    public GameObject FirstBar;
    public GameObject SecondBar;
    public GameObject ThirdBar;
    public GameObject ForthBar;

    private float amount_1st = 0.0f;
    private float amount_2nd = 0.0f;
    private float amount_3rd = 0.0f;
    private float amount_4th = 0.0f;
    private Image RPB_1st;
    private Image RPB_2nd;
    private Image RPB_3rd;
    private Image RPB_4th;
    private TextMeshProUGUI Txt_Indicator_1st;
    private TextMeshProUGUI Txt_Indicator_2nd;
    private TextMeshProUGUI Txt_Indicator_3rd;
    private TextMeshProUGUI Txt_Indicator_4th;


    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Start()
    {
        RPB_1st = FirstBar.transform.Find("ProgressBar").GetComponent<Image>();
        RPB_2nd = SecondBar.transform.Find("ProgressBar").GetComponent<Image>();
        RPB_3rd = ThirdBar.transform.Find("ProgressBar").GetComponent<Image>();
        RPB_4th = ForthBar.transform.Find("ProgressBar").GetComponent<Image>();

        Txt_Indicator_1st = FirstBar.transform.Find("ProgressBar").Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
        Txt_Indicator_2nd = SecondBar.transform.Find("ProgressBar").Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
        Txt_Indicator_3rd = ThirdBar.transform.Find("ProgressBar").Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
        Txt_Indicator_4th = ForthBar.transform.Find("ProgressBar").Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();

    }

    private void Update()
    {
        if (true)
        {
            // 1st
            if (0 <= amount_1st && amount_1st < ReceivedData_1)
            {
                if (FirstBar)
                {
                    amount_1st += _speed * Time.deltaTime;
                    RPB_1st.fillAmount = amount_1st / 100;
                    Txt_Indicator_1st.text = ((int)amount_1st).ToString() + "%";

                }
            }
            if (0 <= amount_2nd && amount_2nd < ReceivedData_2)
            {
                if (SecondBar)
                {
                    amount_2nd += _speed * Time.deltaTime;
                    RPB_2nd.fillAmount = amount_2nd / 100;
                    Txt_Indicator_2nd.text = ((int)amount_2nd).ToString() + "%";
                }
            }
            if (0 <= amount_3rd && amount_3rd < ReceivedData_3)
            {
                if (ThirdBar)
                {
                    amount_3rd += _speed * Time.deltaTime;
                    RPB_3rd.fillAmount = amount_3rd / 100;
                    Txt_Indicator_3rd.text = ((int)amount_3rd).ToString() + "%";

                }
            }
            if (0 <= amount_4th && amount_4th < ReceivedData_4)
            {
                if (ForthBar)
                {
                    amount_4th += _speed * Time.deltaTime;
                    RPB_4th.fillAmount = amount_4th / 100;
                    Txt_Indicator_4th.text = ((int)amount_4th).ToString() + "%";

                }
            }
        }
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    private void ResetData()
    {
        amount_1st = 0.0f;
        amount_2nd = 0.0f;
        amount_3rd = 0.0f;
        amount_4th = 0.0f;
    }

    #endregion  // PRIVATE_METHODS
}
