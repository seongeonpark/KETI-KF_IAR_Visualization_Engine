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

    private void Update()
    {
        
    }

    #endregion  // PRIVATE_VARIABLES


    #region UNITY_MONOBEHAVIOUR_METHODS

    private IEnumerator UpdateGraph()
    {
        while (true)
        {
            
            
            yield return new WaitForSeconds(_RefreshTime);
        }
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS

}
