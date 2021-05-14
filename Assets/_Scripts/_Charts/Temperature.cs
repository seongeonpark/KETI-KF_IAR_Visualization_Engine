using System.Collections;
using TMPro;
using UnityEngine;

public class Temperature : MonoBehaviour
{
    #region Inspetor Options
    [Header("Indicator Options: ")]
    [SerializeField] private float _speed = 40f;
    #endregion

    private GetXML XML;
    private TextMeshProUGUI TextIndicator;

    private float _mAmount = 0f;

    private void Awake()
    {
        XML = GameObject.Find("Temperature").GetComponent<GetXML>();
        TextIndicator = gameObject.transform.Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        StartCoroutine("ResetData");
    }

    void Update()
    {
        if (TextIndicator)
        {
            if (0 <= _mAmount && _mAmount < XML.Data)
            {
                _mAmount += _speed * Time.deltaTime;

                TextIndicator.text = ((int)_mAmount).ToString() + "º";
            }
        }
    }

    private IEnumerator ResetData()
    {
        while (true)
        {
            _mAmount = 0f;

            yield return new WaitForSeconds(XML.RepeatTime);
        }
    }
}