using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public struct Axis
{
    public string Name;
    public float Min, Max;

    public Axis(string name, float min = 0, float max = 100)
    {
        this.Name = name;
        this.Min = min;
        this.Max = max;
    }
}

[System.Serializable]
public struct Label
{
    public string Name;
    public Sprite Sprite;
    public Color Color;
    public Label(string name, Sprite sprite, Color color)
    {
        this.Name = name;
        this.Sprite = sprite;
        this.Color = color;
    }
}

public class ScatterPlot : MonoBehaviour
{
    private enum EAxis
    {
        X, Y
    }

    #region Get data from server
    private VirtualServer mVirtualServer;

    #endregion

    [Header("Plot Settings")]
    [SerializeField] private Label[] Labels;
    [SerializeField] private float Size = 11f;
    [SerializeField] private int DelayTime = 1;

    [Header("Axis")]
    [SerializeField] private Axis XAxis;
    [SerializeField] private Axis YAxis;

    [Header("Label")]
    [SerializeField] private float FontSize = 250f;
    [SerializeField] private float Location = 1.2f;
    [SerializeField] private Color FontColor = new Color(255, 255, 255, 255);
    [SerializeField] private TMP_FontAsset FontAsset;

    private RectTransform mGraphContainer;
    private RectTransform mItemA;
    private RectTransform mItemB;
    private RectTransform mAxisTitleX;
    private RectTransform mAxisTitleY;
    private RectTransform mLabelTemplateX;
    private RectTransform mLabelTemplateY;
    private List<GameObject> mGameObjectLis;

    private void Start()
    {
        #region Server

        // Recieve data from server
        mVirtualServer = transform.GetComponent<VirtualServer>();
        #endregion

        mGraphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        //mItemA = mGraphContainer.Find("labelA").GetComponent<RectTransform>();
        //mItemB = mGraphContainer.Find("labelB").GetComponent<RectTransform>();
        //mLabelTemplateX = mGraphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        //mLabelTemplateY = mGraphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        //mAxisTitleX = mGraphContainer.Find("axisX").GetComponent<RectTransform>();
        //mAxisTitleY = mGraphContainer.Find("axisY").GetComponent<RectTransform>();

        mGameObjectLis = new List<GameObject>();

        // Label, Axis name
        //mItemA.GetComponent<Text>().text = Labels[0].Name;
        //mAxisTitleX.GetComponent<Text>().text = XAxis.Name;
        //mAxisTitleY.GetComponent<Text>().text = YAxis.Name;

        //CreateAxis(EAxis.X, 15);
        //CreateAxis(EAxis.Y, 11);


        StartCoroutine("DrawGraph");
    }


    private IEnumerator DrawGraph()
    {
        Debug.Log("DrawGraph coroutine started");
        while (true)
        {
            var xValue1 = mVirtualServer.IoTData[0];
            var yValue1 = mVirtualServer.IoTData[1];

            var xPos = TransformCoord(EAxis.X, xValue1);
            var yPos = TransformCoord(EAxis.Y, yValue1);

            CreateCircle(new Vector3(xPos, yPos), Labels[0].Sprite, Labels[0].Color);

            yield return new WaitForSeconds(DelayTime);
        }
    }

    private float Normalize(float serverdata, Axis axis)
    {
        var x = Mathf.Clamp(serverdata, axis.Min, axis.Max);
        float data = (x - axis.Max) / (axis.Max - axis.Min);

        return data;
    }


    private void CreateCircle(Vector2 anchoredPosition, Sprite sprite, Color color)
    {
        GameObject gameObject = new GameObject("Circle", typeof(Image));
        gameObject.transform.SetParent(mGraphContainer, false);
        gameObject.GetComponent<Image>().sprite = sprite;
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(Size, Size);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }

    private void CreateAxis(EAxis axis, int tickCount)
    {
        if (axis == EAxis.X)
        {
            float xGap = 50f;
            float xPos = 20f;
            for (int i = 0; i < tickCount; i++)
            {
                RectTransform labelX = Instantiate(mLabelTemplateX);
                labelX.SetParent(mGraphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(xPos + i * xGap, -1f * Location);
                labelX.GetComponent<Text>().text = i.ToString();
            }
        }
        else if (axis == EAxis.Y)
        {
            float yGap = 50f;
            float yPos = 20f;
            for (int i = 0; i < tickCount; i++)
            {
                RectTransform labelX = Instantiate(mLabelTemplateY);
                labelX.SetParent(mGraphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(-1 * Location, yPos + i * yGap);
                labelX.GetComponent<Text>().text = i.ToString();
            }
        }
    }

    private float TransformCoord(EAxis axis, float value)
    {
        float graphWidth = mGraphContainer.sizeDelta.x;
        float graphHeight = mGraphContainer.sizeDelta.y;

        float clampedValue = 0f;
        float pos = 0f;

        if (axis == EAxis.X)
        {
            clampedValue = Mathf.Clamp(value, XAxis.Min, XAxis.Max);
            pos = graphWidth * ((clampedValue - XAxis.Min) / (XAxis.Max - XAxis.Min));
        }
        else if (axis == EAxis.Y)
        {
            clampedValue = Mathf.Clamp(value, YAxis.Min, YAxis.Max);
            pos = graphHeight * ((clampedValue - YAxis.Min) / (YAxis.Max - YAxis.Min));
        }

        return pos;
    }
}
