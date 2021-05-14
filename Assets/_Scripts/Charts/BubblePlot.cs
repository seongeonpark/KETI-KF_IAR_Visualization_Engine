using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BubblePlot : MonoBehaviour
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
    private RectTransform mLabelTemplateX;
    private RectTransform mLabelTemplateY;

    [SerializeField] private int GridCount = 10;
    private float[] mColGrid;
    private float[] mRowGrid;
    private float[,] mColRowGrid;

    // bubble size
    private float sizeMax = 30f;
    private float sizeMin = 5f;

    private int mX_idx = 0;
    private int mY_idx = 0;
    private float mGraphWidth;
    private float mGraphHeight;

    private void Start()
    {
        #region Server

        // Recieve data from server
        mVirtualServer = transform.GetComponent<VirtualServer>();
        #endregion

        mGraphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        mGraphWidth = mGraphContainer.sizeDelta.x;
        mGraphHeight = mGraphContainer.sizeDelta.y;
        
        mColGrid = new float[GridCount];
        mRowGrid = new float[GridCount];
        mColRowGrid = new float[GridCount, GridCount];
        AssignGrid();

        StartCoroutine("DrawGraph");
    }

    private IEnumerator DrawGraph()
    {
        while (true)
        {
            var xValue1 = mVirtualServer.IoTData[0];
            var yValue1 = mVirtualServer.IoTData[1];

            var xPos = TransformCoord(EAxis.X, xValue1);
            var yPos = TransformCoord(EAxis.Y, yValue1);

            var gridPos = SearchGridPosition(xPos, yPos);
            var size = GetSize(mX_idx, mY_idx);
            CreateCircle(gridPos, Labels[0].Sprite, Labels[0].Color, size);

            yield return new WaitForSeconds(DelayTime);
        }
    }

    private float Normalize(float serverdata, Axis axis)
    {
        var x = Mathf.Clamp(serverdata, axis.Min, axis.Max);
        float data = (x - axis.Max) / (axis.Max - axis.Min);

        return data;
    }

    private void CreateCircle(Vector2 anchoredPosition, Sprite sprite, Color color, float size = 4f)
    {
        GameObject gameObject = new GameObject("Circle", typeof(Image));
        gameObject.transform.SetParent(mGraphContainer, false);
        gameObject.GetComponent<Image>().sprite = sprite;
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(size, size);
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
        float clampedValue = 0f;
        float pos = 0f;

        if (axis == EAxis.X)
        {
            clampedValue = Mathf.Clamp(value, XAxis.Min, XAxis.Max);
            pos = mGraphWidth * ((clampedValue - XAxis.Min) / (XAxis.Max - XAxis.Min));
        }
        else if (axis == EAxis.Y)
        {
            clampedValue = Mathf.Clamp(value, YAxis.Min, YAxis.Max);
            pos = mGraphHeight * ((clampedValue - YAxis.Min) / (YAxis.Max - YAxis.Min));
        }

        return pos;
    }

    private void AssignGrid()
    {
        float unitWidth = mGraphWidth / GridCount;
        float unitHeight = mGraphHeight / GridCount;
        
        for (int i = 0; i < GridCount; i++)
        {
            mColGrid[i] = unitWidth * (i + 1);
            mRowGrid[i] = unitHeight * (i + 1);
        }
    }

    private Vector3 SearchGridPosition(float x, float y)
    {
        bool foundX = false;
        bool foundY = false;

        int x_index = 0;
        int y_index = 0;

        // find location of grid
        for (int i = 0; i < GridCount; i++)
        {
            if (foundX == false && x <= mColGrid[i])
            {
                x_index = i;
                foundX = true;
            }

            if (foundY == false && y <= mRowGrid[i])
            {
                y_index = i;
                foundY = true;
            }
        }

        // calculate exact coordination in grid
        float unitWidth = mGraphWidth / GridCount;
        float unitHeight = mGraphHeight / GridCount;
        float xPos = mColGrid[x_index] - (unitWidth / 2);
        float yPos = mRowGrid[y_index] - (unitHeight / 2);

        // update gird size
        mColRowGrid[x_index, y_index]++;
        mX_idx = x_index;
        mY_idx = y_index;
        return new Vector3(xPos, yPos);
    }

    private float GetSize(int x_idx, int y_idx)
    {
        float size = sizeMin + (3f * mColRowGrid[x_idx, y_idx]);

        if (size >= sizeMax)
        {
            return sizeMax;
        }
        else return size;
    }

}
