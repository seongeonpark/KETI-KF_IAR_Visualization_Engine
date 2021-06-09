using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drone_AppBar : MonoBehaviour
{
    [Header("UI:")]
    [SerializeField] private TextMeshProUGUI _ActivationTimeTMP;
    [SerializeField] private TextMeshProUGUI _DateTimeTMP;

    private DateTime mStartTime;

    void Start()
    {
        mStartTime = DateTime.Now;
    }

    void Update()
    {
        
        TimeSpan timeDiff = DateTime.Now - mStartTime;
        _ActivationTimeTMP.text = string.Format("Elapsed time {0:mm\\:ss}", timeDiff);
        _DateTimeTMP.text = DateTime.Now.ToString("yyyy/MM/dd   HH:mm:ss");
    }
}
