using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MRPBChild : MonoBehaviour
{
    #region Inspetor Opions
    public float Speed;
    public Color BarColor;
    public Color BarBackgroundColor;
    public Color FontColor;

    [Header("Graph Options: ")]
    [SerializeField] private float _speed = 70f;
    #endregion

    public float value = 10f;
    private TextMeshProUGUI mTextIndicator;
    
    private float _mAmount = 0f;

    //public GetXML XML;
    public Transform ProgressBar;
    private Image mBar;


    #region UNITY_MONOBEHAVIOUR_METHODS

    private void Awake()
    {
        mBar = transform.Find("ProgressBar").GetComponent<Image>();
        mTextIndicator = transform.Find("ProgressBar").Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    void Update()
    {
        if (mBar)
        {
            if (0 <= _mAmount && _mAmount < value)
            {
                _mAmount += _speed * Time.deltaTime;

                float norm = NormalizeGraph(_mAmount);
                mBar.fillAmount = norm * 0.01f;

                mTextIndicator.text = ((int)_mAmount).ToString() + "%";
            }
        }
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            _mAmount = 0f;

            yield return new WaitForSeconds(3f);
        }
    }

    private float NormalizeGraph(float value)
    {
        return 75 * (value * 0.01f);
    }
    
    #endregion  // PRIVATE_METHODS
}