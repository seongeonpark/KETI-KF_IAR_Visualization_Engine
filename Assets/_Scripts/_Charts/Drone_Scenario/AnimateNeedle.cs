using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateNeedle : MonoBehaviour
{
    #region Inspector_Window_Variables

    [Header("UI:")]
    [SerializeField] private RectTransform _Needle;
    [SerializeField] private float _NeedleSpeed;
    [SerializeField] private float _MaxAngle;
    [SerializeField] private float _MinAngle;
    [Header("Data:")]
    [SerializeField] private float _RefreshTime;


    #endregion  // Inspector_Window_Variables


    #region PRIVATE_VARIABLES

    private float mNeedleValue = 0f;

    #endregion  // PRIVATE_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS
    
    private void Update()
    {
        if (_MinAngle <= mNeedleValue && mNeedleValue <= _MaxAngle)
        {
            mNeedleValue += _NeedleSpeed * Time.deltaTime;
        }
    }



    #endregion  // UNITY_MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS


    private IEnumerator UpdateGraph()
    {
        while (true)
        {


            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    #endregion  // PRIVATE_METHODS
}
