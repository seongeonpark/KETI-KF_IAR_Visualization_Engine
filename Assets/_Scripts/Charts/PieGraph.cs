using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Sensor
{
    public string name;
    public float min, max;

    public Sensor(string name = "iot", float min = 0, float max = 100)
    {
        this.name = name;
        this.min = min;
        this.max = max;
    }
}

public class PieGraph : MonoBehaviour
{
    #region Get data from server
    private VirtualServer mVirtualServer;

    [Header("Server")]
    public GetXMLs XML;
    #endregion


    [Header("Chart Settings")]
    [SerializeField] private Image WedgePrefab;
    [SerializeField] private float Margin;
    [Space]
    [SerializeField] private Sensor[] Sensors;
    [Space]
    [SerializeField] private Color[] WedgeColors;

    [Header("Label")]
    [SerializeField] private float FontSize = 250f;
    [SerializeField] private float Distance = 130f;
    [SerializeField] private TMP_FontAsset FontAsset;
    [SerializeField] private float Padding;


    private RectTransform mChartContainer;
    private RectTransform mTextContainer;
    private Image[] mWedges;
    private TextMeshPro[] mTexts;
    private float[] mY_Coordinates;

    private float mLabelHeight;
    private float mVerticalMovement;
    private const float CHART_DEGREE = 360f;
    private const float CHART_SIZE = 500f;

    void Start()
    {
        #region Server

        // Recieve data from server
        mVirtualServer = transform.GetComponent<VirtualServer>();
        #endregion

        mLabelHeight = CHART_SIZE / Sensors.Length;
        mVerticalMovement = CHART_SIZE * 0.3f;

        mWedges = new Image[Sensors.Length];
        mTexts = new TextMeshPro[Sensors.Length];

        mY_Coordinates = new float[Sensors.Length];
        for (int i = 0; i < Sensors.Length; i++)
        {
            mY_Coordinates[i] = mVerticalMovement - (Padding * i);
        }


        mChartContainer = transform.Find("ChartContainer").GetComponent<RectTransform>();
        mTextContainer = transform.Find("TextContainer").GetComponent<RectTransform>();

        float x = mChartContainer.anchoredPosition.x;

        for (int i = 0; i < Sensors.Length; i++)
        {
            // wedges
            Image wedge = Instantiate(WedgePrefab) as Image;
            wedge.transform.SetParent(mChartContainer, false);
            mWedges[i] = wedge.transform.GetComponent<Image>();

            // labels
            GameObject label = new GameObject("label", typeof(TextMeshPro));
            label.transform.SetParent(mTextContainer, false);
            mTexts[i] = label.transform.GetComponent<TextMeshPro>();

            float y = mY_Coordinates[i];
            CreateLabel(mTexts[i], Sensors[i].name, FontSize, x + Distance, y, FontAsset, WedgeColors[i]);
        }
    }

    private void Update()
    {
        //float[] IoTData = mVirtualServer.IoTData;
        float[] IoTData = Normalize(mVirtualServer.IoTData, Sensors);
        //float[] IoTData = Normalize(XML.Data, Sensors);
        
        float total = 0f;
        float zRotation = 0f;

        for (int i = 0; i < Sensors.Length; i++)
        {
            total += IoTData[i];
        }
        for (int i = 0; i < Sensors.Length; i++)
        {
            float size = IoTData[i] / total;
            CreateWedge(mWedges[i], size - Margin, zRotation, WedgeColors[i]);
            zRotation -= size * CHART_DEGREE + Margin;
        }
    }

    private void CreateWedge(Image wedge, float value, float zRotation, Color color)
    {
        wedge.color = color;
        wedge.fillAmount = value;
        wedge.rectTransform.sizeDelta = new Vector2(CHART_SIZE, CHART_SIZE);
        wedge.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, zRotation));
        //wedge.rectTransform.Rotate(Vector3.forward);
    }

    private void CreateLabel(TextMeshPro labelTMPro, string text, float size, float distance, float y, TMP_FontAsset font, Color color)
    {
        labelTMPro.text = text;
        labelTMPro.fontStyle = FontStyles.Italic;
        labelTMPro.fontSize = size;
        labelTMPro.alignment = TextAlignmentOptions.Left | TextAlignmentOptions.Top;
        labelTMPro.enableWordWrapping = false;
        labelTMPro.font = font;
        if (color == null) labelTMPro.color = Color.white;
        else labelTMPro.color = color;

        // location
        labelTMPro.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(distance, y, 0);
    }

    private float[] Normalize(float[] server, Sensor[] sensors)
    {
        var data = new float[sensors.Length];

        for (int i = 0; i < sensors.Length; i++)
        {
            var x = Mathf.Clamp(server[i], sensors[i].min, sensors[i].max);
            data[i] = (x - sensors[i].min) / (sensors[i].max - sensors[i].min);
        }

        return data;
    }
}
