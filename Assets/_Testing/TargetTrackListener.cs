using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Vuforia
{
    public class TargetTrackListener : MonoBehaviour
    {

        public bool visiToggle;

        [Header("IoT Object")]
        [SerializeField] private TargetTracker TargetObject;

        [Header("Chart script")]

        [SerializeField] private Gauge gauge;
        [SerializeField] private RadialProgressBar radialbar;
        [SerializeField] private PieGraph pieGraph;
        [SerializeField] private FunnelChart funnelChart;
        [SerializeField] private RadarChart radarChart;
        [SerializeField] private ScatterPlot scatterplot;
        [SerializeField] private BubblePlot bubbleplot;

        void Start()
        {
            //targetObject = GameObject.Find("9_IoT").GetComponent<TargetTracker>();
            TargetObject.OnTrackingFound += trackingFound;
            TargetObject.OnTrackingLost += trackingLost;
        }


        void onDestroy()
        {
            TargetObject.OnTrackingFound -= trackingFound;
            TargetObject.OnTrackingLost -= trackingLost;
        }


        public void trackingFound()
        {
            if (visiToggle)
            {
                //gameObject.SetActive(false);
                //visiToggle = false;
            }
            else
            {
                if (gauge != null)
                {
                    gauge.enabled = true;
                }
                if (radialbar != null)
                {
                    radialbar.enabled = true;
                }
                if (pieGraph != null)
                {
                    pieGraph.enabled = true;
                }
                if (funnelChart != null)
                {
                    funnelChart.enabled = true;
                }
                if (radarChart != null)
                {
                    radarChart.enabled = true;
                }
                if (scatterplot != null)
                {
                    scatterplot.enabled = true;
                }
                if (bubbleplot != null)
                {
                    bubbleplot.enabled = true;
                }
                
                gameObject.SetActive(true);
                visiToggle = true;
            }
        }

        public void trackingLost()
        {
            if (visiToggle)
            {
                gameObject.SetActive(false);
                if (gauge != null)
                {
                    gauge.enabled = false;
                }
                if (radialbar != null)
                {
                    radialbar.enabled = false;
                }
                if (pieGraph != null)
                {
                    pieGraph.enabled = false;
                }
                if (funnelChart != null)
                {
                    funnelChart.enabled = false;
                }
                if (radarChart != null)
                {
                    radarChart.enabled = false;
                }
                if (scatterplot != null)
                {
                    scatterplot.enabled = false;
                }
                if (bubbleplot != null)
                {
                    bubbleplot.enabled = false;
                }
                visiToggle = false;
            }
        }


    }

}