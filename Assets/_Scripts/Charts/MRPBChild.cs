using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MRPBChild : MonoBehaviour
{
    #region Inspetor Opions
    [Header("Graph Options: ")]
    [SerializeField] private float _speed = 70f;
    #endregion

    public float value = 10f;
    private TextMeshProUGUI TextIndicator;
    
    private float _mAmount = 0f;

    //public GetXML XML;
    public Transform ProgressBar;

    private void Awake()
    {
        TextIndicator = gameObject.transform.Find("ProgressBar").Find("Txt_Indicator").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine("UpdateGraph");
    }

    void Update()
    {
        if (ProgressBar)
        {
            if (0 <= _mAmount && _mAmount < value)
            {
                _mAmount += _speed * Time.deltaTime;

                float norm = NormalizeGraph(_mAmount);
                ProgressBar.GetComponent<Image>().fillAmount = norm * 0.01f;

                TextIndicator.text = ((int)_mAmount).ToString() + "%";
            }
        }
    }

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
}